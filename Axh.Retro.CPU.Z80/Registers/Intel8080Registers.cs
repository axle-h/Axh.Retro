using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Registers
{
    public class Intel8080Registers : IIntel8080Registers
    {
        public Intel8080Registers(IGeneralPurposeRegisterSet generalPurposeRegisters,
            IAccumulatorAndFlagsRegisterSet accumulatorAndFlagsRegisters,
            IInitialStateFactory<Intel8080RegisterState> initialStateFactory)
        {
            GeneralPurposeRegisters = generalPurposeRegisters;
            AccumulatorAndFlagsRegisters = accumulatorAndFlagsRegisters;
            ResetToState(initialStateFactory.GetInitialRegisterState());
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
            GeneralPurposeRegisters.Reset();
            AccumulatorAndFlagsRegisters.Reset();

            StackPointer = 0;
            ProgramCounter = 0;
            InterruptFlipFlop1 = InterruptFlipFlop2 = false;
            InterruptMode = InterruptMode.InterruptMode0;
        }

        public void ResetToState(Intel8080RegisterState state)
        {
            GeneralPurposeRegisters.ResetToState(state.GeneralPurposeRegisterState);
            AccumulatorAndFlagsRegisters.ResetToState(state.AccumulatorAndFlagsRegisterState);

            StackPointer = state.StackPointer;
            ProgramCounter = state.ProgramCounter;

            InterruptFlipFlop1 = state.InterruptFlipFlop1;
            InterruptFlipFlop2 = state.InterruptFlipFlop2;
            InterruptMode = state.InterruptMode;
        }

        public Intel8080RegisterState GetRegisterState()
        {
            return new Intel8080RegisterState
                   {
                       AccumulatorAndFlagsRegisterState = AccumulatorAndFlagsRegisters.GetRegisterState(),
                       InterruptFlipFlop1 = InterruptFlipFlop1,
                       InterruptFlipFlop2 = InterruptFlipFlop2,
                       InterruptMode = InterruptMode,
                       GeneralPurposeRegisterState = GeneralPurposeRegisters.GetRegisterState(),
                       ProgramCounter = ProgramCounter,
                       StackPointer = StackPointer
                   };
        }
    }
}