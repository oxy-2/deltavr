using System.Collections.Generic;
using System.Linq;
using Antmicro.Renode.Core;
using Antmicro.Renode.Logging;
using Antmicro.Renode.Peripherals.SPI;
using System.Text;

namespace Antmicro.Renode.Peripherals.Wireless
{
    public class NRF24L01 : ISPIPeripheral, IGPIOReceiver
    {

        public void InjectPacket(string text, int pipe = 1)
        {
            lock(Sync)
            {
                if(rxFifo.Count >= 3) { return; }
                rxFifo.Enqueue(new Packet { Payload = Encoding.ASCII.GetBytes(text), Pipe = pipe });
                irqFlags |= 0x40;   // RX_DR
                UpdateIrq();
            }
        }
        public NRF24L01()
        {
            IRQ = new GPIO();
            lock(Sync)
            {
                Instances.Add(this);
            }
            Reset();
        }

        public GPIO IRQ { get; }

        public void Reset()
        {
            lock(Sync)
            {
                regs = new byte[0x1E][];
                for(var i = 0; i < regs.Length; i++)
                {
                    regs[i] = new byte[1];
                }
                regs[0x0A] = new byte[] { 0xE7, 0xE7, 0xE7, 0xE7, 0xE7 };
                regs[0x0B] = new byte[] { 0xC2, 0xC2, 0xC2, 0xC2, 0xC2 };
                regs[0x10] = new byte[] { 0xE7, 0xE7, 0xE7, 0xE7, 0xE7 };
                regs[0x00][0] = 0x08;  // CONFIG
                regs[0x01][0] = 0x3F;  // EN_AA
                regs[0x02][0] = 0x03;  // EN_RXADDR
                regs[0x03][0] = 0x03;  // SETUP_AW (5 bytes)
                regs[0x04][0] = 0x03;  // SETUP_RETR
                regs[0x05][0] = 0x02;  // RF_CH
                regs[0x06][0] = 0x0E;  // RF_SETUP
                regs[0x0C][0] = 0xC3;
                regs[0x0D][0] = 0xC4;
                regs[0x0E][0] = 0xC5;
                regs[0x0F][0] = 0xC6;
                regs[0x1C][0] = 0x00;  // DYNPD
                regs[0x1D][0] = 0x00;  // FEATURE

                rxFifo.Clear();
                txFifo.Clear();
                txBuffer.Clear();
                irqFlags = 0;
                command = null;
                csnHigh = true;
                ce = false;
                UpdateIrq();
            }
        }

        public void OnGPIO(int number, bool value)
        {
            lock(Sync)
            {
                if(number == 0)
                {
                    csnHigh = value;
                    if(value)
                    {
                        EndCommand();
                    }
                }
                else if(number == 1)
                {
                    ce = value;
                    if(ce)
                    {
                        TryTransmit();
                    }
                }
            }
        }

        public byte Transmit(byte data)
        {
            lock(Sync)
            {
                if(csnHigh)
                {
                    return 0xFF;
                }

                if(!command.HasValue)
                {
                    command = data;
                    index = 0;
                    txBuffer.Clear();
                    if(data == 0xE1) { txFifo.Clear(); }          // FLUSH_TX
                    else if(data == 0xE2) { rxFifo.Clear(); }     // FLUSH_RX
                    return GetStatus();
                }

                var cmd = command.Value;
                byte result = 0;

                if((cmd & 0xE0) == 0x00)                          // R_REGISTER
                {
                    result = ReadReg(cmd & 0x1F, index);
                }
                else if((cmd & 0xE0) == 0x20)                     // W_REGISTER
                {
                    WriteReg(cmd & 0x1F, index, data);
                }
                else if(cmd == 0x61)                              // R_RX_PAYLOAD
                {
                    if(rxFifo.Count > 0)
                    {
                        var p = rxFifo.Peek().Payload;
                        result = index < p.Length ? p[index] : (byte)0;
                    }
                }
                else if(cmd == 0x60)                              // R_RX_PL_WID
                {
                    result = rxFifo.Count > 0 ? (byte)rxFifo.Peek().Payload.Length : (byte)0;
                }
                else if(cmd == 0xA0 || cmd == 0xB0)               // W_TX_PAYLOAD (/NO_ACK)
                {
                    txBuffer.Add(data);
                }
                else if(cmd == 0xE1 || cmd == 0xE2 || cmd == 0xFF || (cmd & 0xF8) == 0xA8)
                {
                    // FLUSH / NOP / W_ACK_PAYLOAD: nothing more to do
                }
                else
                {
                    this.Log(LogLevel.Warning, "Unhandled command 0x{0:X2}", cmd);
                }

                index++;
                return result;
            }
        }

        public void FinishTransmission()
        {
        }

        private void EndCommand()
        {
            if(!command.HasValue)
            {
                return;
            }
            var cmd = command.Value;
            command = null;

            if((cmd == 0xA0 || cmd == 0xB0) && txBuffer.Count > 0)
            {
                if(txFifo.Count < 3)
                {
                    txFifo.Enqueue(txBuffer.ToArray());
                }
                else
                {
                    this.Log(LogLevel.Warning, "TX FIFO full, payload dropped");
                }
                txBuffer.Clear();
                TryTransmit();
            }
            else if(cmd == 0x61 && rxFifo.Count > 0)
            {
                rxFifo.Dequeue();
            }
        }

        // radio logic

        private bool PowerUp => (regs[0x00][0] & 0x02) != 0;
        private bool PrimRx => (regs[0x00][0] & 0x01) != 0;
        private int Channel => regs[0x05][0] & 0x7F;
        private int AddrWidth => regs[0x03][0] == 1 ? 3 : (regs[0x03][0] == 2 ? 4 : 5);

        private void TryTransmit()
        {
            while(ce && PowerUp && !PrimRx && txFifo.Count > 0 && (irqFlags & 0x10) == 0)
            {
                var payload = txFifo.Peek();
                var address = regs[0x10].Take(AddrWidth).ToArray();
                var delivered = false;

                foreach(var other in Instances.Where(x => !ReferenceEquals(x, this)).ToList())
                {
                    if(other.Receive(Channel, regs[0x06][0] & 0x28, address, payload))
                    {
                        delivered = true;
                    }
                }

                var autoAck = (regs[0x01][0] & 0x01) != 0;
                if(delivered || !autoAck)
                {
                    txFifo.Dequeue();
                    irqFlags |= 0x20;   // TX_DS
                }
                else
                {
                    irqFlags |= 0x10;   // MAX_RT, payload stays in the FIFO
                }
                this.Log(LogLevel.Debug, "TX {0} bytes ch{1}: delivered={2}", payload.Length, Channel, delivered);
                UpdateIrq();
            }
        }

        private bool Receive(int channel, int rate, byte[] address, byte[] payload)
        {
            if(!PowerUp || !PrimRx || !ce || Channel != channel || (regs[0x06][0] & 0x28) != rate)
            {
                return false;
            }
            var pipe = FindPipe(address);
            if(pipe < 0)
            {
                return false;
            }
            if(rxFifo.Count >= 3)
            {
                return false;
            }
            rxFifo.Enqueue(new Packet { Payload = (byte[])payload.Clone(), Pipe = pipe });
            irqFlags |= 0x40;           // RX_DR
            this.Log(LogLevel.Debug, "RX {0} bytes on pipe {1}", payload.Length, pipe);
            UpdateIrq();
            return true;
        }

        private int FindPipe(byte[] address)
        {
            var w = AddrWidth;
            if(address.Length < w)
            {
                return -1;
            }
            for(var pipe = 0; pipe < 6; pipe++)
            {
                if((regs[0x02][0] & (1 << pipe)) == 0)
                {
                    continue;
                }
                byte[] pipeAddr;
                if(pipe == 0) { pipeAddr = (byte[])regs[0x0A].Clone(); }
                else
                {
                    pipeAddr = (byte[])regs[0x0B].Clone();
                    if(pipe > 1) { pipeAddr[0] = regs[0x0A + pipe][0]; }
                }
                if(pipeAddr.Take(w).SequenceEqual(address.Take(w)))
                {
                    return pipe;
                }
            }
            return -1;
        }

        // registers

        private byte GetStatus()
        {
            var pipe = rxFifo.Count > 0 ? rxFifo.Peek().Pipe : 7;
            return (byte)((irqFlags & 0x70) | (pipe << 1) | (txFifo.Count >= 3 ? 1 : 0));
        }

        private byte ReadReg(int reg, int i)
        {
            if(reg >= regs.Length) { return 0; }
            if(reg == 0x07) { return GetStatus(); }
            if(reg == 0x17)
            {
                return (byte)((rxFifo.Count == 0 ? 0x01 : 0) | (rxFifo.Count >= 3 ? 0x02 : 0)
                            | (txFifo.Count == 0 ? 0x10 : 0) | (txFifo.Count >= 3 ? 0x20 : 0));
            }
            return i < regs[reg].Length ? regs[reg][i] : (byte)0;
        }

        private void WriteReg(int reg, int i, byte data)
        {
            if(reg >= regs.Length) { return; }
            if(reg == 0x07)
            {
                irqFlags &= (byte)~(data & 0x70);   // write 1 to clear
                UpdateIrq();
                TryTransmit();
                return;
            }
            if(reg == 0x08 || reg == 0x09 || reg == 0x17) { return; }  // read-only
            if(i < regs[reg].Length)
            {
                regs[reg][i] = data;
            }
            if(reg == 0x00)
            {
                UpdateIrq();
                TryTransmit();
            }
        }

        private void UpdateIrq()
        {
            var cfg = regs[0x00][0];
            var active = ((irqFlags & 0x40) != 0 && (cfg & 0x40) == 0)
                      || ((irqFlags & 0x20) != 0 && (cfg & 0x20) == 0)
                      || ((irqFlags & 0x10) != 0 && (cfg & 0x10) == 0);
            IRQ.Set(!active);   // active low
        }

        private class Packet
        {
            public byte[] Payload;
            public int Pipe;
        }

        private byte[][] regs;
        private readonly Queue<Packet> rxFifo = new Queue<Packet>();
        private readonly Queue<byte[]> txFifo = new Queue<byte[]>();
        private readonly List<byte> txBuffer = new List<byte>();
        private byte? command;
        private int index;
        private byte irqFlags;
        private bool csnHigh = true;
        private bool ce;

        // One global lock: the two machines may run on different threads, and
        // radios call into each other, so per-instance locks could deadlock.
        private static readonly object Sync = new object();
        private static readonly List<NRF24L01> Instances = new List<NRF24L01>();
    }
}