namespace Axh.Retro.CPU.Z80.Registers
{
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.CPU.Z80.Util;

    public class Z80Registers : IZ80Registers
    {
        private readonly IGeneralPurposeRegisterSet primaryGeneralPurposeRegisterSet;
        private readonly IGeneralPurposeRegisterSet alternativeGeneralPurposeRegisterSet;
        private bool isGeneralPurposeAlternative;
        private readonly object generalPurposeLockingContext = new object();

        private readonly IAccumulatorAndFlagsRegisterSet primaryAccumulatorAndFlagsRegisterSet;
        private readonly IAccumulatorAndFlagsRegisterSet alternativeAccumulatorAndFlagsRegisterSet;
        private bool isAccumulatorAndFlagsAlternative;
        private readonly object accumulatorAndFlagsLockingContext = new object();

        public IGeneralPurposeRegisterSet GeneralPurposeRegisters { get; private set; }

        public IAccumulatorAndFlagsRegisterSet AccumulatorAndFlagsRegisters { get; private set; }

        //Index Registers
        public ushort IX { get; set; }
        public ushort IY { get; set; }

        public byte IXl
        {
            get
            {
                return BitConverterHelpers.GetLowOrderByte(IX);
            }
            set
            {
                IX = BitConverterHelpers.SetLowOrderByte(IX, value);
            }
        }

        public byte IXh
        {
            get
            {
                return BitConverterHelpers.GetHighOrderByte(IX);
            }
            set
            {
                IX = BitConverterHelpers.SetHighOrderByte(IX, value);
            }
        }

        public byte IYl
        {
            get
            {
                return BitConverterHelpers.GetLowOrderByte(IY);
            }
            set
            {
                IY = BitConverterHelpers.SetLowOrderByte(IY, value);
            }
        }

        public byte IYh
        {
            get
            {
                return BitConverterHelpers.GetHighOrderByte(IY);
            }
            set
            {
                IY = BitConverterHelpers.SetHighOrderByte(IY, value);
            }
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

        public Z80Registers(
            IGeneralPurposeRegisterSet primaryGeneralPurposeRegisterSet,
            IGeneralPurposeRegisterSet alternativeGeneralPurposeRegisterSet,
            IAccumulatorAndFlagsRegisterSet primaryAccumulatorAndFlagsRegisterSet,
            IAccumulatorAndFlagsRegisterSet alternativeAccumulatorAndFlagsRegisterSet,
            Z80RegisterState initialState)
        {
            this.primaryGeneralPurposeRegisterSet = primaryGeneralPurposeRegisterSet;
            this.alternativeGeneralPurposeRegisterSet = alternativeGeneralPurposeRegisterSet;
            this.primaryAccumulatorAndFlagsRegisterSet = primaryAccumulatorAndFlagsRegisterSet;
            this.alternativeAccumulatorAndFlagsRegisterSet = alternativeAccumulatorAndFlagsRegisterSet;

            this.ResetToState(initialState);
        }

        public void SwitchToAlternativeGeneralPurposeRegisters()
        {
            lock (this.generalPurposeLockingContext)
            {
                this.isGeneralPurposeAlternative = !isGeneralPurposeAlternative;
                this.GeneralPurposeRegisters = isGeneralPurposeAlternative ? this.alternativeGeneralPurposeRegisterSet : this.primaryGeneralPurposeRegisterSet;
            }
        }

        public void SwitchToAlternativeAccumulatorAndFlagsRegisters()
        {
            lock (this.accumulatorAndFlagsLockingContext)
            {
                this.isAccumulatorAndFlagsAlternative = !isAccumulatorAndFlagsAlternative;
                this.AccumulatorAndFlagsRegisters = isAccumulatorAndFlagsAlternative ? this.alternativeAccumulatorAndFlagsRegisterSet : this.primaryAccumulatorAndFlagsRegisterSet;
            }
        }

        public void Reset()
        {
            this.primaryGeneralPurposeRegisterSet.Reset();
            this.alternativeGeneralPurposeRegisterSet.Reset();
            this.GeneralPurposeRegisters = this.primaryGeneralPurposeRegisterSet;
            this.isGeneralPurposeAlternative = false;

            this.primaryAccumulatorAndFlagsRegisterSet.Reset();
            this.alternativeAccumulatorAndFlagsRegisterSet.Reset();
            this.AccumulatorAndFlagsRegisters = this.primaryAccumulatorAndFlagsRegisterSet;
            this.isAccumulatorAndFlagsAlternative = false;

            IX = IY = 0;
            I = R = 0;
            StackPointer = 0;
            ProgramCounter = 0;
            InterruptFlipFlop1 = InterruptFlipFlop2 = false;
            InterruptMode = InterruptMode.InterruptMode0;
        }

        public void ResetToState(Z80RegisterState state)
        {
            this.primaryGeneralPurposeRegisterSet.ResetToState(state.PrimaryGeneralPurposeRegisterState);
            this.alternativeGeneralPurposeRegisterSet.ResetToState(state.AlternativeGeneralPurposeRegisterState);
            this.GeneralPurposeRegisters = state.IsGeneralPurposeAlternative ? this.alternativeGeneralPurposeRegisterSet : this.primaryGeneralPurposeRegisterSet;
            this.isGeneralPurposeAlternative = state.IsGeneralPurposeAlternative;

            this.primaryAccumulatorAndFlagsRegisterSet.ResetToState(state.PrimaryAccumulatorAndFlagsRegisterState);
            this.alternativeAccumulatorAndFlagsRegisterSet.ResetToState(state.AlternativeAccumulatorAndFlagsRegisterState);
            this.AccumulatorAndFlagsRegisters = state.IsAccumulatorAndFlagsAlternative ? this.alternativeAccumulatorAndFlagsRegisterSet : this.primaryAccumulatorAndFlagsRegisterSet;
            this.isAccumulatorAndFlagsAlternative = state.IsAccumulatorAndFlagsAlternative;

            this.IX = state.IX;
            this.IY = state.IY;
            this.I = state.I;
            this.R = state.R;
            this.StackPointer = state.StackPointer;
            this.ProgramCounter = state.ProgramCounter;

            this.InterruptFlipFlop1 = state.InterruptFlipFlop1;
            this.InterruptFlipFlop2 = state.InterruptFlipFlop2;
            this.InterruptMode = state.InterruptMode;
        }

        public Z80RegisterState GetRegisterState()
        {
            return new Z80RegisterState
                   {
                       AlternativeAccumulatorAndFlagsRegisterState = this.alternativeAccumulatorAndFlagsRegisterSet.GetRegisterState(),
                       AlternativeGeneralPurposeRegisterState = this.alternativeGeneralPurposeRegisterSet.GetRegisterState(),
                       I = this.I,
                       IX = this.IX,
                       IY = this.IY,
                       InterruptFlipFlop1 = this.InterruptFlipFlop1,
                       InterruptFlipFlop2 = this.InterruptFlipFlop2,
                       InterruptMode = this.InterruptMode,
                       IsAccumulatorAndFlagsAlternative = this.isAccumulatorAndFlagsAlternative,
                       IsGeneralPurposeAlternative = this.isGeneralPurposeAlternative,
                       PrimaryAccumulatorAndFlagsRegisterState = this.primaryAccumulatorAndFlagsRegisterSet.GetRegisterState(),
                       PrimaryGeneralPurposeRegisterState = this.primaryGeneralPurposeRegisterSet.GetRegisterState(),
                       ProgramCounter = this.ProgramCounter,
                       R = this.R,
                       StackPointer = this.StackPointer
                   };
        }
    }
}
