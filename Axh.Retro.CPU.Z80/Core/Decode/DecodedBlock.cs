using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Z80.Core.Decode
{
    /// <summary>
    /// A decoded block of CPU op-codes.
    /// </summary>
    internal class DecodedBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecodedBlock"/> class.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="timings">The timings.</param>
        public DecodedBlock(Operation[] operations, InstructionTimings timings)
        {
            Operations = operations;
            Timings = timings;
        }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        public Operation[] Operations { get; }

        /// <summary>
        /// Gets the timings.
        /// </summary>
        /// <value>
        /// The timings.
        /// </value>
        public InstructionTimings Timings { get; }
    }
}