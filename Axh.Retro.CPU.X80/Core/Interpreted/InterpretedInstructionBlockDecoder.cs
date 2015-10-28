namespace Axh.Retro.CPU.X80.Core.Interpreted
{
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Registers;

    public class InterpretedInstructionBlockDecoder : IInstructionBlockDecoder<Z80Registers>
    {
        public bool SupportsInstructionBlockCaching => false;

        public IInstructionBlock<Z80Registers> DecodeNextBlock(IMmu mmu, ushort address)
        {
            throw new System.NotImplementedException();
        }
    }
}
