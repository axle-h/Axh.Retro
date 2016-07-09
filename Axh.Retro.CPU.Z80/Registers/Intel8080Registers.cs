using System;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Registers
{
    /// <summary>
    /// CPU registers for intel 8080 based CPU's.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Registers.IRegisters" />
    public class Intel8080Registers : IRegisters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Intel8080Registers"/> class.
        /// </summary>
        /// <param name="initialStateFactory">The initial state factory.</param>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public Intel8080Registers(IInitialStateFactory initialStateFactory, IPlatformConfig platformConfig)
        {
            IFlagsRegister flagsRegister;
            switch (platformConfig.CpuMode)
            {
                case CpuMode.Intel8080:
                case CpuMode.Z80:
                    flagsRegister = new Intel8080FlagsRegister();
                    break;
                case CpuMode.GameBoy:
                    flagsRegister = new GameBoyFlagsRegister();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GeneralPurposeRegisters = new GeneralPurposeRegisterSet();
            AccumulatorAndFlagsRegisters = new AccumulatorAndFlagsRegisterSet(flagsRegister);
            ResetToState(initialStateFactory.GetInitialRegisterState());
        }

        /// <summary>
        /// Gets the general purpose registers.
        /// </summary>
        /// <value>
        /// The general purpose registers.
        /// </value>
        public GeneralPurposeRegisterSet GeneralPurposeRegisters { get; }

        /// <summary>
        /// Gets the accumulator and flags registers.
        /// </summary>
        /// <value>
        /// The accumulator and flags registers.
        /// </value>
        public AccumulatorAndFlagsRegisterSet AccumulatorAndFlagsRegisters { get; }


        /// <summary>
        /// Gets or sets the stack pointer.
        /// </summary>
        /// <value>
        /// The stack pointer.
        /// </value>
        public ushort StackPointer { get; set; }

        /// <summary>
        /// Gets or sets the program counter.
        /// </summary>
        /// <value>
        /// The program counter.
        /// </value>
        public ushort ProgramCounter { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [interrupt flip flop1].
        /// </summary>
        /// <value>
        /// <c>true</c> if [interrupt flip flop1]; otherwise, <c>false</c>.
        /// </value>
        public bool InterruptFlipFlop1 { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [interrupt flip flop2].
        /// </summary>
        /// <value>
        /// <c>true</c> if [interrupt flip flop2]; otherwise, <c>false</c>.
        /// </value>
        public bool InterruptFlipFlop2 { get; set; }

        /// <summary>
        /// Gets or sets the interrupt mode.
        /// </summary>
        /// <value>
        /// The interrupt mode.
        /// </value>
        public InterruptMode InterruptMode { get; set; }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            GeneralPurposeRegisters.Reset();
            AccumulatorAndFlagsRegisters.Reset();

            StackPointer = 0;
            ProgramCounter = 0;
            InterruptFlipFlop1 = InterruptFlipFlop2 = false;
            InterruptMode = InterruptMode.InterruptMode0;
        }

        /// <summary>
        /// Resets the registers to the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        public void ResetToState(Z80RegisterState state)
        {
            GeneralPurposeRegisters.ResetToState(state.PrimaryGeneralPurposeRegisterState);
            AccumulatorAndFlagsRegisters.ResetToState(state.PrimaryAccumulatorAndFlagsRegisterState);

            StackPointer = state.StackPointer;
            ProgramCounter = state.ProgramCounter;

            InterruptFlipFlop1 = state.InterruptFlipFlop1;
            InterruptFlipFlop2 = state.InterruptFlipFlop2;
            InterruptMode = state.InterruptMode;
        }

        /// <summary>
        /// Gets the state of the register.
        /// </summary>
        /// <returns></returns>
        public Z80RegisterState GetRegisterState()
        {
            return new Z80RegisterState { PrimaryAccumulatorAndFlagsRegisterState = AccumulatorAndFlagsRegisters.GetRegisterState(), InterruptFlipFlop1 = InterruptFlipFlop1, InterruptFlipFlop2 = InterruptFlipFlop2, InterruptMode = InterruptMode, PrimaryGeneralPurposeRegisterState = GeneralPurposeRegisters.GetRegisterState(), ProgramCounter = ProgramCounter, StackPointer = StackPointer };
        }
    }
}