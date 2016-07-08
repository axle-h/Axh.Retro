using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    /// <summary>
    /// Instruction block decoder.
    /// </summary>
    /// <typeparam name="TRegisters">The type of the registers.</typeparam>
    public interface IInstructionBlockDecoder<in TRegisters> where TRegisters : IRegisters
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
        IInstructionBlock<TRegisters> DecodeNextBlock(ushort address);
    }
}