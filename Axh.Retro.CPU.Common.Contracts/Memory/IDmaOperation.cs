using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    /// <summary>
    /// A DMA operation.
    /// </summary>
    public interface IDmaOperation
    {
        /// <summary>
        /// Gets the execution cpu timings.
        /// </summary>
        /// <value>
        /// The execution cpu timings.
        /// </value>
        InstructionTimings Timings { get; }

        /// <summary>
        /// Gets addresses ranges that should be locked for reading and writing during this dma operation.
        /// </summary>
        /// <value>
        /// The locked addresses ranges.
        /// </value>
        IEnumerable<AddressRange> LockedAddressesRanges { get; }

        /// <summary>
        /// Executes the dma operation.
        /// </summary>
        /// <param name="mmu">The mmu.</param>
        void Execute(IMmu mmu);
    }
}