namespace Axh.Retro.CPU.Z80.Contracts.Core.Timing
{
    using System;
    using System.Threading.Tasks;

    public interface ICoreInstructionTimer : IInstructionTimer
    {
        Task SyncToTimings(InstructionTimings timings);
    }

    public interface IInstructionTimer
    {
        event EventHandler<TimingSyncEventArgs> TimingSync;
    }
}