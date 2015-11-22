namespace Axh.Retro.CPU.Z80.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Common.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;

    /// <summary>
    /// Z80 MMU builds a segment MMU taking memory bank configs from IPlatformConfig and IPeripheralManager
    /// </summary>
    public class Z80Mmu : SegmentMmu
    {
        public Z80Mmu(IPeripheralManager peripheralManager, IPlatformConfig platformConfig)
            : base(GetAddressSegments(peripheralManager, platformConfig))
        {
        }

        private static IEnumerable<IAddressSegment> GetAddressSegments(IPeripheralManager peripheralManager, IPlatformConfig platformConfig)
        {
            var memoryBanks = platformConfig.MemoryBanks.Select(GetAddressSegment).ToArray();
            return memoryBanks.Concat(peripheralManager.GetAllMemoryMappedPeripherals().SelectMany(x => x.AddressSegments));
        }

        private static IAddressSegment GetAddressSegment(IMemoryBankConfig config)
        {
            switch (config.Type)
            {
                case MemoryBankType.RandomAccessMemory:
                    return new ArrayBackedMemoryBank(config);
                case MemoryBankType.ReadOnlyMemory:
                    return new ReadOnlyMemoryBank(config);
                case MemoryBankType.Unused:
                    return new NullMemoryBank(config);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
