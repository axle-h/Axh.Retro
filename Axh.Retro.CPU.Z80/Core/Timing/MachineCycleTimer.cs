﻿namespace Axh.Retro.CPU.Z80.Core.Timing
{
    using System;
    using System.Threading.Tasks;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;

    public class MachineCycleTimer : ICoreInstructionTimer
    {
        private readonly InstructionTimingSyncMode syncMode;
        private readonly double syncMagnitude;

        public MachineCycleTimer(IPlatformConfig platformConfig)
        {
            this.syncMode = platformConfig.InstructionTimingSyncMode;

            switch (syncMode)
            {
                case InstructionTimingSyncMode.Null:
                    // Run ASAP
                    syncMagnitude = 0;
                    break;
                case InstructionTimingSyncMode.MachineCycles:
                    syncMagnitude = 1 / platformConfig.MachineCycleSpeedMhz / 1000000;
                    break;
                case InstructionTimingSyncMode.ThrottlingStates:
                    // Throttling state clock runs 1/4 of machine cycles
                    syncMagnitude = 1 / platformConfig.MachineCycleSpeedMhz / 1000000 / 4;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Task SyncToTimings(InstructionTimings timings)
        {
            TimingSync?.Invoke(this, new TimingSyncEventArgs(timings));

            switch (syncMode)
            {
                case InstructionTimingSyncMode.Null:
                    return Task.CompletedTask;
                case InstructionTimingSyncMode.MachineCycles:
                    return Task.Delay(TimeSpan.FromSeconds(syncMagnitude * timings.MachineCycles));
                case InstructionTimingSyncMode.ThrottlingStates:
                    return Task.Delay(TimeSpan.FromSeconds(syncMagnitude * timings.ThrottlingStates));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event EventHandler<TimingSyncEventArgs> TimingSync;
    }
}
