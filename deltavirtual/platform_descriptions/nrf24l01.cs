using System.Collections.Generic;
using Antmicro.Renode.Core;
using Antmicro.Renode.Logging;
using Antmicro.Renode.Peripherals.SPI;

namespace Antmicro.Renode.Peripherals.Wireless
{
    public class nrf24l01 : ISPIPeripheral, IGPIOReceiver
    {
        public nrf24l01()
        {
            IRQ = new GPIO();
            Reset();
        }

        public GPIO IRQ { get; }

        public void Reset()
        {
            regs = new byte[0x1E];
            regs[0x00] = 0x08;   // CONFIG
            regs[0x01] = 0x3F;   // EN_AA
            regs[0x02] = 0x03;   // EN_RXADDR
            regs[0x03] = 0x03;   // SETUP_AW
            regs[0x04] = 0x03;   // SETUP_RETR
            regs[0x05] = 0x02;   // RF_CH
            regs[0x06] = 0x0E;   // RF_SETUP
            regs[0x07] = 0x0E;   // STATUS
            command = null;
            IRQ.Unset();
        }

        // GPIO inputs: 0 = CSN, 1 = CE (matches the .repl numbering)
        public void OnGPIO(int number, bool value)
        {
            if(number == 0)
            {
                csnHigh = value;
                if(value) { FinishTransmission(); }
            }
            else if(number == 1)
            {
                ce = value;
                // TODO: if ce && PTX mode && TX FIFO non-empty -> send packet
            }
        }

        public byte Transmit(byte data)
        {
            if(csnHigh) { return 0xFF; }   // ignore traffic when not selected

            if(command == null)
            {
                command = data;
                return regs[0x07];         // first byte back is always STATUS
            }

            byte result = 0;
            var cmd = command.Value;

            if((cmd & 0xE0) == 0x00)       // R_REGISTER
            {
                result = regs[cmd & 0x1F];
            }
            else if((cmd & 0xE0) == 0x20)  // W_REGISTER
            {
                regs[cmd & 0x1F] = data;
            }
            else
            {
                this.Log(LogLevel.Warning, "Unhandled command 0x{0:X2}", cmd);
            }
            return result;
        }

        public void FinishTransmission()
        {
            command = null;
        }

        private byte[] regs;
        private byte? command;
        private bool csnHigh = true;
        private bool ce;
    }
}