namespace Axh.Retro.CPU.X80.Factories
{
    using System;
    using System.Linq;

    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.CPU.X80.Memory;

    public class MmuFactory : IMmuFactory
    {
        private readonly IPlatformConfig platformConfig;

        public MmuFactory(IPlatformConfig platformConfig, IPeripheralFactory peripheralFactory)
        {
            this.platformConfig = platformConfig;
        }

        public IMmu GetMmu(IPeripheralManager peripheralManager)
        {
            var memoryBanks = this.platformConfig.MemoryBanks.OfType<IMemoryBankConfig>().Select(GetAddressSegment).ToArray();

            var mmu = new SegmentMmu(memoryBanks.Concat(peripheralManager.GetAllMemoryMappedPeripherals().SelectMany(x => x.AddressSegments)));
            
            peripheralManager.RegisterDmaForIOPeripherals(mmu);

            return mmu;
        }

        public IMmuCache GetMmuCache(IMmu mmu, ushort address)
        {
            return new MmuCache(mmu, address);
        }

        private static IAddressSegment GetAddressSegment(IMemoryBankConfig config)
        {
            switch (config.Type)
            {
                case MemoryBankType.RandomAccessMemory:
                    return new ArrayBackedMemoryBank(config);
                case MemoryBankType.ReadOnlyMemory:
                    return new ReadOnlyMemoryBank(config);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
