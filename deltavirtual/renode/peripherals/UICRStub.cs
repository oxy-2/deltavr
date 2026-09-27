//
// UICRStub.cs
//
// Minimal Nordic UICR (User Information Configuration Registers) model.
// Renode's SVD-generated UICR region treats NFCPINS as unimplemented, so
// reads return 0xFFFFFFFF and writes are dropped. Zephyr's nRF5 SystemInit
// then sees NFC pins still "protected", writes NFCPINS to switch them to
// GPIO, and issues NVIC_SystemReset() — which re-enters the same path and
// resets forever.
//
// This stub just stores values and powers up with NFCPINS protect disabled
// (0xFFFFFFFE), which is what SystemInit wants to see.
//
// Load before the platform description:
//   i @peripherals/UICRStub.cs

using System;
using System.Collections.Generic;
using Antmicro.Renode.Core;
using Antmicro.Renode.Peripherals;
using Antmicro.Renode.Peripherals.Bus;

namespace Antmicro.Renode.Peripherals.Misc
{
    public class UICRStub : IDoubleWordPeripheral, IKnownSize
    {
        public UICRStub()
        {
            // NFCPINS @ 0x20C: bit 0 = PROTECT (0 = disabled / GPIO mode).
            // Power up already in GPIO mode so SystemInit does not reset.
            registers[0x20C] = 0xFFFFFFFE;
            registers[0x014] = 0xFFFFFFFF;
            registers[0x018] = 0xFFFFFFFF;
            registers[0x304] = 0xFFFFFFFF;
        }

        public uint ReadDoubleWord(long offset)
        {
            uint value;
            registers.TryGetValue((uint)offset, out value);
            return value;
        }

        public void WriteDoubleWord(long offset, uint value)
        {
            registers[(uint)offset] = value;
        }

        public void Reset()
        {
            registers.Clear();
            registers[0x20C] = 0xFFFFFFFE;
            registers[0x014] = 0xFFFFFFFF;
            registers[0x018] = 0xFFFFFFFF;
            registers[0x304] = 0xFFFFFFFF;
        }

        public long Size { get { return 0x1000; } }

        private readonly Dictionary<uint, uint> registers = new Dictionary<uint, uint>();
    }
}
