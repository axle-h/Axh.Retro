using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Timer = System.Timers.Timer;

namespace Axh.Retro.CPU.Z80.Timing
{
    /// <summary>
    /// An instruction timer based on machine cycles.
    /// </summary>
    public class MachineCycleTimer : IInstructionTimer
    {
        /// <summary>
        /// The number of millisends to wait before syncing real-time to CPU time.
        /// Sync up every 16.6ms. I.e. will run sync a 60 fps device e.g. GameBoy well.
        /// This uses <see cref="Thread.Sleep(TimeSpan)"/> for increased accuracy over <see cref="Task.Delay(TimeSpan)"/>.
        /// </summary>
        private const double MillisecondsPerSync = 1000 / 60.0;

        /// <summary>
        /// The number of cycles to wait between calling <see cref="TimingSync"/>.
        /// TODO: move to config. E.g. the GameBoy needs this to be ~80 machine cycles, may be higher/lower on other systems.
        /// </summary>
        private const int CyclesPerSyncEvent = 80;

        private readonly double _machineCycleFrequencyHz;

        private readonly Timer _syncTimer;
        private readonly int _cyclesPerSync;
        
        private readonly double _ticksPerCycle;

        private int _cyclesSinceLastSync;
        private int _cyclesSinceLastEventSync;
        

        private int _syncCycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineCycleTimer"/> class.
        /// </summary>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public MachineCycleTimer(IPlatformConfig platformConfig)
        {
            _machineCycleFrequencyHz = platformConfig.MachineCycleFrequencyMhz / 1000000;

            switch (platformConfig.InstructionTimingSyncMode)
            {
                case InstructionTimingSyncMode.Null:
                    // Run ASAP
                    _ticksPerCycle = 0;
                    _cyclesPerSync = 0;
                    break;
                case InstructionTimingSyncMode.MachineCycles:
                    _ticksPerCycle = 1 / platformConfig.MachineCycleFrequencyMhz * 10;
                    _cyclesPerSync = (int) Math.Ceiling(MillisecondsPerSync / (_ticksPerCycle / TimeSpan.TicksPerMillisecond));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _syncTimer = new Timer(MillisecondsPerSync);
            _syncTimer.Elapsed += (sender, args) =>
                                  {
                                      // If the CPU is running faster than real time then we must wait.
                                      // Otherwise do nothing.
                                      var extraCycles = _cyclesSinceLastSync - _cyclesPerSync;
                                      if (extraCycles > 0)
                                      {
                                          // TODO: Why does only waiting for ~40% of the extra cycles give a better result? Is it timer accuracy? Or my maths? If it's the timer will it always be 40%?!
                                          Interlocked.Add(ref _syncCycles, (int) (extraCycles * 0.4));
                                      }

                                      _cyclesSinceLastSync = 0;
                                  };
            _syncTimer.Start();
        }

        /// <summary>
        /// Uses the configured instruction timings to sync real time to the CPU.
        /// Always syncs timings immediately.
        /// </summary>
        /// <param name="timings">The timings.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public async Task SyncToTimingsNow(InstructionTimings timings) => await Task.Delay(new TimeSpan((long) _ticksPerCycle * timings.MachineCycles));

        /// <summary>
        /// Uses the configured instruction timings to sync real time to the CPU.
        /// </summary>
        /// <param name="timings">The timings.</param>
        /// <returns></returns>
        public async Task SyncToTimings(InstructionTimings timings)
        {
            _cyclesSinceLastSync += timings.MachineCycles;
 
            if (_syncCycles > 0)
            {
                var syncCycles = _syncCycles;
                Interlocked.Exchange(ref _syncCycles, 0);
                await Task.Delay(new TimeSpan((long) (_ticksPerCycle * syncCycles)));
            }
            
            // Check if we need to call the sync event.
            _cyclesSinceLastEventSync += timings.MachineCycles;
            if (_cyclesSinceLastEventSync > CyclesPerSyncEvent)
            {
                TimingSync?.Invoke(new InstructionTimings(_cyclesSinceLastEventSync));
                _cyclesSinceLastEventSync = 0;
            }
        }
        
        /// <summary>
        /// Occurs when [timing synchronize].
        /// </summary>
        public event Action<InstructionTimings> TimingSync;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => _syncTimer.Dispose();
    }
}