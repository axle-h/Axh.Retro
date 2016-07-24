using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Z80.Core.Decode
{
    /// <summary>
    /// A decoded block of CPU op-codes.
    /// </summary>
    public class DecodedBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecodedBlock"/> class.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="timings">The timings.</param>
        /// <param name="halt">if set to <c>true</c> [ends on a HALT instruction].</param>
        /// <param name="stop">if set to <c>true</c> [ends on a STOP instruction].</param>
        public DecodedBlock(Operation[] operations, InstructionTimings timings, bool halt, bool stop)
        {
            Operations = operations;
            Timings = timings;
            Halt = halt;
            Stop = stop;
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

        /// <summary>
        /// Gets a value indicating whether this <see cref="DecodedBlock"/> ends on a HALT instruction.
        /// </summary>
        /// <value>
        ///   <c>true</c> if halt; otherwise, <c>false</c>.
        /// </value>
        public bool Halt { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DecodedBlock"/> ends on a STOP instruction.
        /// </summary>
        /// <value>
        ///   <c>true</c> if stop; otherwise, <c>false</c>.
        /// </value>
        public bool Stop { get; }
    }
}