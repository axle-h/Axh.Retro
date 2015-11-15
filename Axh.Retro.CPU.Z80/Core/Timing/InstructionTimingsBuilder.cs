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

        private const int PrefetchWordMachineCycles = 2;
        private const int PretetchWordThrottlingStates = 6;

        private const int IndexMachineCycles = 1;
        private const int IndexThrottlingStates = 3;

        private const int DisplacedIndexMachineCycles = 3;
        private const int DisplacedIndexThrottlingStates = 11;

        private const int Arithmetic16MachineCycles = 2;
        private const int Arithmetic16ThrottlingStates = 7;

        private const int IoMachineCycles = 1;
        private const int IoThrottlingStates = 4;

        private const int ApplyDisplacementMachineCycles = 1;
        private const int ApplyDisplacementThrottlingStates = 5;

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

        public IInstructionTimingsBuilder MmuByte()
        {
            MachineCycles += PrefetchMachineCycles;
            ThrottlingStates += PretetchThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder MmuWord()
        {
            MachineCycles += PrefetchWordMachineCycles;
            ThrottlingStates += PretetchWordThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder Index(bool isDisplaced)
        {
            MachineCycles += isDisplaced ? DisplacedIndexMachineCycles : IndexMachineCycles;
            ThrottlingStates += isDisplaced ? DisplacedIndexThrottlingStates : IndexThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder IndexAndMmuByte(bool isDisplaced)
        {
            // Only add on prefetch timings when not an indexed register
            MachineCycles += isDisplaced ? DisplacedIndexMachineCycles : IndexMachineCycles + PrefetchMachineCycles;
            ThrottlingStates += isDisplaced ? DisplacedIndexThrottlingStates : IndexThrottlingStates + PretetchThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder IndexAndMmuWord()
        {
            // Only add on prefetch timings when not an indexed register
            MachineCycles += 2 * IndexMachineCycles + PrefetchWordMachineCycles;
            ThrottlingStates += 2 * IndexThrottlingStates + PretetchWordThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder Extend(int tStates)
        {
            ThrottlingStates += tStates;
            return this;
        }

        public IInstructionTimingsBuilder Arithmetic16()
        {
            MachineCycles += Arithmetic16MachineCycles;
            ThrottlingStates += Arithmetic16ThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder Io()
        {
            MachineCycles += IoMachineCycles;
            ThrottlingStates += IoThrottlingStates;
            return this;
        }

        public IInstructionTimingsBuilder ApplyDisplacement()
        {
            MachineCycles += ApplyDisplacementMachineCycles;
            ThrottlingStates += ApplyDisplacementThrottlingStates;
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
