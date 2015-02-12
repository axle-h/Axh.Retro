namespace Axh.Emulation.CPU.Z80.Registers
{
    using Axh.Emulation.CPU.Z80.Contracts.Opcodes;
    using Axh.Emulation.CPU.Z80.Contracts.Registers;
    using Axh.Emulation.CPU.Z80.Contracts.State;

    public class Z80Registers : IZ80Registers
    {
        private readonly IGeneralPurposeRegisterSet primaryGeneralPurposeRegisterSet;
        private readonly IGeneralPurposeRegisterSet alternativeGeneralPurposeRegisterSet;
        private bool isAlternative;

        private readonly object switchToAlternativeLockingContext = new object();

        public IGeneralPurposeRegisterSet GeneralPurposeRegisters { get; private set; }

        //Index Registers
        public ushort IX { get; set; }
        public ushort IY { get; set; }

        //IR Flags
        public byte I { get; set; }
        public byte R { get; set; }

        //Pointers
        public ushort StackPointer { get; set; }
        public ushort ProgramCounter { get; set; }

        //Interrupt Flip-Flops
        public bool InterruptFlipFlop1 { get; set; }
        public bool InterruptFlipFlop2 { get; set; }
        public InterruptMode InterruptMode { get; set; }

        public Z80Registers(IGeneralPurposeRegisterSet primaryGeneralPurposeRegisterSet, IGeneralPurposeRegisterSet alternativeGeneralPurposeRegisterSet)
        {
            this.primaryGeneralPurposeRegisterSet = primaryGeneralPurposeRegisterSet;
            this.alternativeGeneralPurposeRegisterSet = alternativeGeneralPurposeRegisterSet;
            
            Reset();
        }

        public void SwitchToAlternativeGeneralPurposeRegisters()
        {
            lock (this.switchToAlternativeLockingContext)
            {
                this.isAlternative = !isAlternative;
                this.GeneralPurposeRegisters = isAlternative ? this.alternativeGeneralPurposeRegisterSet : this.primaryGeneralPurposeRegisterSet;
            }
        }

        public void Reset()
        {
            this.primaryGeneralPurposeRegisterSet.Reset();
            this.alternativeGeneralPurposeRegisterSet.Reset();
            this.GeneralPurposeRegisters = this.primaryGeneralPurposeRegisterSet;
            this.isAlternative = false;

            IX = IY = 0;
            I = R = 0;
            StackPointer = 0;
            ProgramCounter = 0;
            InterruptFlipFlop1 = InterruptFlipFlop2 = false;
            InterruptMode = InterruptMode.InterruptMode0;

        }

        public void ResetToState(Z80RegisterState state)
        {
            this.primaryGeneralPurposeRegisterSet.ResetToState(state.PrimaryRegisterState);
            this.alternativeGeneralPurposeRegisterSet.ResetToState(state.AlternativeRegisterState);
            
            this.GeneralPurposeRegisters = state.IsAlternative ? this.alternativeGeneralPurposeRegisterSet : this.primaryGeneralPurposeRegisterSet;
            this.isAlternative = state.IsAlternative;

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
                AlternativeRegisterState = this.alternativeGeneralPurposeRegisterSet.GetRegisterState(),
                I = this.I,
                IX = this.IX,
                IY = this.IY,
                InterruptFlipFlop1 = this.InterruptFlipFlop1,
                InterruptFlipFlop2 = this.InterruptFlipFlop2,
                InterruptMode = this.InterruptMode,
                IsAlternative = this.isAlternative,
                PrimaryRegisterState = this.primaryGeneralPurposeRegisterSet.GetRegisterState(),
                ProgramCounter = this.ProgramCounter,
                R = this.R,
                StackPointer = this.StackPointer
            };
        }
    }
}
