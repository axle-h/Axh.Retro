using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    /// <summary>
    /// CPU registers for use with a Z80 based CPU.
    /// </summary>
    public interface IZ80Registers : IRegisters
    {
        /// <summary>
        /// Gets or sets the IX register.
        /// </summary>
        /// <value>
        /// The IX register.
        /// </value>
        ushort IX { get; set; }

        /// <summary>
        /// Gets or sets the IY register.
        /// </summary>
        /// <value>
        /// The IY register.
        /// </value>
        ushort IY { get; set; }

        /// <summary>
        /// Gets or sets the lower byte of the IX register.
        /// </summary>
        /// <value>
        /// The lower byte of the IX register.
        /// </value>
        byte IXl { get; set; }

        /// <summary>
        /// Gets or sets the upper byte of the IX register.
        /// </summary>
        /// <value>
        /// The upper byte of the IX register.
        /// </value>
        byte IXh { get; set; }

        /// <summary>
        /// Gets or sets the lower byte of the IY register.
        /// </summary>
        /// <value>
        /// The lower byte of the IY register.
        /// </value>
        byte IYl { get; set; }

        /// <summary>
        /// Gets or sets the upper byte of the IY register.
        /// </summary>
        /// <value>
        /// The upper byte of the IY register.
        /// </value>
        byte IYh { get; set; }

        /// <summary>
        /// Gets or sets the I register.
        /// </summary>
        /// <value>
        /// The I register.
        /// </value>
        byte I { get; set; }

        /// <summary>
        /// Gets or sets the R register.
        /// </summary>
        /// <value>
        /// The R register.
        /// </value>
        byte R { get; set; }

        /// <summary>
        /// Switches to alternative general purpose registers.
        /// </summary>
        void SwitchToAlternativeGeneralPurposeRegisters();

        /// <summary>
        /// Switches to alternative accumulator and flags registers.
        /// </summary>
        void SwitchToAlternativeAccumulatorAndFlagsRegisters();
    }
}