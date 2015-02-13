namespace Axh.Emulation.CPU.Z80.Factories
{
    using System.Linq;

    using Axh.Emulation.CPU.Z80.Contracts;
    using Axh.Emulation.CPU.Z80.Contracts.Config;
    using Axh.Emulation.CPU.Z80.Contracts.Memory;
    using Axh.Emulation.CPU.Z80.Memory;

    public class MmuFactory : IMmuFactory
    {
        private readonly IZ80PlatformConfig platformConfig;

        public MmuFactory(IZ80PlatformConfig platformConfig)
        {
            this.platformConfig = platformConfig;
        }

        public IMmu GetMmu()
        {
            var memoryBanks = this.platformConfig.RandomAccessMemoryBanks.Select(GetAddressSegment).ToArray();

            return new Z80Mmu(platformConfig, memoryBanks);
        }

        private static IAddressSegment GetAddressSegment(IMemoryBankConfig config)
        {
            return new ArrayBackedMemoryBank(config);
        }
    }
}
