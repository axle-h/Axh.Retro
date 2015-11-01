namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Threading.Tasks;
    
    using Axh.Retro.CPU.X80.Contracts.Cache;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Core.Timing;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.IO;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public class CachingCpuCore<TRegisters> : ICpuCore where TRegisters : IRegisters
    {
        private readonly IRegisterFactory<TRegisters> registerFactory;

        private readonly IMmuFactory mmuFactory;

        private readonly IAluFactory aluFactory;

        private readonly IInstructionBlockDecoder<TRegisters> instructionBlockDecoder;

        private readonly IInstructionBlockCache<TRegisters> instructionBlockCache;

        private readonly IInputOutputManager inputOutputManager;

        private readonly IInstructionTimer instructionTimer;

        private TaskCompletionSource<bool> haltTaskSource;
        private bool halt;

        private TaskCompletionSource<ushort> interruptTaskSource;
        private Task<ushort> interruptTask;
        private bool interrupted;

        private readonly object interruptSyncContext;

        public CachingCpuCore(
            IRegisterFactory<TRegisters> registerFactory,
            IMmuFactory mmuFactory,
            IInstructionBlockDecoder<TRegisters> instructionBlockDecoder,
            IAluFactory aluFactory,
            IInstructionBlockCache<TRegisters> instructionBlockCache,
            IInputOutputManager inputOutputManager,
            IInstructionTimer instructionTimer)
        {
            this.registerFactory = registerFactory;
            this.mmuFactory = mmuFactory;
            this.instructionBlockDecoder = instructionBlockDecoder;
            this.aluFactory = aluFactory;
            this.instructionBlockCache = instructionBlockCache;
            this.inputOutputManager = inputOutputManager;
            this.instructionTimer = instructionTimer;

            if (!this.instructionBlockDecoder.SupportsInstructionBlockCaching)
            {
                throw new Exception("Instruction block decoder must support caching");
            }
            
            this.haltTaskSource = new TaskCompletionSource<bool>();
            this.halt = false;
            this.interrupted = false;
            this.interruptSyncContext = new object();
        }

        public async Task StartCoreProcessAsync()
        {
            // Build components
            var registers = registerFactory.GetInitialRegisters();
            var mmu = mmuFactory.GetMmu();
            var alu = this.aluFactory.GetArithmeticLogicUnit(registers.AccumulatorAndFlagsRegisters.Flags);

            // Register the invalidate cache event with mmu AddressWrite event
            mmu.AddressWrite += (sender, args) => this.instructionBlockCache.InvalidateCache(args.Address, args.Length);
            ushort? interruptAddress = null;

            while (true)
            {
                var address = interruptAddress ?? registers.ProgramCounter;
                var instructionBlock = this.instructionBlockCache.GetOrSet(address, () => this.instructionBlockDecoder.DecodeNextBlock(address, mmu));
                var timings = instructionBlock.ExecuteInstructionBlock(registers, mmu, alu, this.inputOutputManager);

                if (instructionBlock.HaltCpu)
                {
                    Halt(instructionBlock.HaltPeripherals);
                }

                if (this.halt)
                {
                    if (registers.InterruptFlipFlop1)
                    {
                        // Notify halt success before halting
                        this.haltTaskSource.TrySetResult(true);
                        interruptAddress = await this.interruptTask;

                        // Push the program counter onto the stack
                        registers.StackPointer = unchecked((ushort)(registers.StackPointer - 2));
                        mmu.WriteWord(registers.StackPointer, registers.ProgramCounter);
                    }
                    else
                    {
                        // Dummy halt so we don't block threads trigerring interrupts when disabled.
                        this.haltTaskSource.TrySetResult(true);
                    }
                    this.halt = false;
                }
                else
                {
                    interruptAddress = null;
                    await this.instructionTimer.SyncToTimings(timings);
                }
            }
        }

        private void Halt(bool haltPeripherals)
        {
            this.interruptTaskSource = new TaskCompletionSource<ushort>();
            this.halt = true;
            
            if (haltPeripherals)
            {
                this.inputOutputManager.Halt();
                this.interruptTask = this.interruptTaskSource.Task.ContinueWith(
                    x =>
                    {
                        this.inputOutputManager.Resume();
                        return x.Result;
                    });
            }
            else
            {
                this.interruptTask = this.interruptTaskSource.Task;
            }
        }

        public async Task Interrupt(ushort address)
        {
            if (interrupted)
            {
                // TODO: don't ignore these interrupts, interrupts trigerreed at the same time should be chosen by priority
                return;
            }

            lock (this.interruptSyncContext)
            {
                if (interrupted)
                {
                    return;
                }
                this.interrupted = true;
            }

            // Halt the CPU if not already halted
            if (!halt)
            {
                Halt(false);
            }

            // Wait for the halt to be confirmed
            await this.haltTaskSource.Task;
            this.haltTaskSource = new TaskCompletionSource<bool>();

            // Resume the CPU with the program counter set to address
            this.interruptTaskSource.TrySetResult(address);
            this.interrupted = false;
        }
    }
}
