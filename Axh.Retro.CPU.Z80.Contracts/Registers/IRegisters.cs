using System;
using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    /// <summary>
    /// Intel 8080 registers.
    /// The processor has seven 8-bit registers (A, B, C, D, E, H, and L).
    /// Where A is the primary 8-bit accumulator and the other six registers can be used as either individual 8-bit registers
    /// or as three 16-bit register pairs (BC, DE, and HL) depending on the particular instruction.
    /// It also has a 16-bit stack pointer to memory (replacing the 8008's internal stack), and a 16-bit program counter.
    /// The processor maintains internal flag bits (a status register) which indicates the results of arithmetic and logical instructions.
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
        /// Resets the registers to their initial state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Resets the registers to the specified state.
        /// </summary>
        /// <typeparam name="TRegisterState">The type of the register state.</typeparam>
        /// <exception cref="ArgumentException">The register state type must be compatible.</exception>
        /// <param name="state">The state.</param>
        void ResetToState<TRegisterState>(TRegisterState state) where TRegisterState : Intel8080RegisterState;

        /// <summary>
        /// Gets the state of the registers in Intel 8080 format.
        /// </summary>
        /// <exception cref="NotSupportedException">The implementation must have Intel 8080 style registers. I.e. not be a Z80.</exception>
        /// <returns></returns>
        Intel8080RegisterState GetIntel8080RegisterState();
        
        /// <summary>
        /// Gets the state of the registers in Z80 format.
        /// </summary>
        /// <returns></returns>
        Z80RegisterState GetZ80RegisterState();
    }
}