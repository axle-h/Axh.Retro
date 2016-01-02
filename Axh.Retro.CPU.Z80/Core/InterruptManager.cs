namespace Axh.Retro.CPU.Z80.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public class InterruptManager : ICoreInterruptManager
    {
        private readonly IRegisters registers;

        private TaskCompletionSource<bool> haltTaskSource;

        private TaskCompletionSource<ushort> interruptTaskSource;
        private Task<ushort> interruptTask;

        private readonly object interruptSyncContext;

        public InterruptManager(IRegisters registers)
        {
            this.registers = registers;
            this.haltTaskSource = new TaskCompletionSource<bool>();
            this.IsHalted = false;
            this.IsInterrupted = false;
            this.interruptSyncContext = new object();
        }

        public bool InterruptsEnabled => this.registers.InterruptFlipFlop1 && !IsInterrupted;

        public async Task Interrupt(ushort address)
        {
            if (!this.registers.InterruptFlipFlop1)
            {
                // Interrupts disabled.
                return;
            }

            if (IsInterrupted)
            {
                // TODO: support nested interrupts
                return;
            }

            lock (this.interruptSyncContext)
            {
                if (IsInterrupted)
                {
                    // TODO: support nested interrupts
                    return;
                }
                this.IsInterrupted = true;
            }

            this.registers.InterruptFlipFlop1 = false;

            // Halt the CPU if not already halted
            if (!IsHalted)
            {
                Halt();
            }
            
            // Wait for the halt to be confirmed
            await this.haltTaskSource.Task;
            this.haltTaskSource = new TaskCompletionSource<bool>();

            // Resume the CPU with the program counter set to address
            this.interruptTaskSource.TrySetResult(address);
            this.IsInterrupted = false;
        }

        public void Halt()
        {
            this.interruptTaskSource = new TaskCompletionSource<ushort>();
            this.IsHalted = true;
            this.interruptTask = this.interruptTaskSource.Task;
        }

        public bool IsHalted { get; private set; }

        public bool IsInterrupted { get; private set; }

        public void AddResumeTask(Action task)
        {
            this.interruptTask = this.interruptTask.ContinueWith(
                        x =>
                        {
                            task();
                            return x.Result;
                        });
        }

        public void NotifyHalt()
        {
            this.haltTaskSource.TrySetResult(true);
        }

        public void NotifyResume()
        {
            this.IsHalted = false;
        }

        public async Task<ushort> WaitForNextInterrupt()
        {
            return await this.interruptTask;
        }
    }
}
