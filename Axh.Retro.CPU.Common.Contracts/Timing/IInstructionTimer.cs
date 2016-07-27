using System;
using System.Threading.Tasks;

namespace Axh.Retro.CPU.Common.Contracts.Timing
{
    /// <summary>
    /// An instruction timer.
    /// </summary>
    public interface IInstructionTimer : IDisposable
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
        Task SyncToTimingsNow(InstructionTimings timings);

        /// <summary>
        /// Uses the configured instruction timings to sync real time to the CPU.
        /// </summary>
        /// <param name="timings">The timings.</param>
        /// <returns></returns>
        Task SyncToTimings(InstructionTimings timings);

        /// <summary>
        /// Notifies the instruction timer that the CPU has accepted the halt and is halted.
        /// I.e. we'll need to stop waiting for instruction blocks to trigger timing sync events and generate our own fake ones.
        /// </summary>
        void NotifyHalt();

        /// <summary>
        /// Notifies the instruction timer that the CPU has accepted the interrupt and is running again.
        /// I.e. we'll need to stop generating fake timing sync events.
        /// </summary>
        void NotifyResume();
    }
}