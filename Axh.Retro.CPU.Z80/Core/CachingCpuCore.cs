namespace Axh.Retro.CPU.Z80.Core
{
    using System;
    using System.Threading.Tasks;
    
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public class CachingCpuCore<TRegisters, TRegisterState> : ICpuCore<TRegisters, TRegisterState>
        where TRegisters : IStateBackedRegisters<TRegisterState>
        where TRegisterState : struct
    {
        private readonly ICoreContextFactory<TRegisters, TRegisterState> coreContextFactory;

        public CachingCpuCore(ICoreContextFactory<TRegisters, TRegisterState> coreContextFactory)
        {
            this.coreContextFactory = coreContextFactory;
        }

        public ICoreContext<TRegisters, TRegisterState> GetContext()
        {
            return this.coreContextFactory.GetContext();
        }

        public async Task StartCoreProcessAsync(ICoreContext<TRegisters, TRegisterState> context)
        {
            var interruptManager = context.InterruptManager as ICoreInterruptManager;
            if (interruptManager == null)
            {
                throw new ArgumentException("interruptManager");
            }

            var timer = context.InstructionTimer as ICoreInstructionTimer;
            if (timer == null)
            {
                throw new ArgumentException("timer");
            }

            // Flatten the context so we aren't calling the properties in the core.
            var registers = context.Registers;
            var peripherals = context.PeripheralManager;
            var mmu = context.Mmu;
            var alu = context.Alu;
            var cache = context.InstructionBlockCache;
            var instructionBlockDecoder = context.InstructionBlockDecoder;

            if (!instructionBlockDecoder.SupportsInstructionBlockCaching)
            {
                throw new Exception("Instruction block decoder must support caching");
            }

            // Register the invalidate cache event with mmu AddressWrite event
            mmu.AddressWrite += (sender, args) => cache.InvalidateCache(args.Address, args.Length);
            ushort? interruptAddress = null;

            while (true)
            {
                var address = interruptAddress ?? registers.ProgramCounter;
                var instructionBlock = cache.GetOrSet(address, () => instructionBlockDecoder.DecodeNextBlock(address));
                var timings = instructionBlock.ExecuteInstructionBlock(registers, mmu, alu, peripherals);

                if (instructionBlock.HaltCpu)
                {
                    interruptManager.Halt();
                    if (instructionBlock.HaltPeripherals)
                    {
                        peripherals.Signal(ControlSignal.Halt);
                        interruptManager.AddResumeTask(() => peripherals.Signal(ControlSignal.Resume));
                    }
                }

                if (interruptManager.IsHalted)
                {
                    if (registers.InterruptFlipFlop1 || !interruptManager.IsInterrupted)
                    {
                        // Notify halt success before halting
                        interruptManager.NotifyHalt();
                        interruptAddress = await interruptManager.WaitForNextInterrupt();

                        // Push the program counter onto the stack
                        registers.StackPointer = unchecked((ushort)(registers.StackPointer - 2));
                        mmu.WriteWord(registers.StackPointer, registers.ProgramCounter);
                    }
                    else
                    {
                        // Dummy halt so we don't block threads trigerring interrupts when disabled.
                        interruptManager.NotifyHalt();
                    }
                    
                    interruptManager.NotifyResume();
                }
                else
                {
                    interruptAddress = null;
                }
                
                await timer.SyncToTimings(timings);
            }
        }
    }
}
