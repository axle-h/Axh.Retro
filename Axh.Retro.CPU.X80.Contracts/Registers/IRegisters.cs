namespace Axh.Retro.CPU.X80.Contracts.Registers
{
    public interface IRegisters
    {
        IGeneralPurposeRegisterSet GeneralPurposeRegisters { get; }

        ushort StackPointer { get; set; }

        ushort ProgramCounter { get; set; }

        bool InterruptFlipFlop1 { get; set; }

        bool InterruptFlipFlop2 { get; set; }

        InterruptMode InterruptMode { get; set; }
    }
}
