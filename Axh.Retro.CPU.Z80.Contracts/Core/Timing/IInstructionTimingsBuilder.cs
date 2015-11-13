namespace Axh.Retro.CPU.Z80.Contracts.Core.Timing
{
    public interface IInstructionTimingsBuilder
    {
        int MachineCycles { get; }

        int ThrottlingStates { get; }

        void Add(int mCycles, int tStates);

        InstructionTimings GetInstructionTimings();

        void Reset();

        IInstructionTimingsBuilder Nop();

        IInstructionTimingsBuilder Prefetch();

        IInstructionTimingsBuilder Index(bool isDisplaced);

        IInstructionTimingsBuilder IndexAndPrefetch(bool isDisplaced);
    }
}