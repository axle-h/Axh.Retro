namespace Axh.Retro.CPU.X80.Contracts.Core
{
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public interface IInstructionBlockDecoder<in TRegisters> where TRegisters : IRegisters
    {
        bool SupportsInstructionBlockCaching { get; }

        IInstructionBlock<TRegisters> DecodeNextBlock(IMmu mmu, ushort address);
    }
}