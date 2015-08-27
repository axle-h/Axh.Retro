namespace Axh.Retro.CPU.X80.Contracts.Core
{
    public interface IInstructionTimer
    {
        int MachineCycles { get; }

        int ThrottlingStates { get; }

        void Add(int mCycles, int tStates);

        InstructionTimings GetInstructionTimings();
    }
}