﻿using Axh.Retro.CPU.Common.Contracts.Util;
using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    /// <summary>
    /// The accumulator and flags registers.
    /// </summary>
    public class AccumulatorAndFlagsRegisterSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccumulatorAndFlagsRegisterSet"/> class.
        /// </summary>
        /// <param name="flagsRegister">The flags register.</param>
        public AccumulatorAndFlagsRegisterSet(IFlagsRegister flagsRegister)
        {
            Flags = flagsRegister;
        }

        /// <summary>
        /// Gets or sets the A register.
        /// </summary>
        /// <value>
        /// The A register.
        /// </value>
        public byte A { get; set; }

        /// <summary>
        /// Gets the flags register.
        /// </summary>
        /// <value>
        /// The flags register.
        /// </value>
        public IFlagsRegister Flags { get; }

        /// <summary>
        /// Gets or sets the AF register.
        /// </summary>
        /// <value>
        /// The AF register.
        /// </value>
        public ushort AF
        {
            get { return BitConverterHelpers.To16Bit(A, Flags.Register); }
            set
            {
                var bytes = BitConverterHelpers.To8Bit(value);
                A = bytes[1];
                Flags.Register = bytes[0];
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            A = 0;
            Flags.ResetFlags();
        }
        
        /// <summary>
        /// Resets the register values to the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        public void ResetToState(AccumulatorAndFlagsRegisterState state)
        {
            A = state.A;
            Flags.Register = state.F;
        }

        /// <summary>
        /// Gets the state of the registers.
        /// </summary>
        /// <returns></returns>
        public AccumulatorAndFlagsRegisterState GetRegisterState() => new AccumulatorAndFlagsRegisterState(A, Flags.Register);
    }
}