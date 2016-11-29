﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Common.Timing;
using Axh.Retro.CPU.Z80.Contracts.Config;

namespace Axh.Retro.CPU.Z80.Timing
{
    /// <summary>
    /// An instruction timer based on machine cycles.
    /// </summary>
    public class InstructionTimer : IInstructionTimer
    {
        /// <summary>
        /// The number of cycles to wait between calling <see cref="TimingSync"/>.
        /// TODO: move to config. E.g. the GameBoy needs this to be ~80 machine cycles, may be higher/lower on other systems.
        /// </summary>
        private const int CyclesPerSyncEvent = 80;
        
        private readonly double _ticksPerCycle;

        private readonly HighPrecisionTimer _timer;

        private int _cyclesSinceLastEventSync;

        private bool _isHalted;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionTimer"/> class.
        /// </summary>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public InstructionTimer(IPlatformConfig platformConfig)
        {
            switch (platformConfig.InstructionTimingSyncMode)
            {
                case InstructionTimingSyncMode.Null:
                    // Run ASAP
                    _ticksPerCycle = 0;
                    break;
                case InstructionTimingSyncMode.MachineCycles:
                    _ticksPerCycle = 1 / platformConfig.MachineCycleFrequencyMhz * 10 / 3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _timer = new HighPrecisionTimer();
        }

        /// <summary>
        /// Uses the configured instruction timings to sync real time to the CPU.
        /// </summary>
        /// <param name="timings">The timings.</param>
        /// <param name="backgroundSync">if set to <c>true</c> [background synchronize].</param>
        public void SyncToTimings(InstructionTimings timings, bool backgroundSync = false)
        {
            if (backgroundSync)
            {
                _timer.BlockFor((long) (_ticksPerCycle * timings.MachineCycles));
            }
            else
            {
                // Check if we need to call the sync event.
                _cyclesSinceLastEventSync += timings.MachineCycles;
                if (_cyclesSinceLastEventSync > CyclesPerSyncEvent)
                {
                    TimingSync?.Invoke(new InstructionTimings(_cyclesSinceLastEventSync));
                    _timer.Block((long)(_ticksPerCycle * _cyclesSinceLastEventSync));
                    _cyclesSinceLastEventSync = _cyclesSinceLastEventSync - CyclesPerSyncEvent;
                }
            }
        }

        /// <summary>
        /// Notifies the instruction timer that the CPU has accepted the halt and is halted.
        /// I.e. we'll need to stop waiting for instruction blocks to trigger timing sync events and generate our own fake ones.
        /// </summary>
        public void NotifyHalt()
        {
            _isHalted = true;

            Task.Run(() =>
                     {
                         while (_isHalted)
                         {
                             _timer.BlockFor((long)(_ticksPerCycle * CyclesPerSyncEvent));
                             TimingSync?.Invoke(new InstructionTimings(CyclesPerSyncEvent));
                         }
                     });

        }

        /// <summary>
        /// Notifies the instruction timer that the CPU has accepted the interrupt and is running again.
        /// I.e. we'll need to stop generating fake timing sync events.
        /// </summary>
        public void NotifyResume()
        {
            _isHalted = false;
        }

        /// <summary>
        /// Occurs when [timing synchronize].
        /// </summary>
        public event Action<InstructionTimings> TimingSync;
        
    }
}