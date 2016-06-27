using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    public interface IInstructionBlockDecoder<in TRegisters> where TRegisters : IRegisters
    {
        bool SupportsInstructionBlockCaching { get; }

        IInstructionBlock<TRegisters> DecodeNextBlock(ushort address);
    }
}