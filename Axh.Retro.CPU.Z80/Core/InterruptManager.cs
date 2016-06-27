using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Core
{
    public class InterruptManager : ICoreInterruptManager
    {
        private readonly object disposingContext = new object();

        private readonly BlockingCollection<ushort> interruptQueue;
        private readonly IRegisters registers;
        private bool disposed, disposing;

        private TaskCompletionSource<bool> haltTaskSource;
        private Task<ushort> interruptTask;

        private TaskCompletionSource<ushort> interruptTaskSource;

        public InterruptManager(IRegisters registers)
        {
            this.registers = registers;
            haltTaskSource = new TaskCompletionSource<bool>();
            interruptQueue = new BlockingCollection<ushort>();
            Task.Factory.StartNew(InterruptTask, TaskCreationOptions.LongRunning);
        }


        public bool InterruptsEnabled => registers.InterruptFlipFlop1;

        public void Interrupt(ushort address) => interruptQueue.TryAdd(address);

        public void Halt()
        {
            interruptTaskSource = new TaskCompletionSource<ushort>();
            IsHalted = true;
            interruptTask = interruptTaskSource.Task;
        }

        public bool IsHalted { get; private set; }

        public bool IsInterrupted { get; private set; }

        public void AddResumeTask(Action task)
        {
            interruptTask = interruptTask.ContinueWith(x =>
                                                       {
                                                           task();
                                                           return x.Result;
                                                       });
        }

        public void NotifyHalt() => Task.Run(() => haltTaskSource.TrySetResult(true));

        public void NotifyResume() => IsHalted = false;

        public async Task<ushort> WaitForNextInterrupt() => await interruptTask.ConfigureAwait(false);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed || disposing)
            {
                return;
            }

            lock (disposingContext)
            {
                if (disposed || disposing)
                {
                    return;
                }

                disposing = true;
            }

            interruptQueue.CompleteAdding();

            var timeout = Task.Delay(1000);
            while (interruptQueue.Any())
            {
                var iteration = Task.Delay(100);
                var completedTask = Task.WhenAny(timeout, iteration).Result;

                if (completedTask == timeout)
                {
                    throw new Exception("Cannot dispose");
                }
            }

            interruptQueue.Dispose();
            disposed = true;
        }

        private async Task InterruptTask()
        {
            while (!disposing)
            {
                ushort address;
                if (!interruptQueue.TryTake(out address, 100))
                {
                    continue;
                }

                if (!registers.InterruptFlipFlop1)
                {
                    // Interrupts disabled. Discard this interrupt.
                    continue;
                }

                IsInterrupted = true;

                // Disable interrupts whilst we're... interrupting.
                registers.InterruptFlipFlop1 = false;

                // Halt the CPU if not already halted
                if (!IsHalted)
                {
                    Halt();
                }

                // Wait for the halt to be confirmed
                await haltTaskSource.Task.ConfigureAwait(false);
                haltTaskSource = new TaskCompletionSource<bool>();

                // Resume the CPU with the program counter set to address
                Task.Run(() => interruptTaskSource.TrySetResult(address));
                IsInterrupted = false;

                IsInterrupted = false;
            }
        }
    }
}