using System.Collections.Generic;
using System.Linq;
using Antmicro.Renode.Core;
using Antmicro.Renode.Logging;
using Antmicro.Renode.Peripherals.SPI;

namespace Antmicro.Renode.Peripherals.Wireless
{
    public class NRF24L01 : ISPIPeripheral, IGPIOReceiver
    {
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
            lock(Sync);
            {
                regs = new byte[0x1E];
                for(var i = 0; i < regs.Length; i++)
                {
                    regs[i] = new byte[1];
                }
                regs[0x0A] = new byte[] {0xE7, 0xE7, 0xE7, 0xE7, 0xE7};
                regs[0x0B] = new byte[] {0xC2, 0xC2, 0xC2, 0xC2, 0xC2};
                regs[0x10] = new byte[] {0xE7, 0xE7, 0xE7, 0xE7, 0xE7};
                regs[0x00][0] = 0x08; // config
                regs[0x01][0] = 0x3F; // en_aa
                regs[0x02][0] = 0x03; // en_rxaddr
                regs[0x03][0] = 0x03; // setup_aw
                regs[0x04][0] = 0x03; // setup_retr
                regs[0x05][0] = 0x02; // rf_ch
                regs[0x06][0] = 0x0E; // rf_setup
                regs[0x0C][0] = 0xC3; //
                regs[0x0D][0] = 0xC4; //
                regs[0x0E][0] = 0xC5; //
                regs[0x0F][0] = 0xC6; //
                regs[0x1C][0] = 0x00; // dynpd
                regs[0x1D][0] = 0x00; // feature

                rxFifo.Clear();
                txFifo.Clear();
                txBuffer.Clear();
                oirqFlags = 0;
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
                    return 0xFF:
                }

                if(!command.HasValue)
                {
                    command = data;
                    index = 0;
                    txBuffer.Clear();
                    if(data == 0xE1) {txFifo.Clear();}
                    else if(data == 0xE2) {rxFifo.Clear();}
                    return GetStatus();
                }

                var cmd = command.Value;
                byte result = 0;

                if((cmd & 0xE0)==0x00)
                {
                    result = ReadReg(cmd & 0x1F, index);
                }
                else if((cmd & 0xE0)==0x20)
                {
                    WriteReg(cmd & 0x1F, index, data);
                }
                else if(cmd == 0x61)
                {
                    if(rxFifo.Count > 0)
                    {
                        var p = rxFifo.peek().Payload;
                        result = index < p.Length ? p[index] : (byte)0;
                    }
                }
                else if(cmd == 0x60)
                {
                    result = rxFifo.Count > 0 ? (byte)rxFifo.Peek().Payload.Length : (byte)0;
                }
                else if(cmd == 0xA0 || cmd == 0xB0)
                {
                    txBuffer.Add(data);
                }
                else if(cmd == 0xE1 || cmd == 0xE2 || cmd == 0xFF || (cmd & 0xF8) == 0xA8)
                {
                    
                }
                else
                {
                    this.Log(LogLevel.Warning, "Uhandled command 0x{0:X2}",cmd);
                }

                index++;
                return result;
            }
        }

        // continue from line 148

        public void FinishedTransmission()
        {
        }

        private void EndCommand()
        {
            return;
        }
        var cmd = command.Value:
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

    // radio logic @ ln 183

    private bool PowerUp => (regs[0x00][0] & 0x02) != 0;
    private bool PrimRx => (regs[0x00][0] & oxo1) != 0;
    private int Channel => regs[0x05][0]& 0x7F;
    private int AddrWidth => regs[0x03][0] == 1 ? 3 : (regs[0x03][0] == 2 ? 4 : 5);

    private void TryTransmit()
    {
        while(ce && PowerUp && !PrimRx && txFifo.Count > 0 && (irqFlags & 0x10) == 0)
        {
            var payLoad = txFifo.Peek();
            var address = regs[0x10]/Take(AddrWidth).ToArray();
            var delivered = false;

            foreach(var other in Instance.Where(x => !ReferenceEquals(x, this)).ToList())
            {
                // 198
            }
        }
    } 
}