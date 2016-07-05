using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Cache;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Core
{
    public class CoreContext<TRegisters, TRegisterState> : ICoreContext<TRegisters, TRegisterState>
        where TRegisters : IStateBackedRegisters<TRegisterState> where TRegisterState : struct
    {
        public CoreContext(TRegisters registers,
            IInterruptManager interruptManager,
            IPeripheralManager peripheralManager,
            IMmu mmu,
            IInstructionTimer instructionTimer,
            IAlu alu,
            IInstructionBlockCache<TRegisters> instructionBlockCache,
            IInstructionBlockDecoder<TRegisters> instructionBlockDecoder,
            IDmaController dmaController)
        {
            Registers = registers;
            InterruptManager = interruptManager;
            PeripheralManager = peripheralManager;
            Mmu = mmu;
            InstructionTimer = instructionTimer;
            Alu = alu;
            InstructionBlockCache = instructionBlockCache;
            InstructionBlockDecoder = instructionBlockDecoder;
            DmaController = dmaController;
        }

        public IDmaController DmaController { get; }

        public TRegisters Registers { get; }

        public IInterruptManager InterruptManager { get; }

        public IPeripheralManager PeripheralManager { get; }

        public IMmu Mmu { get; }

        public IInstructionTimer InstructionTimer { get; }

        public IAlu Alu { get; }

        public IInstructionBlockCache<TRegisters> InstructionBlockCache { get; }

        public IInstructionBlockDecoder<TRegisters> InstructionBlockDecoder { get; }

        public void Dispose()
        {
            InterruptManager.Dispose();
            PeripheralManager.Dispose();
            Mmu.Dispose();
            DmaController.Dispose();
            InstructionBlockCache.Dispose();
        }
    }
}