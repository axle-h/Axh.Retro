using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System.Collections.Generic;

    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;

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
