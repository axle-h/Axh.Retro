using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Core
{
    /// <summary>
    /// The interrupt manager.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Core.IInterruptManager" />
    public class InterruptManager : IInterruptManager
    {
        private readonly BlockingCollection<ushort> _interruptQueue;
        private readonly IRegisters _registers;
        private readonly object _disposingContext = new object();
        private bool _disposed, _disposing;

        private TaskCompletionSource<bool> _haltTaskSource;
        private Task<ushort> _interruptTask;

        private TaskCompletionSource<ushort> _interruptTaskSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptManager"/> class.
        /// </summary>
        /// <param name="registers">The registers.</param>
        public InterruptManager(IRegisters registers)
        {
            _registers = registers;
            _haltTaskSource = new TaskCompletionSource<bool>();
            _interruptQueue = new BlockingCollection<ushort>();
            Task.Factory.StartNew(InterruptTask, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Gets a value indicating whether [interrupts enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [interrupts enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool InterruptsEnabled => _registers.InterruptFlipFlop1;

        /// <summary>
        /// Interrupts the CPU, pushing all registers to the stack and setting the program counter to the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        public void Interrupt(ushort address) => _interruptQueue.TryAdd(address);

        /// <summary>
        /// Halts the CPU.
        /// </summary>
        public void Halt()
        {
            _interruptTaskSource = new TaskCompletionSource<ushort>();
            IsHalted = true;
            _interruptTask = _interruptTaskSource.Task;
        }

        /// <summary>
        /// Resumes the CPU from a halt.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">CPU must be halted first.</exception>
        public void Resume()
        {
            if (!IsHalted)
            {
                throw new InvalidOperationException("CPU must be halted first.");
            }

            // Interrupt back to program counter...
            Interrupt(_registers.ProgramCounter);
        }

        /// <summary>
        /// Gets a value indicating whether the CPU is halted.
        /// </summary>
        /// <value>
        /// <c>true</c> if the CPU is halted; otherwise, <c>false</c>.
        /// </value>
        public bool IsHalted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the CPU is interrupted.
        /// </summary>
        /// <value>
        /// <c>true</c> if the CPU is interrupted; otherwise, <c>false</c>.
        /// </value>
        public bool IsInterrupted { get; private set; }

        /// <summary>
        /// Adds a task to run when the CPU is resumed from a halt state.
        /// </summary>
        /// <param name="task">The task.</param>
        public void AddResumeTask(Action task)
        {
            _interruptTask = _interruptTask.ContinueWith(x =>
                                                         {
                                                             task();
                                                             return x.Result;
                                                         });
        }

        /// <summary>
        /// Notifies this interrupt manager that the CPU has accepted the halt and is halted.
        /// </summary>
        public void NotifyHalt() => Task.Run(() => _haltTaskSource.TrySetResult(true));

        /// <summary>
        /// Notifies this interrupt manager that the CPU has accepted the interrupt and is running again.
        /// </summary>
        public void NotifyResume() => IsHalted = false;

        /// <summary>
        /// Gets a task that will wait for next interrupt.
        /// </summary>
        /// <returns></returns>
        public async Task<ushort> WaitForNextInterrupt() => await _interruptTask.ConfigureAwait(false);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed || _disposing)
            {
                return;
            }

            lock (_disposingContext)
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

        /// <summary>
        /// The interrupt background task.
        /// </summary>
        /// <returns></returns>
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
                var task = Task.Run(() => _interruptTaskSource.TrySetResult(address));
                IsInterrupted = false;
            }
        }
    }
}