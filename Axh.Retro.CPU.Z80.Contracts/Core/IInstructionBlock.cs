namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public interface IInstructionBlock<in TRegisters> where TRegisters : IRegisters
    {
        ushort Address { get; }

        ushort Length { get; }

        bool HaltCpu { get; }

        bool HaltPeripherals { get; }

        InstructionTimings ExecuteInstructionBlock(TRegisters registers, IMmu mmu, IArithmeticLogicUnit alu, IPeripheralManager peripheralManager);
    }
}