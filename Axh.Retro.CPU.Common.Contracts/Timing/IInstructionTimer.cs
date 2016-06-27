using System;
using System.Threading.Tasks;

namespace Axh.Retro.CPU.Common.Contracts.Timing
{
    public interface IInstructionTimer
    {
        event Action<InstructionTimings> TimingSync;

        Task SyncToTimings(InstructionTimings timings);
    }
}