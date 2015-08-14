namespace Axh.Retro.CPU.X80.Contracts.Core
{
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public interface IInstructionBlockDecoder<TRegisters> where TRegisters : IRegisters
    {
        InstructionBlock<TRegisters> DecodeNextBlock(ushort address);
    }
}