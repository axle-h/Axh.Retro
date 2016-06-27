using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Z80.Core.Decode
{
    internal class DecodedBlock
    {
        public DecodedBlock(Operation[] operations, InstructionTimings timings)
        {
            Operations = operations;
            Timings = timings;
        }

        public Operation[] Operations { get; }

        public InstructionTimings Timings { get; }
    }
}