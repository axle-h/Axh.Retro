namespace Axh.Retro.CPU.Z80.Contracts.Core.Timing
{
    public interface IInstructionTimingsBuilder
    {
        int MachineCycles { get; }

        int ThrottlingStates { get; }

        void Add(int mCycles, int tStates);

        InstructionTimings GetInstructionTimings();
    }
}