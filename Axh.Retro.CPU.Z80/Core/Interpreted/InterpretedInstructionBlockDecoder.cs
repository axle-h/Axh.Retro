using System;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Registers;

namespace Axh.Retro.CPU.Z80.Core.Interpreted
{
    public class InterpretedInstructionBlockDecoder : IInstructionBlockDecoder<Z80Registers>
    {
        public bool SupportsInstructionBlockCaching => false;

        public IInstructionBlock<Z80Registers> DecodeNextBlock(ushort address)
        {
            throw new NotImplementedException();
        }
    }
}