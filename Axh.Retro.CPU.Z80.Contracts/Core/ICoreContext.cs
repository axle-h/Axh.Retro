namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public interface ICoreContext<TRegisters, TRegisterState>
        where TRegisters : IStateBackedRegisters<TRegisterState>
        where TRegisterState : struct
    {
        TRegisters Registers { get; }

        IInterruptManager InterruptManager { get; }

        IPeripheralManager PeripheralManager { get; }

        IMmu Mmu { get; }

        IInstructionTimer InstructionTimer { get; }

        IAlu Alu { get; }

        IPrefetchQueue PrefetchQueue { get; }

        IInstructionBlockCache<TRegisters> InstructionBlockCache { get; }
    }
}
