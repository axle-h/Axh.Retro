using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.CPU.Z80.Util;

namespace Axh.Retro.CPU.Z80.Registers
{
    public class Z80Registers : IZ80Registers
    {
        private readonly object accumulatorAndFlagsLockingContext = new object();
        private readonly IAccumulatorAndFlagsRegisterSet alternativeAccumulatorAndFlagsRegisterSet;
        private readonly IGeneralPurposeRegisterSet alternativeGeneralPurposeRegisterSet;
        private readonly object generalPurposeLockingContext = new object();

        private readonly IAccumulatorAndFlagsRegisterSet primaryAccumulatorAndFlagsRegisterSet;
        private readonly IGeneralPurposeRegisterSet primaryGeneralPurposeRegisterSet;
        private bool isAccumulatorAndFlagsAlternative;
        private bool isGeneralPurposeAlternative;

        public Z80Registers(IGeneralPurposeRegisterSet primaryGeneralPurposeRegisterSet,
                            IGeneralPurposeRegisterSet alternativeGeneralPurposeRegisterSet,
                            IAccumulatorAndFlagsRegisterSet primaryAccumulatorAndFlagsRegisterSet,
                            IAccumulatorAndFlagsRegisterSet alternativeAccumulatorAndFlagsRegisterSet,
                            IInitialStateFactory<Z80RegisterState> initialStateFactory)
        {
            this.primaryGeneralPurposeRegisterSet = primaryGeneralPurposeRegisterSet;
            this.alternativeGeneralPurposeRegisterSet = alternativeGeneralPurposeRegisterSet;
            this.primaryAccumulatorAndFlagsRegisterSet = primaryAccumulatorAndFlagsRegisterSet;
            this.alternativeAccumulatorAndFlagsRegisterSet = alternativeAccumulatorAndFlagsRegisterSet;

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
            lock (generalPurposeLockingContext)
            {
                isGeneralPurposeAlternative = !isGeneralPurposeAlternative;
                GeneralPurposeRegisters = isGeneralPurposeAlternative
                    ? alternativeGeneralPurposeRegisterSet
                    : primaryGeneralPurposeRegisterSet;
            }
        }

        public void SwitchToAlternativeAccumulatorAndFlagsRegisters()
        {
            lock (accumulatorAndFlagsLockingContext)
            {
                isAccumulatorAndFlagsAlternative = !isAccumulatorAndFlagsAlternative;
                AccumulatorAndFlagsRegisters = isAccumulatorAndFlagsAlternative
                    ? alternativeAccumulatorAndFlagsRegisterSet
                    : primaryAccumulatorAndFlagsRegisterSet;
            }
        }

        public void Reset()
        {
            primaryGeneralPurposeRegisterSet.Reset();
            alternativeGeneralPurposeRegisterSet.Reset();
            GeneralPurposeRegisters = primaryGeneralPurposeRegisterSet;
            isGeneralPurposeAlternative = false;

            primaryAccumulatorAndFlagsRegisterSet.Reset();
            alternativeAccumulatorAndFlagsRegisterSet.Reset();
            AccumulatorAndFlagsRegisters = primaryAccumulatorAndFlagsRegisterSet;
            isAccumulatorAndFlagsAlternative = false;

            IX = IY = 0;
            I = R = 0;
            StackPointer = 0;
            ProgramCounter = 0;
            InterruptFlipFlop1 = InterruptFlipFlop2 = false;
            InterruptMode = InterruptMode.InterruptMode0;
        }

        public void ResetToState(Z80RegisterState state)
        {
            primaryGeneralPurposeRegisterSet.ResetToState(state.PrimaryGeneralPurposeRegisterState);
            alternativeGeneralPurposeRegisterSet.ResetToState(state.AlternativeGeneralPurposeRegisterState);
            GeneralPurposeRegisters = state.IsGeneralPurposeAlternative
                ? alternativeGeneralPurposeRegisterSet
                : primaryGeneralPurposeRegisterSet;
            isGeneralPurposeAlternative = state.IsGeneralPurposeAlternative;

            primaryAccumulatorAndFlagsRegisterSet.ResetToState(state.PrimaryAccumulatorAndFlagsRegisterState);
            alternativeAccumulatorAndFlagsRegisterSet.ResetToState(state.AlternativeAccumulatorAndFlagsRegisterState);
            AccumulatorAndFlagsRegisters = state.IsAccumulatorAndFlagsAlternative
                ? alternativeAccumulatorAndFlagsRegisterSet
                : primaryAccumulatorAndFlagsRegisterSet;
            isAccumulatorAndFlagsAlternative = state.IsAccumulatorAndFlagsAlternative;

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
                           alternativeAccumulatorAndFlagsRegisterSet.GetRegisterState(),
                       AlternativeGeneralPurposeRegisterState =
                           alternativeGeneralPurposeRegisterSet.GetRegisterState(),
                       I = I,
                       IX = IX,
                       IY = IY,
                       InterruptFlipFlop1 = InterruptFlipFlop1,
                       InterruptFlipFlop2 = InterruptFlipFlop2,
                       InterruptMode = InterruptMode,
                       IsAccumulatorAndFlagsAlternative = isAccumulatorAndFlagsAlternative,
                       IsGeneralPurposeAlternative = isGeneralPurposeAlternative,
                       PrimaryAccumulatorAndFlagsRegisterState =
                           primaryAccumulatorAndFlagsRegisterSet.GetRegisterState(),
                       PrimaryGeneralPurposeRegisterState =
                           primaryGeneralPurposeRegisterSet.GetRegisterState(),
                       ProgramCounter = ProgramCounter,
                       R = R,
                       StackPointer = StackPointer
                   };
        }
    }
}