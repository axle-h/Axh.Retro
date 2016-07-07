using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.CPU.Z80.Util;

namespace Axh.Retro.CPU.Z80.Registers
{
    public class Z80Registers : IZ80Registers
    {
        private readonly object _accumulatorAndFlagsLockingContext = new object();
        private readonly IAccumulatorAndFlagsRegisterSet _alternativeAccumulatorAndFlagsRegisterSet;
        private readonly IGeneralPurposeRegisterSet _alternativeGeneralPurposeRegisterSet;
        private readonly object _generalPurposeLockingContext = new object();

        private readonly IAccumulatorAndFlagsRegisterSet _primaryAccumulatorAndFlagsRegisterSet;
        private readonly IGeneralPurposeRegisterSet _primaryGeneralPurposeRegisterSet;
        private bool _isAccumulatorAndFlagsAlternative;
        private bool _isGeneralPurposeAlternative;

        public Z80Registers(IGeneralPurposeRegisterSet primaryGeneralPurposeRegisterSet,
            IGeneralPurposeRegisterSet alternativeGeneralPurposeRegisterSet,
            IAccumulatorAndFlagsRegisterSet primaryAccumulatorAndFlagsRegisterSet,
            IAccumulatorAndFlagsRegisterSet alternativeAccumulatorAndFlagsRegisterSet,
            IInitialStateFactory<Z80RegisterState> initialStateFactory)
        {
            _primaryGeneralPurposeRegisterSet = primaryGeneralPurposeRegisterSet;
            _alternativeGeneralPurposeRegisterSet = alternativeGeneralPurposeRegisterSet;
            _primaryAccumulatorAndFlagsRegisterSet = primaryAccumulatorAndFlagsRegisterSet;
            _alternativeAccumulatorAndFlagsRegisterSet = alternativeAccumulatorAndFlagsRegisterSet;

            ResetToState(initialStateFactory.GetInitialRegisterState());
        }

        public IGeneralPurposeRegisterSet GeneralPurposeRegisters { get; private set; }

        public IAccumulatorAndFlagsRegisterSet AccumulatorAndFlagsRegisters { get; private set; }

        //Index Registers
        public ushort IX { get; set; }
        public ushort IY { get; set; }

        public byte IXl
        {
            get { return BitConverterHelpers.GetLowOrderByte(IX); }
            set { IX = BitConverterHelpers.SetLowOrderByte(IX, value); }
        }

        public byte IXh
        {
            get { return BitConverterHelpers.GetHighOrderByte(IX); }
            set { IX = BitConverterHelpers.SetHighOrderByte(IX, value); }
        }

        public byte IYl
        {
            get { return BitConverterHelpers.GetLowOrderByte(IY); }
            set { IY = BitConverterHelpers.SetLowOrderByte(IY, value); }
        }

        public byte IYh
        {
            get { return BitConverterHelpers.GetHighOrderByte(IY); }
            set { IY = BitConverterHelpers.SetHighOrderByte(IY, value); }
        }

        // IR Flags
        public byte I { get; set; }
        public byte R { get; set; }

        // Pointers
        public ushort StackPointer { get; set; }
        public ushort ProgramCounter { get; set; }

        // Interrupt Flip-Flops
        public bool InterruptFlipFlop1 { get; set; }
        public bool InterruptFlipFlop2 { get; set; }
        public InterruptMode InterruptMode { get; set; }

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