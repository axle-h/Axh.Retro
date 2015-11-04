namespace Axh.Retro.CPU.Z80.Contracts.Core.Timing
{
    using System.Threading.Tasks;

    public interface IInstructionTimer
    {
        Task SyncToTimings(InstructionTimings timings);
    }
}