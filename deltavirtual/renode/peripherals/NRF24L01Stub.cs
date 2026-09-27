//
// NRF24L01Stub.cs
//
// A deliberately SIMPLIFIED nRF24L01+ model for Renode, enough to exercise
// real firmware driver code (SPI command bytes, STATUS/FIFO register
// semantics, CE-triggered TX, IRQ-triggered RX) across two simulated
// boards in the SAME Renode process.
//
// NOT modeled (add if your firmware depends on it): auto-ack / retries,
// multiple RX pipes with per-pipe addressing, dynamic payload length,
// channel/frequency, ESB timing. Point-to-point delivery here ignores
// TX_ADDR/RX_ADDR entirely and just delivers to "the other registered node" -
// fine for a 2-board headset<->controller link, not fine for >2 nodes.
//
// Load this ad-hoc in the Renode monitor / .resc with:
//   i @peripherals/NRF24L01Stub.cs
// before `machine LoadPlatformDescription`, since the .repl file below
// references the Peripherals.SPI.NRF24L01Stub type.
//
using System;
using System.Collections.Generic;
using Antmicro.Renode.Core;
using Antmicro.Renode.Core.Structure;
using Antmicro.Renode.Logging;
using Antmicro.Renode.Peripherals.SPI;
using Antmicro.Renode.Peripherals.GPIOPort;

namespace Antmicro.Renode.Peripherals.SPI
{
    public class NRF24L01Stub : ISPIPeripheral, IGPIOReceiver
    {
        public NRF24L01Stub()
        {
            IRQ = new GPIO();
            txFifo = new Queue<byte>();
            rxFifo = new Queue<byte[]>();
            status = 0x0E; // TX_FULL clear, RX_P_NO=7 (empty), reset default-ish
        }

        // Call from .resc: sysbus.nrf24 NodeId "headset"
        public string NodeId
        {
            get { return nodeId; }
            set
            {
                nodeId = value;
                lock (Registry)
                {
                    Registry[nodeId] = this;
                }
            }
        }

        public GPIO IRQ { get; private set; }

        public void Reset()
        {
            txFifo.Clear();
            rxFifo.Clear();
            status = 0x0E;
            csnLow = false;
            byteIndex = 0;
            currentCommand = 0;
            currentRegister = 0;
        }

        // ---- IGPIOReceiver: pin 0 = CSN, pin 1 = CE (see .repl wiring) ----
        public void OnGPIO(int number, bool value)
        {
            if (number == 0) // CSN
            {
                if (!value) // falling edge: CSN asserted (active low), start txn
                {
                    csnLow = true;
                    byteIndex = 0;
                }
                else // rising edge: end of transaction
                {
                    csnLow = false;
                }
            }
            else if (number == 1) // CE
            {
                if (value && txFifo.Count > 0)
                {
                    DeliverToPeer();
                }
            }
        }

        // ---- ISPIPeripheral ----
        public byte Transmit(byte data)
        {
            if (!csnLow)
            {
                return 0xFF;
            }

            byte response;

            if (byteIndex == 0)
            {
                currentCommand = data;
                response = status; // nRF24 always shifts out STATUS on cmd byte
            }
            else
            {
                response = HandleDataByte(data);
            }

            byteIndex++;
            return response;
        }

        public void FinishTransmission()
        {
            // CSN rising edge already handled in OnGPIO; nothing extra needed
            // unless you want to latch partial register writes here instead.
        }

        private byte HandleDataByte(byte data)
        {
            byte cmdHigh = (byte)(currentCommand & 0xE0);

            if (currentCommand == 0xFF) // NOP
            {
                return status;
            }
            if (currentCommand == 0xE1) // FLUSH_TX
            {
                txFifo.Clear();
                return status;
            }
            if (currentCommand == 0xE2) // FLUSH_RX
            {
                rxFifo.Clear();
                return status;
            }
            if (currentCommand == 0xA0) // W_TX_PAYLOAD
            {
                txFifo.Enqueue(data);
                return status;
            }
            if (currentCommand == 0x61) // R_RX_PAYLOAD
            {
                if (pendingRxRead == null)
                {
                    pendingRxRead = rxFifo.Count > 0 ? rxFifo.Dequeue() : new byte[0];
                    pendingRxIndex = 0;
                }
                byte outByte = pendingRxIndex < pendingRxRead.Length ? pendingRxRead[pendingRxIndex] : (byte)0;
                pendingRxIndex++;
                if (pendingRxIndex >= pendingRxRead.Length)
                {
                    pendingRxRead = null;
                }
                return outByte;
            }
            if (cmdHigh == 0x20) // W_REGISTER
            {
                currentRegister = (byte)(currentCommand & 0x1F);
                if (currentRegister == 0x07) // STATUS: write-1-to-clear
                {
                    status &= (byte)~(data & 0x70);
                    UpdateIrq();
                }
                else
                {
                    registers[currentRegister] = data;
                }
                return status;
            }
            if (cmdHigh == 0x00) // R_REGISTER
            {
                currentRegister = (byte)(currentCommand & 0x1F);
                byte val;
                registers.TryGetValue(currentRegister, out val);
                return currentRegister == 0x07 ? status : val;
            }

            return status;
        }

        private void DeliverToPeer()
        {
            var payload = txFifo.ToArray();
            txFifo.Clear();
            status |= 0x20; // TX_DS
            UpdateIrq();

            NRF24L01Stub peer = null;
            lock (Registry)
            {
                foreach (var kv in Registry)
                {
                    if (kv.Key != nodeId)
                    {
                        peer = kv.Value;
                        break;
                    }
                }
            }
            if (peer != null)
            {
                peer.ReceiveFromPeer(payload);
            }
            else
            {
                this.Log(LogLevel.Warning, "NRF24L01Stub '{0}': no peer registered yet, payload dropped", nodeId);
            }
        }

        private void ReceiveFromPeer(byte[] payload)
        {
            rxFifo.Enqueue(payload);
            status |= 0x40; // RX_DR
            UpdateIrq();
        }

        private void UpdateIrq()
        {
            // Active low: assert (drive low = true->false on real pin, but
            // Renode GPIO.Set(bool) treats true as asserted logical level;
            // invert here so firmware polling "IRQ low = event" works as-is
            // if you wire IRQ straight through without inverting in the repl.
            bool anyEvent = (status & 0x70) != 0;
            IRQ.Set(!anyEvent);
        }

        private static readonly Dictionary<string, NRF24L01Stub> Registry = new Dictionary<string, NRF24L01Stub>();

        private readonly Dictionary<byte, byte> registers = new Dictionary<byte, byte>();
        private readonly Queue<byte> txFifo;
        private readonly Queue<byte[]> rxFifo;
        private byte[] pendingRxRead;
        private int pendingRxIndex;

        private string nodeId = "unnamed";
        private bool csnLow;
        private int byteIndex;
        private byte currentCommand;
        private byte currentRegister;
        private byte status;
    }
}
