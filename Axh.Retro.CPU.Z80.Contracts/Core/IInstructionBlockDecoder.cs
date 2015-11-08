namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public interface IInstructionBlockDecoder<in TRegisters> where TRegisters : IRegisters
    {
        bool SupportsInstructionBlockCaching { get; }

        IInstructionBlock<TRegisters> DecodeNextBlock(ushort address);
    }
}