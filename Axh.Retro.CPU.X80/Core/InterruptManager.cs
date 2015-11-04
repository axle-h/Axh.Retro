namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Threading.Tasks;

    using Axh.Retro.CPU.X80.Contracts.Core;

    public class InterruptManager : ICoreInterruptManager
    {
        private TaskCompletionSource<bool> haltTaskSource;

        private TaskCompletionSource<ushort> interruptTaskSource;
        private Task<ushort> interruptTask;
        private bool interrupted;

        private readonly object interruptSyncContext;

        public InterruptManager()
        {
            this.haltTaskSource = new TaskCompletionSource<bool>();
            this.IsHalted = false;
            this.interrupted = false;
            this.interruptSyncContext = new object();
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
            if (!IsHalted)
            {
                Halt();
            }

            // Wait for the halt to be confirmed
            await this.haltTaskSource.Task;
            this.haltTaskSource = new TaskCompletionSource<bool>();

            // Resume the CPU with the program counter set to address
            this.interruptTaskSource.TrySetResult(address);
            this.interrupted = false;
        }

        public void Halt()
        {
            this.interruptTaskSource = new TaskCompletionSource<ushort>();
            this.IsHalted = true;
            this.interruptTask = this.interruptTaskSource.Task;
        }

        public bool IsHalted { get; private set; }

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
