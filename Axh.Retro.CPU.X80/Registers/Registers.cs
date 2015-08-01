using Axh.Retro.CPU.X80.Contracts.Registers;
using Axh.Retro.CPU.X80.Contracts.State;

namespace Axh.Retro.CPU.X80.Registers
{
    using Retro.CPU.X80.Contracts.Registers;
    using Retro.CPU.X80.Contracts.State;

    public class Registers : IRegisters
    {
        public Registers(IGeneralPurposeRegisterSet registerSet)
        {
            this.GeneralPurposeRegisters = registerSet;
            this.Reset();
        }

        public IGeneralPurposeRegisterSet GeneralPurposeRegisters { get; private set; }

        //Pointers
        public ushort StackPointer { get; set; }
        public ushort ProgramCounter { get; set; }
        
        public void Reset()
        {
            this.GeneralPurposeRegisters.Reset();
            StackPointer = 0;
            ProgramCounter = 0;
        }

        public void ResetToState(RegisterState state)
        {
            this.GeneralPurposeRegisters.ResetToState(state.GeneralPurposeRegisterState);
            this.StackPointer = state.StackPointer;
            this.ProgramCounter = state.ProgramCounter;
        }

        public RegisterState GetRegisterState()
        {
            return new RegisterState
            {
                GeneralPurposeRegisterState = this.GeneralPurposeRegisters.GetRegisterState(),
                ProgramCounter = this.ProgramCounter,
                StackPointer = this.StackPointer
            };
        }
    }
}
