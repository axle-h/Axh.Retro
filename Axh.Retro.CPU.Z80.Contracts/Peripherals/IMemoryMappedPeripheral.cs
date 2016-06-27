using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.CPU.Z80.Contracts.Peripherals
{
    public interface IMemoryMappedPeripheral : IPeripheral
    {
        IEnumerable<IAddressSegment> AddressSegments { get; }
    }
}