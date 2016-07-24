using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Core;

namespace Axh.Retro.CPU.Z80.Cache
{
    /// <summary>
    /// An instruction block cache item.
    /// </summary>
    internal interface ICachedInstructionBlock
    {
        /// <summary>
        /// Gets the instruction block.
        /// </summary>
        /// <value>
        /// The instruction block.
        /// </value>
        IInstructionBlock InstructionBlock { get; }

        /// <summary>
        /// Gets or sets the accessed count.
        /// </summary>
        /// <value>
        /// The accessed count.
        /// </value>
        uint AccessedCount { get; set; }

        /// <summary>
        /// Gets the address ranges.
        /// </summary>
        /// <value>
        /// The address ranges.
        /// </value>
        IEnumerable<AddressRange> AddressRanges { get; }

        /// <summary>
        /// Checks if the specified range intersects this cached instruction block.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        bool Intersects(AddressRange range);
    }
}