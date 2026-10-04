using System;
using Antmicro.Renode.Core;
using Antmicro.Renode.Logging;
using Antmicro.Renode.Peripherals.SPI;

namespace Antmicro.Renode.Peripherals.Sensors
{
    // Simplified LSM6DSV (SPI only). Register map and scaling follow the LSM6DSV
    // datasheet; FIFO, interrupts, FSM and sensor fusion are NOT modelled.
    // GPIO input 0 = CS (active low).
    // Sensor values are set from the monitor, e.g.  sysbus.spi2.imu AccelZ 1.0
    public class LSM6DSV : ISPIPeripheral, IGPIOReceiver
    {
        public LSM6DSV()
        {
            Reset();
        }

        // ---- values you set from the monitor (physical units) ----
        public double AccelX { get; set; }       // g
        public double AccelY { get; set; }
        public double AccelZ { get; set; } = 1.0;
        public double GyroX { get; set; }        // dps
        public double GyroY { get; set; }
        public double GyroZ { get; set; }
        public double Temperature { get; set; } = 25.0;   // deg C

        public void Reset()
        {
            regs = new byte[0x80];
            regs[0x0F] = 0x70;     // WHO_AM_I
            regs[0x12] = 0x04;     // CTRL3: IF_INC = 1
            csHigh = true;
            started = false;
        }

        public void OnGPIO(int number, bool value)
        {
            if(number != 0) { return; }
            csHigh = value;
            if(value) { started = false; }
        }

        public byte Transmit(byte data)
        {
            if(csHigh) { return 0; }

            if(!started)
            {
                started = true;
                isRead = (data & 0x80) != 0;
                address = data & 0x7F;
                return 0;
            }

            byte result = 0;
            if(isRead) { result = ReadReg(address); }
            else { WriteReg(address, data); }

            if((regs[0x12] & 0x04) != 0)           // IF_INC
            {
                address = (address + 1) & 0x7F;
            }
            return result;
        }

        // Ignored on purpose: the nRF SPI controller may call this after every
        // DMA chunk; only CS going high ends a transaction (see OnGPIO).
        public void FinishTransmission()
        {
        }

        private byte ReadReg(int addr)
        {
            switch(addr)
            {
                case 0x1E:                          // STATUS_REG
                    var xl = (regs[0x10] & 0x0F) != 0;
                    var g = (regs[0x11] & 0x0F) != 0;
                    return (byte)((xl ? 0x01 : 0) | (g ? 0x02 : 0) | ((xl || g) ? 0x04 : 0));
                case 0x20: case 0x21:               // OUT_TEMP
                    return Byte((short)Clamp((Temperature - 25.0) * 256.0), addr - 0x20);
                case 0x22: case 0x23: return Byte(GyroRaw(GyroX), addr - 0x22);
                case 0x24: case 0x25: return Byte(GyroRaw(GyroY), addr - 0x24);
                case 0x26: case 0x27: return Byte(GyroRaw(GyroZ), addr - 0x26);
                case 0x28: case 0x29: return Byte(AccelRaw(AccelX), addr - 0x28);
                case 0x2A: case 0x2B: return Byte(AccelRaw(AccelY), addr - 0x2A);
                case 0x2C: case 0x2D: return Byte(AccelRaw(AccelZ), addr - 0x2C);
                default:
                    return regs[addr];
            }
        }

        private void WriteReg(int addr, byte value)
        {
            if(addr == 0x0F || (addr >= 0x1E && addr <= 0x2D))
            {
                return;                             // read-only
            }
            if(addr == 0x12 && (value & 0x01) != 0)
            {
                Reset();                            // SW_RESET
                csHigh = false;
                started = true;
                return;
            }
            regs[addr] = value;
        }

        private static byte Byte(short raw, int which)
        {
            return which == 0 ? (byte)(raw & 0xFF) : (byte)((raw >> 8) & 0xFF);
        }

        private static double Clamp(double v)
        {
            return Math.Max(short.MinValue, Math.Min(short.MaxValue, Math.Round(v)));
        }

        // CTRL8 (0x17) bits 1:0: 2/4/8/16 g -> 0.061/0.122/0.244/0.488 mg/LSB
        private short AccelRaw(double g)
        {
            var sens = new[] { 0.061, 0.122, 0.244, 0.488 }[regs[0x17] & 0x03];
            return (short)Clamp(g * 1000.0 / sens);
        }

        // CTRL6 (0x15) bits 3:0: 125/250/500/1000/2000/4000 dps
        private short GyroRaw(double dps)
        {
            double sens;
            switch(regs[0x15] & 0x0F)
            {
                case 0: sens = 4.375; break;
                case 1: sens = 8.75; break;
                case 2: sens = 17.5; break;
                case 3: sens = 35.0; break;
                case 12: sens = 140.0; break;
                default: sens = 70.0; break;        // 4 = 2000 dps
            }
            return (short)Clamp(dps * 1000.0 / sens);
        }

        private byte[] regs;
        private bool csHigh = true;
        private bool started;
        private bool isRead;
        private int address;
    }
}