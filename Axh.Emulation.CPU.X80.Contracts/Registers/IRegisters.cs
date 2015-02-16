namespace Axh.Emulation.CPU.X80.Contracts.Registers
{
    using Axh.Emulation.CPU.X80.Contracts.State;

    public interface IRegisters
    {
        IGeneralPurposeRegisterSet GeneralPurposeRegisters { get; }
        
        ushort StackPointer { get; set; }

        ushort ProgramCounter { get; set; }

        void Reset();

        void ResetToState(RegisterState state);
        RegisterState GetRegisterState();
    }
}
