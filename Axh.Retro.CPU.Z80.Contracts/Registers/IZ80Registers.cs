using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    /// <summary>
    /// Z80 registers which extend the original Intel 8080 registers with:
    /// 
    /// IX: 16-bit index or base register for 8-bit immediate offsets
    /// IY: 16-bit index or base register for 8-bit immediate offsets
    /// I: interrupt vector base register, 8 bits
    /// R: DRAM refresh counter, 8 bits(msb does not count)
    /// AF': alternate (or shadow) accumulator and flags (toggled in and out with EX AF,AF' )
    /// BC', DE' and HL': alternate (or shadow) registers (toggled in and out with EXX)
    /// Four bits of interrupt status and interrupt mode status
    /// 
    /// There is no direct access to the alternate registers;
    /// instead, two special instructions, EX AF,AF' and EXX, each toggles one of two multiplexer flip-flops;
    /// this enables fast context switches for interrupt service routines: EX AF, AF'.
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