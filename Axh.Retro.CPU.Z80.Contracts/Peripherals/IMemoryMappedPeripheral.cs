using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.CPU.Z80.Contracts.Peripherals
{
    /// <summary>
    /// A peripheral called through the z80's address space.
    /// </summary>
    public interface IMemoryMappedPeripheral : IPeripheral
    {
        /// <summary>
        /// Gets the address segments.
        /// </summary>
        /// <value>
        /// The address segments.
        /// </value>
        IEnumerable<IAddressSegment> AddressSegments { get; }
    }
}