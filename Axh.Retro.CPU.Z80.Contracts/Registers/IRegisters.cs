namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    public interface IRegisters
    {
        IGeneralPurposeRegisterSet GeneralPurposeRegisters { get; }

        IAccumulatorAndFlagsRegisterSet AccumulatorAndFlagsRegisters { get; }

        ushort StackPointer { get; set; }

        ushort ProgramCounter { get; set; }

        bool InterruptFlipFlop1 { get; set; }

        bool InterruptFlipFlop2 { get; set; }

        InterruptMode InterruptMode { get; set; }
    }
}