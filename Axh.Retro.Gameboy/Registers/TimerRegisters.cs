﻿using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    /// <summary>
    /// Timer registers.
    /// </summary>
    public class TimerRegisters : ITimerRegisters
    {
        private readonly IInterruptFlagsRegister _interruptFlagsRegister;

        private int _cyclesSinceLastIncrement;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerRegisters"/> class.
        /// </summary>
        /// <param name="timerControlRegister">The timer control register.</param>
        /// <param name="interruptFlagsRegister">The interrupt flags register.</param>
        /// <param name="instructionTimer">The instruction timer.</param>
        public TimerRegisters(ITimerControlRegister timerControlRegister, IInterruptFlagsRegister interruptFlagsRegister, IInstructionTimer instructionTimer)
        {
            TimerControlRegister = timerControlRegister;
            _interruptFlagsRegister = interruptFlagsRegister;
            TimerModuloRegister = new SimpleRegister(0xff06, "Timer Modulo (TMA R/W)");
            TimerCounterRegister = new SimpleRegister(0xff05, "Timer counter (TIMA R/W)");
            instructionTimer.TimingSync += timings =>
                                           {
                                               if (!TimerControlRegister.TimerEnabled)
                                               {
                                                   return;
                                               }

                                               _cyclesSinceLastIncrement += timings.MachineCycles;
                                               if (_cyclesSinceLastIncrement > TimerControlRegister.TimerFrequency)
                                               {
                                                   if (TimerCounterRegister.Register == 0xff)
                                                   {
                                                       // Overflow.
                                                       TimerCounterRegister.Register = TimerModuloRegister.Register;
                                                       _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.TimerOverflow);
                                                   }
                                                   else
                                                   {
                                                       TimerCounterRegister.Register++;
                                                   }
                                               }

                                               _cyclesSinceLastIncrement -= TimerControlRegister.TimerFrequency;
                                           };
        }

        /// <summary>
        /// Gets the timer control register.
        /// </summary>
        /// <value>
        /// The timer control register.
        /// </value>
        public ITimerControlRegister TimerControlRegister { get; }

        /// <summary>
        /// Gets the timer modulo register.
        /// </summary>
        /// <value>
        /// The timer modulo register.
        /// </value>
        public IRegister TimerModuloRegister { get; }

        /// <summary>
        /// Gets the timer counter register.
        /// </summary>
        /// <value>
        /// The timer counter register.
        /// </value>
        public IRegister TimerCounterRegister { get; }
    }
}
