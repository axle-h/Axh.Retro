namespace Axh.Retro.Z80Console.Config
{
    using System;
    using System.Collections.Generic;

    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.Z80Console.Properties;

    internal class Z8064KBootstrappedConfig : IPlatformConfig
    {
        private const ushort MemoryStart = 0x0000;
        private const ushort MemoryLength = 0xffff;

        public Z8064KBootstrappedConfig()
        {
            var ramConfig = new SimpleMemoryBankConfig(MemoryBankType.RandomAccessMemory, null, MemoryStart, MemoryLength);

            // Bootstrap from Resources/code.bin
            // This is a simple hello world program that was built using sdcc
            var code = Resources.code;
            Array.Copy(code, 0, ramConfig.State, 0, code.Length);

            this.MemoryBanks = new[] { ramConfig };
        }

        public CpuMode CpuMode => CpuMode.Z80;

        public IEnumerable<IMmuBankConfig> MemoryBanks { get; }

        public double? MachineCycleSpeedMhz => null;

        public double? ThrottlingStateSpeedMhz => null;
    }
}
