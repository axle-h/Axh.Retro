namespace Axh.Emulation.CPU.X80.Factories
{
    using System;
    using System.Linq;

    using Axh.Emulation.CPU.X80.Contracts;
    using Axh.Emulation.CPU.X80.Contracts.Config;
    using Axh.Emulation.CPU.X80.Contracts.Factories;
    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Memory;
    using Axh.Emulation.CPU.X80.Mmu;

    public class MmuFactory : IMmuFactory
    {
        private readonly IPlatformConfig platformConfig;

        public MmuFactory(IPlatformConfig platformConfig)
        {
            this.platformConfig = platformConfig;
        }

        public IMmu GetMmu()
        {
            var memoryBanks = this.platformConfig.RandomAccessMemoryBanks.Select(GetAddressSegment).ToArray();

            switch (platformConfig.CpuMode)
            {
                case CpuMode.Intel8080:
                    throw new NotImplementedException();
                case CpuMode.GameBoy:
                    throw new NotImplementedException();
                case CpuMode.Z80:
                    return new SegmentMmu(memoryBanks);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IMmuCache GetMmuCache(IMmu mmu, ushort address)
        {
            return new MmuCache(mmu, address);
        }

        private static IReadableAddressSegment GetAddressSegment(IMemoryBankConfig config)
        {
            return new ArrayBackedMemoryBank(config);
        }
    }
}
