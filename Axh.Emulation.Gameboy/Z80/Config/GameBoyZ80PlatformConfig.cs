namespace Axh.Emulation.GameBoy.Z80.Config
{
    using System.Collections.Generic;

    using Axh.Emulation.CPU.Z80.Contracts.Config;

    public class GameBoyZ80PlatformConfig : IZ80PlatformConfig
    {
        private const WriteFaultMode Z80WriteFaultMode = WriteFaultMode.Continue;

        private const ushort SystemMemoryBankLength = 0x0fff;
        private const ushort SystemMemoryBank0Address = 0xc000;
        private const ushort SystemMemoryBank1Address = 0xd000;

        public GameBoyZ80PlatformConfig()
        {
            var systemMemoryBank0Config = new MemoryBankConfig(0, SystemMemoryBank0Address, SystemMemoryBankLength);
            var systemMemoryBank1Config = new MemoryBankConfig(1, SystemMemoryBank1Address, SystemMemoryBankLength);

            this.RandomAccessMemoryBanks = new[] { systemMemoryBank0Config, systemMemoryBank1Config };
        }

        public WriteFaultMode WriteFaultMode
        {
            get
            {
                return Z80WriteFaultMode;
            }
        }

        public IEnumerable<IMemoryBankConfig> RandomAccessMemoryBanks { get; private set; }
    }
}
