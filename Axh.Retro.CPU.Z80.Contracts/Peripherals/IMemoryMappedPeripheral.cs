namespace Axh.Retro.CPU.Z80.Contracts.Peripherals
{
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Contracts.Memory;

    public interface IMemoryMappedPeripheral : IPeripheral
    {
        IEnumerable<IAddressSegment> AddressSegments { get; }
    }
}
