namespace Axh.Emulation.CPU.X80.Contracts.Registers
{
    using Axh.Emulation.CPU.X80.Contracts.State;

    public interface IZ80Registers
    {
        IGeneralPurposeRegisterSet GeneralPurposeRegisters { get; }

        ushort IX { get; set; }

        ushort IY { get; set; }

        byte I { get; set; }

        byte R { get; set; }

        ushort StackPointer { get; set; }

        ushort ProgramCounter { get; set; }

        bool InterruptFlipFlop1 { get; set; }

        bool InterruptFlipFlop2 { get; set; }

        InterruptMode InterruptMode { get; set; }

        void SwitchToAlternativeGeneralPurposeRegisters();

        void Reset();

        void ResetToState(Z80RegisterState state);
        Z80RegisterState GetRegisterState();
    }
}