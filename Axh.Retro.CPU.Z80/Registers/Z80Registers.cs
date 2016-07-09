using Axh.Retro.CPU.Common.Contracts.Util;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Registers
{
    /// <summary>
    /// CPU registers for use with a Z80 based CPU.
    /// </summary>
    public class Z80Registers : IZ80Registers
    {
        private readonly object _accumulatorAndFlagsLockingContext = new object();
        private readonly AccumulatorAndFlagsRegisterSet _alternativeAccumulatorAndFlagsRegisterSet;
        private readonly GeneralPurposeRegisterSet _alternativeGeneralPurposeRegisterSet;
        private readonly object _generalPurposeLockingContext = new object();

        private readonly AccumulatorAndFlagsRegisterSet _primaryAccumulatorAndFlagsRegisterSet;
        private readonly GeneralPurposeRegisterSet _primaryGeneralPurposeRegisterSet;
        private bool _isAccumulatorAndFlagsAlternative;
        private bool _isGeneralPurposeAlternative;

        /// <summary>
        /// Initializes a new instance of the <see cref="Z80Registers"/> class.
        /// </summary>
        /// <param name="initialStateFactory">The initial state factory.</param>
        public Z80Registers(IInitialStateFactory initialStateFactory)
        {
            _primaryGeneralPurposeRegisterSet = new GeneralPurposeRegisterSet();
            _alternativeGeneralPurposeRegisterSet = new GeneralPurposeRegisterSet();
            _primaryAccumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(new Intel8080FlagsRegister());
            _alternativeAccumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(new Intel8080FlagsRegister());

            ResetToState(initialStateFactory.GetInitialRegisterState());
        }

        /// <summary>
        /// Gets the general purpose registers.
        /// </summary>
        /// <value>
        /// The general purpose registers.
        /// </value>
        public GeneralPurposeRegisterSet GeneralPurposeRegisters { get; private set; }

        /// <summary>
        /// Gets the accumulator and flags registers.
        /// </summary>
        /// <value>
        /// The accumulator and flags registers.
        /// </value>
        public AccumulatorAndFlagsRegisterSet AccumulatorAndFlagsRegisters { get; private set; }
        
        /// <summary>
        /// Gets or sets the IX register.
        /// </summary>
        /// <value>
        /// The IX register.
        /// </value>
        public ushort IX { get; set; }
        
        /// <summary>
        /// Gets or sets the IY register.
        /// </summary>
        /// <value>
        /// The IY register.
        /// </value>
        public ushort IY { get; set; }

        /// <summary>
        /// Gets or sets the lower byte of the IX register.
        /// </summary>
        /// <value>
        /// The lower byte of the IX register.
        /// </value>
        public byte IXl
        {
            get { return BitConverterHelpers.GetLowOrderByte(IX); }
            set { IX = BitConverterHelpers.SetLowOrderByte(IX, value); }
        }

        /// <summary>
        /// Gets or sets the upper byte of the IX register.
        /// </summary>
        /// <value>
        /// The upper byte of the IX register.
        /// </value>
        public byte IXh
        {
            get { return BitConverterHelpers.GetHighOrderByte(IX); }
            set { IX = BitConverterHelpers.SetHighOrderByte(IX, value); }
        }

        /// <summary>
        /// Gets or sets the lower byte of the IY register.
        /// </summary>
        /// <value>
        /// The lower byte of the IY register.
        /// </value>
        public byte IYl
        {
            get { return BitConverterHelpers.GetLowOrderByte(IY); }
            set { IY = BitConverterHelpers.SetLowOrderByte(IY, value); }
        }

        /// <summary>
        /// Gets or sets the upper byte of the IY register.
        /// </summary>
        /// <value>
        /// The upper byte of the IY register.
        /// </value>
        public byte IYh
        {
            get { return BitConverterHelpers.GetHighOrderByte(IY); }
            set { IY = BitConverterHelpers.SetHighOrderByte(IY, value); }
        }
        
        /// <summary>
        /// Gets or sets the I register.
        /// </summary>
        /// <value>
        /// The I register.
        /// </value>
        public byte I { get; set; }
        
        /// <summary>
        /// </summary>
        /// <value>
        /// The R register.
        /// </value>
        public byte R { get; set; }

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
        /// Switches to alternative general purpose registers.
        /// </summary>
        public void SwitchToAlternativeGeneralPurposeRegisters()
        {
            lock (_generalPurposeLockingContext)
            {
                _isGeneralPurposeAlternative = !_isGeneralPurposeAlternative;
                GeneralPurposeRegisters = _isGeneralPurposeAlternative
                                              ? _alternativeGeneralPurposeRegisterSet
                                              : _primaryGeneralPurposeRegisterSet;
            }
        }

        /// <summary>
        /// Switches to alternative accumulator and flags registers.
        /// </summary>
        public void SwitchToAlternativeAccumulatorAndFlagsRegisters()
        {
            lock (_accumulatorAndFlagsLockingContext)
            {
                _isAccumulatorAndFlagsAlternative = !_isAccumulatorAndFlagsAlternative;
                AccumulatorAndFlagsRegisters = _isAccumulatorAndFlagsAlternative
                                                   ? _alternativeAccumulatorAndFlagsRegisterSet
                                                   : _primaryAccumulatorAndFlagsRegisterSet;
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            _primaryGeneralPurposeRegisterSet.Reset();
            _alternativeGeneralPurposeRegisterSet.Reset();
            GeneralPurposeRegisters = _primaryGeneralPurposeRegisterSet;
            _isGeneralPurposeAlternative = false;

            _primaryAccumulatorAndFlagsRegisterSet.Reset();
            _alternativeAccumulatorAndFlagsRegisterSet.Reset();
            AccumulatorAndFlagsRegisters = _primaryAccumulatorAndFlagsRegisterSet;
            _isAccumulatorAndFlagsAlternative = false;

            IX = IY = 0;
            I = R = 0;
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
            _primaryGeneralPurposeRegisterSet.ResetToState(state.PrimaryGeneralPurposeRegisterState);
            _alternativeGeneralPurposeRegisterSet.ResetToState(state.AlternativeGeneralPurposeRegisterState);
            GeneralPurposeRegisters = state.IsGeneralPurposeAlternative
                                          ? _alternativeGeneralPurposeRegisterSet
                                          : _primaryGeneralPurposeRegisterSet;
            _isGeneralPurposeAlternative = state.IsGeneralPurposeAlternative;

            _primaryAccumulatorAndFlagsRegisterSet.ResetToState(state.PrimaryAccumulatorAndFlagsRegisterState);
            _alternativeAccumulatorAndFlagsRegisterSet.ResetToState(state.AlternativeAccumulatorAndFlagsRegisterState);
            AccumulatorAndFlagsRegisters = state.IsAccumulatorAndFlagsAlternative
                                               ? _alternativeAccumulatorAndFlagsRegisterSet
                                               : _primaryAccumulatorAndFlagsRegisterSet;
            _isAccumulatorAndFlagsAlternative = state.IsAccumulatorAndFlagsAlternative;

            IX = state.IX;
            IY = state.IY;
            I = state.I;
            R = state.R;
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
            return new Z80RegisterState
                   {
                       AlternativeAccumulatorAndFlagsRegisterState =
                           _alternativeAccumulatorAndFlagsRegisterSet.GetRegisterState(),
                       AlternativeGeneralPurposeRegisterState =
                           _alternativeGeneralPurposeRegisterSet.GetRegisterState(),
                       I = I,
                       IX = IX,
                       IY = IY,
                       InterruptFlipFlop1 = InterruptFlipFlop1,
                       InterruptFlipFlop2 = InterruptFlipFlop2,
                       InterruptMode = InterruptMode,
                       IsAccumulatorAndFlagsAlternative = _isAccumulatorAndFlagsAlternative,
                       IsGeneralPurposeAlternative = _isGeneralPurposeAlternative,
                       PrimaryAccumulatorAndFlagsRegisterState =
                           _primaryAccumulatorAndFlagsRegisterSet.GetRegisterState(),
                       PrimaryGeneralPurposeRegisterState =
                           _primaryGeneralPurposeRegisterSet.GetRegisterState(),
                       ProgramCounter = ProgramCounter,
                       R = R,
                       StackPointer = StackPointer
                   };
        }
    }
}