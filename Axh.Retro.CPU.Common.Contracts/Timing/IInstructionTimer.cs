using System;
using System.Threading.Tasks;

namespace Axh.Retro.CPU.Common.Contracts.Timing
{
    /// <summary>
    /// An instruction timer.
    /// </summary>
    public interface IInstructionTimer
    {
        /// <summary>
        /// Occurs when [timing synchronize].
        /// </summary>
        event Action<InstructionTimings> TimingSync;

        /// <summary>
        /// Uses the configured instruction timings to sync real time to the CPU.
        /// </summary>
        /// <param name="timings">The timings.</param>
        /// <returns></returns>
        Task SyncToTimings(InstructionTimings timings);
    }
}