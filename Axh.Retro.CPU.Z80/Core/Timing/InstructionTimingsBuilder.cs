namespace Axh.Retro.CPU.Z80.Core.Timing
{
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;

    /// <summary>
    /// This is just to clean up incrementing the cycle counters per instruction. Calls to Add 'should' be inlined by the JIT compiler.
    /// </summary>
    internal class InstructionTimingsBuilder : IInstructionTimingsBuilder
    {
        public InstructionTimingsBuilder()
        {
            Reset();
        }

        public int MachineCycles { get; private set; }
        public int ThrottlingStates { get; private set; }

        public void Add(int mCycles, int tStates)
        {
            MachineCycles += mCycles;
            ThrottlingStates += tStates;
        }

        public InstructionTimings GetInstructionTimings()
        {
            return new InstructionTimings { MachineCycles = MachineCycles, ThrottlingStates = ThrottlingStates };
        }

        public void Reset()
        {
            MachineCycles = 0;
            ThrottlingStates = 0;
        }
    }
}
