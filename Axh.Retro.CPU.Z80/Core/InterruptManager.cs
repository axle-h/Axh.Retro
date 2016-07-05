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
        private readonly BlockingCollection<ushort> _interruptQueue;
        private readonly IRegisters _registers;
        private readonly object disposingContext = new object();
        private bool _disposed, _disposing;

        private TaskCompletionSource<bool> _haltTaskSource;
        private Task<ushort> _interruptTask;

        private TaskCompletionSource<ushort> _interruptTaskSource;

        public InterruptManager(IRegisters registers)
        {
            _registers = registers;
            _haltTaskSource = new TaskCompletionSource<bool>();
            _interruptQueue = new BlockingCollection<ushort>();
            Task.Factory.StartNew(InterruptTask, TaskCreationOptions.LongRunning);
        }


        public bool InterruptsEnabled => _registers.InterruptFlipFlop1;

        public void Interrupt(ushort address) => _interruptQueue.TryAdd(address);

        public void Halt()
        {
            _interruptTaskSource = new TaskCompletionSource<ushort>();
            IsHalted = true;
            _interruptTask = _interruptTaskSource.Task;
        }

        public bool IsHalted { get; private set; }

        public bool IsInterrupted { get; private set; }

        public void AddResumeTask(Action task)
        {
            _interruptTask = _interruptTask.ContinueWith(x =>
            {
                task();
                return x.Result;
            });
        }

        public void NotifyHalt() => Task.Run(() => _haltTaskSource.TrySetResult(true));

        public void NotifyResume() => IsHalted = false;

        public async Task<ushort> WaitForNextInterrupt() => await _interruptTask.ConfigureAwait(false);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed || _disposing)
            {
                return;
            }

            lock (disposingContext)
            {
                if (_disposed || _disposing)
                {
                    return;
                }

                _disposing = true;
            }

            _interruptQueue.CompleteAdding();

            var timeout = Task.Delay(1000);
            while (_interruptQueue.Any())
            {
                var iteration = Task.Delay(100);
                var completedTask = Task.WhenAny(timeout, iteration).Result;

                if (completedTask == timeout)
                {
                    throw new Exception("Cannot dispose");
                }
            }

            _interruptQueue.Dispose();
            _disposed = true;
        }

        private async Task InterruptTask()
        {
            while (!_disposing)
            {
                ushort address;
                if (!_interruptQueue.TryTake(out address, 100))
                {
                    continue;
                }

                if (!_registers.InterruptFlipFlop1)
                {
                    // Interrupts disabled. Discard this interrupt.
                    continue;
                }

                IsInterrupted = true;

                // Disable interrupts whilst we're... interrupting.
                _registers.InterruptFlipFlop1 = false;

                // Halt the CPU if not already halted
                if (!IsHalted)
                {
                    Halt();
                }

                // Wait for the halt to be confirmed
                await _haltTaskSource.Task.ConfigureAwait(false);
                _haltTaskSource = new TaskCompletionSource<bool>();

                // Resume the CPU with the program counter set to address
                Task.Run(() => _interruptTaskSource.TrySetResult(address));
                IsInterrupted = false;

                IsInterrupted = false;
            }
        }
    }
}