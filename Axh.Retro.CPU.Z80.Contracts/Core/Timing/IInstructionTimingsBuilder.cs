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

        IInstructionTimingsBuilder MmuByte();

        IInstructionTimingsBuilder Index(bool isDisplaced);

        IInstructionTimingsBuilder IndexAndMmuByte(bool isDisplaced);

        IInstructionTimingsBuilder MmuWord();

        IInstructionTimingsBuilder IndexAndMmuWord();

        IInstructionTimingsBuilder Extend(int tStates);

        IInstructionTimingsBuilder Arithmetic16();

        IInstructionTimingsBuilder AutoCopy();
    }
}