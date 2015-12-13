namespace Axh.Retro.CPU.Z80.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public class CachingCpuCore<TRegisters, TRegisterState> : ICpuCore<TRegisters, TRegisterState>
        where TRegisters : IStateBackedRegisters<TRegisterState>
        where TRegisterState : struct
    {
        public CachingCpuCore(ICoreContext<TRegisters, TRegisterState> context)
        {
            this.Context = context;
        }

        public ICoreContext<TRegisters, TRegisterState> Context { get; }

        public async Task StartCoreProcessAsync(CancellationToken cancellationToken)
        {
            var interruptManager = Context.InterruptManager as ICoreInterruptManager;
            if (interruptManager == null)
            {
                throw new ArgumentException("interruptManager");
            }

            var timer = Context.InstructionTimer as ICoreInstructionTimer;
            if (timer == null)
            {
                throw new ArgumentException("timer");
            }

            // Flatten the context so we aren't calling the properties in the core.
            var registers = Context.Registers;
            var peripherals = Context.PeripheralManager;
            var mmu = Context.Mmu;
            var alu = Context.Alu;
            var cache = Context.InstructionBlockCache;
            var instructionBlockDecoder = Context.InstructionBlockDecoder;

            if (!instructionBlockDecoder.SupportsInstructionBlockCaching)
            {
                throw new Exception("Instruction block decoder must support caching");
            }

            // Register the invalidate cache event with mmu AddressWrite event
            mmu.AddressWrite += (sender, args) => cache.InvalidateCache(args.Address, args.Length);
            ushort? interruptAddress = null;

            while (!cancellationToken.IsCancellationRequested)
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
                    // Did we request an interrupt or run a HALT opcode.
                    if (interruptManager.IsInterrupted || instructionBlock.HaltCpu)
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
