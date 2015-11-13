namespace Axh.Retro.CPU.Z80.Core.Timing
{
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;

    /// <summary>
    /// This is just to clean up incrementing the cycle counters per instruction. Calls to Add 'should' be inlined by the JIT compiler.
    /// </summary>
    internal class InstructionTimingsBuilder : IInstructionTimingsBuilder
    {
        private const int NopMachineCycles = 1;
        private const int NopThrottlingStates = 4;

        private const int PrefetchMachineCycles = 1;
        private const int PretetchThrottlingStates = 3;

        private const int IndexMachineCycles = 2;
        private const int IndexThrottlingStates = 7;

        private const int DisplaceedIndexMachineCycles = 4;
        private const int DisplaceedIndexThrottlingStates = 15;

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

        public IInstructionTimingsBuilder Nop()
        {
            MachineCycles += NopMachineCycles;
            ThrottlingStates += NopThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder Prefetch()
        {
            MachineCycles += PrefetchMachineCycles;
            ThrottlingStates += PretetchThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder Index(bool isDisplaced)
        {
            MachineCycles += isDisplaced ? DisplaceedIndexMachineCycles : IndexMachineCycles;
            ThrottlingStates += isDisplaced ? DisplaceedIndexThrottlingStates : IndexThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder IndexAndPrefetch(bool isDisplaced)
        {
            // Only add on prefetch timings when not an indexed register
            MachineCycles += isDisplaced ? DisplaceedIndexMachineCycles : IndexMachineCycles + PrefetchMachineCycles;
            ThrottlingStates += isDisplaced ? DisplaceedIndexThrottlingStates : IndexThrottlingStates + PretetchThrottlingStates;
            return this;
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
