namespace Axh.Retro.CPU.Z80.Registers
{
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;

    public class Intel8080Registers : IIntel8080Registers
    {
        public Intel8080Registers(IGeneralPurposeRegisterSet generalPurposeRegisters, IAccumulatorAndFlagsRegisterSet accumulatorAndFlagsRegisters, Intel8080RegisterState initialState)
        {
            GeneralPurposeRegisters = generalPurposeRegisters;
            AccumulatorAndFlagsRegisters = accumulatorAndFlagsRegisters;
            this.ResetToState(initialState);
        }

        public IGeneralPurposeRegisterSet GeneralPurposeRegisters { get; }

        public IAccumulatorAndFlagsRegisterSet AccumulatorAndFlagsRegisters { get; }

        // Pointers
        public ushort StackPointer { get; set; }
        public ushort ProgramCounter { get; set; }

        // Interrupt Flip-Flops
        public bool InterruptFlipFlop1 { get; set; }
        public bool InterruptFlipFlop2 { get; set; }
        public InterruptMode InterruptMode { get; set; }

        public void Reset()
        {
            this.GeneralPurposeRegisters.Reset();
            this.AccumulatorAndFlagsRegisters.Reset();
            
            StackPointer = 0;
            ProgramCounter = 0;
            InterruptFlipFlop1 = InterruptFlipFlop2 = false;
            InterruptMode = InterruptMode.InterruptMode0;
        }

        public void ResetToState(Intel8080RegisterState state)
        {
            this.GeneralPurposeRegisters.ResetToState(state.GeneralPurposeRegisterState);
            this.AccumulatorAndFlagsRegisters.ResetToState(state.AccumulatorAndFlagsRegisterState);

            this.StackPointer = state.StackPointer;
            this.ProgramCounter = state.ProgramCounter;

            this.InterruptFlipFlop1 = state.InterruptFlipFlop1;
            this.InterruptFlipFlop2 = state.InterruptFlipFlop2;
            this.InterruptMode = state.InterruptMode;
        }

        public Intel8080RegisterState GetRegisterState()
        {
            return new Intel8080RegisterState
                   {
                       AccumulatorAndFlagsRegisterState = this.AccumulatorAndFlagsRegisters.GetRegisterState(),
                       InterruptFlipFlop1 = this.InterruptFlipFlop1,
                       InterruptFlipFlop2 = this.InterruptFlipFlop2,
                       InterruptMode = this.InterruptMode,
                       GeneralPurposeRegisterState = this.GeneralPurposeRegisters.GetRegisterState(),
                       ProgramCounter = this.ProgramCounter,
                       StackPointer = this.StackPointer
                   };
        }
    }
}
