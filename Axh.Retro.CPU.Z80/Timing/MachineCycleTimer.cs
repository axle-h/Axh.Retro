using System;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Config;

namespace Axh.Retro.CPU.Z80.Timing
{
    public class MachineCycleTimer : IInstructionTimer
    {
        private readonly double _syncMagnitude;
        private readonly InstructionTimingSyncMode _syncMode;

        public MachineCycleTimer(IPlatformConfig platformConfig)
        {
            _syncMode = platformConfig.InstructionTimingSyncMode;

            switch (_syncMode)
            {
                case InstructionTimingSyncMode.Null:
                    // Run ASAP
                    _syncMagnitude = 0;
                    break;
                case InstructionTimingSyncMode.MachineCycles:
                    _syncMagnitude = 1 / platformConfig.MachineCycleSpeedMhz / 1000000;
                    break;
                case InstructionTimingSyncMode.ThrottlingStates:
                    // Throttling state clock runs 1/4 of machine cycles
                    _syncMagnitude = 1 / platformConfig.MachineCycleSpeedMhz / 1000000 / 4;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Task SyncToTimings(InstructionTimings timings)
        {
            TimingSync?.Invoke(timings);

            switch (_syncMode)
            {
                case InstructionTimingSyncMode.Null:
                    return Task.CompletedTask;
                case InstructionTimingSyncMode.MachineCycles:
                    return Task.Delay(TimeSpan.FromSeconds(_syncMagnitude * timings.MachineCycles));
                case InstructionTimingSyncMode.ThrottlingStates:
                    return Task.Delay(TimeSpan.FromSeconds(_syncMagnitude * timings.ThrottlingStates));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event Action<InstructionTimings> TimingSync;
    }
}