namespace Axh.Retro.CPU.X80.Contracts.Core
{
    using Axh.Retro.CPU.X80.Contracts.Core.Timing;
    using Axh.Retro.CPU.X80.Contracts.IO;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public interface IInstructionBlock<in TRegisters> where TRegisters : IRegisters
    {
        ushort Address { get; }

        ushort Length { get; }

        InstructionTimings ExecuteInstructionBlock(TRegisters registers, IMmu mmu, IArithmeticLogicUnit alu, IInputOutputManager inputOutputManager);
    }
}