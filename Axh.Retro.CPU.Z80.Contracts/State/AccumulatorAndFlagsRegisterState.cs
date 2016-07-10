﻿namespace Axh.Retro.CPU.Z80.Contracts.State
{
    /// <summary>
    /// The state of Intel 8080 registers A and F.
    /// </summary>
    public struct AccumulatorAndFlagsRegisterState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccumulatorAndFlagsRegisterState"/> struct.
        /// </summary>
        /// <param name="a">The A register value.</param>
        /// <param name="f">The F register value.</param>
        public AccumulatorAndFlagsRegisterState(byte a, byte f) : this()
        {
            A = a;
            F = f;
        }

        /// <summary>
        /// Gets the A register.
        /// </summary>
        /// <value>
        /// The A register.
        /// </value>
        public byte A { get; }

        /// <summary>
        /// Gets the F register.
        /// </summary>
        /// <value>
        /// The F register.
        /// </value>
        public byte F { get; }
    }
}