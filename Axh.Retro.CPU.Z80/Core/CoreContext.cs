namespace Axh.Retro.CPU.Z80.Core
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public class CoreContext<TRegisters, TRegisterState> : ICoreContext<TRegisters, TRegisterState>
        where TRegisters : IStateBackedRegisters<TRegisterState>
        where TRegisterState : struct
    {
        public CoreContext(TRegisters registers, IInterruptManager interruptManager, IPeripheralManager peripheralManager, IMmu mmu, IInstructionTimer instructionTimer, IAlu alu, IPrefetchQueue prefetchQueue, IInstructionBlockCache<TRegisters> instructionBlockCache)
        {
            Registers = registers;
            InterruptManager = interruptManager;
            PeripheralManager = peripheralManager;
            Mmu = mmu;
            InstructionTimer = instructionTimer;
            Alu = alu;
            PrefetchQueue = prefetchQueue;
            InstructionBlockCache = instructionBlockCache;
        }

        public TRegisters Registers { get; }

        public IInterruptManager InterruptManager { get; }

        public IPeripheralManager PeripheralManager { get; }

        public IMmu Mmu { get; }

        public IInstructionTimer InstructionTimer { get; }

        public IAlu Alu { get; }

        public IPrefetchQueue PrefetchQueue { get; }

        public IInstructionBlockCache<TRegisters> InstructionBlockCache { get; }
    }
}
