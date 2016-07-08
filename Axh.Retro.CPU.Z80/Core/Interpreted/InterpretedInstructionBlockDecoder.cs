using System;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Registers;

namespace Axh.Retro.CPU.Z80.Core.Interpreted
{
    /// <summary>
    /// Simple instruction block decoder.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Core.IInstructionBlockDecoder{Axh.Retro.CPU.Z80.Registers.Z80Registers}" />
    public class InterpretedInstructionBlockDecoder : IInstructionBlockDecoder<Z80Registers>
    {
        /// <summary>
        /// Gets a value indicating whether this instruction block decoder [supports instruction block caching].
        /// </summary>
        /// <value>
        /// <c>true</c> if this instruction block decoder [supports instruction block caching]; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsInstructionBlockCaching => false;

        /// <summary>
        /// Decodes the next block.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IInstructionBlock<Z80Registers> DecodeNextBlock(ushort address)
        {
            throw new NotImplementedException();
        }
    }
}