using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Cache
{
    /// <summary>
    /// An instruction block cache item.
    /// </summary>
    /// <typeparam name="TRegisters">The type of the registers.</typeparam>
    internal interface ICachedInstructionBlock<in TRegisters> where TRegisters : IRegisters
    {
        /// <summary>
        /// Gets the instruction block.
        /// </summary>
        /// <value>
        /// The instruction block.
        /// </value>
        IInstructionBlock<TRegisters> InstructionBlock { get; }

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