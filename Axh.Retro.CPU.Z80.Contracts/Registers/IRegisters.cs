using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    /// <summary>
    /// CPU registers.
    /// </summary>
    public interface IRegisters
    {
        /// <summary>
        /// Gets the general purpose registers.
        /// </summary>
        /// <value>
        /// The general purpose registers.
        /// </value>
        GeneralPurposeRegisterSet GeneralPurposeRegisters { get; }

        /// <summary>
        /// Gets the accumulator and flags registers.
        /// </summary>
        /// <value>
        /// The accumulator and flags registers.
        /// </value>
        AccumulatorAndFlagsRegisterSet AccumulatorAndFlagsRegisters { get; }

        /// <summary>
        /// Gets or sets the stack pointer.
        /// </summary>
        /// <value>
        /// The stack pointer.
        /// </value>
        ushort StackPointer { get; set; }

        /// <summary>
        /// Gets or sets the program counter.
        /// </summary>
        /// <value>
        /// The program counter.
        /// </value>
        ushort ProgramCounter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [interrupt flip flop1].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [interrupt flip flop1]; otherwise, <c>false</c>.
        /// </value>
        bool InterruptFlipFlop1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [interrupt flip flop2].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [interrupt flip flop2]; otherwise, <c>false</c>.
        /// </value>
        bool InterruptFlipFlop2 { get; set; }

        /// <summary>
        /// Gets or sets the interrupt mode.
        /// </summary>
        /// <value>
        /// The interrupt mode.
        /// </value>
        InterruptMode InterruptMode { get; set; }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();

        /// <summary>
        /// Resets the registers to the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        void ResetToState(Z80RegisterState state);

        /// <summary>
        /// Gets the state of the register.
        /// </summary>
        /// <returns></returns>
        Z80RegisterState GetRegisterState();
    }
}