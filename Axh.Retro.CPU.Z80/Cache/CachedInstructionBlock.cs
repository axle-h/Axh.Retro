using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Cache
{
    /// <summary>
    /// Instruction block cache wrapper with two ranges.
    /// </summary>
    internal class CachedInstructionBlock<TRegisters> : ICachedInstructionBlock<TRegisters> where TRegisters : IRegisters
    {
        private readonly AddressRange _addressRange0;
        private readonly AddressRange _addressRange1;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInstructionBlock{TRegisters}"/> class.
        /// </summary>
        /// <param name="addressRanges">The address ranges.</param>
        /// <param name="instructionBlock">The instruction block.</param>
        public CachedInstructionBlock(IReadOnlyList<AddressRange> addressRanges,
            IInstructionBlock<TRegisters> instructionBlock)
        {
            InstructionBlock = instructionBlock;
            _addressRange0 = addressRanges[0];
            _addressRange1 = addressRanges[1];
        }

        /// <summary>
        /// Gets the instruction block.
        /// </summary>
        /// <value>
        /// The instruction block.
        /// </value>
        public IInstructionBlock<TRegisters> InstructionBlock { get; }

        /// <summary>
        /// Gets or sets the accessed count.
        /// </summary>
        /// <value>
        /// The accessed count.
        /// </value>
        public uint AccessedCount { get; set; }

        /// <summary>
        /// Gets the address ranges.
        /// </summary>
        /// <value>
        /// The address ranges.
        /// </value>
        public IEnumerable<AddressRange> AddressRanges {
            get
            {
                yield return _addressRange0;
                yield return _addressRange1;
            }
        }

        /// <summary>
        /// Checks if the specified range intersects this cached instruction block.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public bool Intersects(AddressRange range) => range.Intersects(_addressRange0) || range.Intersects(_addressRange1);
    }
}