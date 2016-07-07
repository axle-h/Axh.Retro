using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Cache
{
    /// <summary>
    /// Instruction block cache wrapper with a single normal range.
    /// </summary>
    internal class NormalCachedInstructionBlock<TRegisters> : ICachedInstructionBlock<TRegisters> where TRegisters : IRegisters
    {
        private readonly AddressRange _addressRange;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalCachedInstructionBlock{TRegisters}"/> class.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="instructionBlock">The instruction block.</param>
        public NormalCachedInstructionBlock(AddressRange range, IInstructionBlock<TRegisters> instructionBlock)
        {
            InstructionBlock = instructionBlock;
            _addressRange = range;
        }

        /// <summary>
        /// Gets or sets the accessed count.
        /// </summary>
        /// <value>
        /// The accessed count.
        /// </value>
        public uint AccessedCount { get; set; }

        /// <summary>
        /// Gets the instruction block.
        /// </summary>
        /// <value>
        /// The instruction block.
        /// </value>
        public IInstructionBlock<TRegisters> InstructionBlock { get; }

        /// <summary>
        /// Checks if the specified range intersects this cached instruction block.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public bool Intersects(AddressRange range) => range.Intersects(_addressRange);
    }
}