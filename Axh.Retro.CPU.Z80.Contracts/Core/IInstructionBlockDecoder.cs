using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    /// <summary>
    /// Instruction block decoder.
    /// </summary>
    public interface IInstructionBlockDecoder
    {
        /// <summary>
        /// Gets a value indicating whether this instruction block decoder [supports instruction block caching].
        /// </summary>
        /// <value>
        /// <c>true</c> if this instruction block decoder [supports instruction block caching]; otherwise, <c>false</c>.
        /// </value>
        bool SupportsInstructionBlockCaching { get; }

        /// <summary>
        /// Decodes the next block.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        IInstructionBlock DecodeNextBlock(ushort address);
    }
}