namespace Axh.Retro.CPU.X80.Contracts.Core.Timing
{
    using System.Threading.Tasks;

    public interface IInstructionTimer
    {
        Task SyncToTimings(InstructionTimings timings);
    }
}