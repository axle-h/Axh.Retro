namespace Axh.Retro.CPU.X80.Contracts.Core
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public interface IInstructionBlockDecoder<in TRegisters> where TRegisters : IRegisters
    {
        bool SupportsInstructionBlockCaching { get; }

        IInstructionBlock<TRegisters> DecodeNextBlock(ushort address, IMmu mmu);
    }
}