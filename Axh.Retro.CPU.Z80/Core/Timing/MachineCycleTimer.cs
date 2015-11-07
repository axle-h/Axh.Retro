namespace Axh.Retro.CPU.Z80.Core.Timing
{
    using System;
    using System.Threading.Tasks;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;

    public class MachineCycleTimer : ICoreInstructionTimer
    {
        private readonly SyncMode syncMode;
        private readonly double syncMagnitude;

        public MachineCycleTimer(IPlatformConfig platformConfig)
        {
            if (!platformConfig.MachineCycleSpeedMhz.HasValue && !platformConfig.ThrottlingStateSpeedMhz.HasValue)
            {
                // Run ASAP
                syncMode = SyncMode.Null;
                syncMagnitude = 0;
            }
            else
            {
                // Prefer syncing to t-states as should be more accurate
                if (platformConfig.ThrottlingStateSpeedMhz.HasValue)
                {
                    syncMode = SyncMode.ThrottlingStates;
                    syncMagnitude = 1 / platformConfig.ThrottlingStateSpeedMhz.Value / 1000000;
                }
                else
                {
                    syncMode = SyncMode.MachineCycles;
                    syncMagnitude = 1 / platformConfig.MachineCycleSpeedMhz.Value / 1000000;
                }
            }
        }

        public Task SyncToTimings(InstructionTimings timings)
        {
            TimingSync?.Invoke(this, new TimingSyncEventArgs(timings));

            switch (syncMode)
            {
                case SyncMode.Null:
                    return Task.CompletedTask;
                case SyncMode.MachineCycles:
                    return Task.Delay(TimeSpan.FromSeconds(syncMagnitude * timings.MachineCycles));
                case SyncMode.ThrottlingStates:
                    return Task.Delay(TimeSpan.FromSeconds(syncMagnitude * timings.ThrottlingStates));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum SyncMode
        {
            /// <summary>
            /// Do no timing sync, simulation will run as fast as possible
            /// </summary>
            Null,

            /// <summary>
            /// Sync to machine cycles
            /// </summary>
            MachineCycles,

            /// <summary>
            /// Sync to throttling states
            /// </summary>
            ThrottlingStates,
        }

        public event EventHandler<TimingSyncEventArgs> TimingSync;
    }
}
