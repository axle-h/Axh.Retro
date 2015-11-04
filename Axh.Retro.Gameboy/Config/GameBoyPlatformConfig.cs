namespace Axh.Retro.GameBoy.Config
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Common.Config;
    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Config;

    public class GameBoyPlatformConfig : IPlatformConfig
    {
        private const double CpuFrequency = 4.194304;
        private const ushort SystemMemoryBankLength = 0x0fff;
        private const ushort SystemMemoryBank0Address = 0xc000;
        private const ushort SystemMemoryBank1Address = 0xd000;

        public GameBoyPlatformConfig()
        {
            var systemMemoryBank0Config = new SimpleMemoryBankConfig(MemoryBankType.RandomAccessMemory, null, SystemMemoryBank0Address, SystemMemoryBankLength);
            var systemMemoryBank1Config = new SimpleMemoryBankConfig(MemoryBankType.RandomAccessMemory, null, SystemMemoryBank1Address, SystemMemoryBankLength);

            this.MemoryBanks = new[] { systemMemoryBank0Config, systemMemoryBank1Config };
        }

        public CpuMode CpuMode => CpuMode.GameBoy;

        public IEnumerable<IMmuBankConfig> MemoryBanks { get; }

        /// <summary>
        /// GB rounds all machine cycles to 4 throttling states. I.e. we need to run timing based on machine cycles.
        /// </summary>
        public double? MachineCycleSpeedMhz => CpuFrequency;

        public double? ThrottlingStateSpeedMhz => null;    
    }
}
