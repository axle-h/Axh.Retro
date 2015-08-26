namespace Axh.Retro.CPU.X80.Contracts.Registers
{
    using Axh.Retro.CPU.X80.Contracts.State;

    public interface IZ80Registers : IRegisters, IStateBackedRegisters<Z80RegisterState>
    {
        ushort IX { get; set; }

        ushort IY { get; set; }

        byte I { get; set; }

        byte R { get; set; }
        
        void SwitchToAlternativeGeneralPurposeRegisters();
        void SwitchToAlternativeAccumulatorAndFlagsRegisters();
    }
}