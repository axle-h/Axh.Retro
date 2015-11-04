namespace Axh.Retro.CPU.X80.Core.Interpreted
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.CPU.X80.Registers;

    public class InterpretedInstructionBlockDecoder : IInstructionBlockDecoder<Z80Registers>
    {
        public bool SupportsInstructionBlockCaching => false;

        public IInstructionBlock<Z80Registers> DecodeNextBlock(ushort address, IMmu mmu)
        {
            throw new System.NotImplementedException();
        }
    }
}
