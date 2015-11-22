namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    using Axh.Retro.CPU.Z80.Contracts.State;

    public interface IZ80Registers : IStateBackedRegisters<Z80RegisterState>
    {
        ushort IX { get; set; }

        ushort IY { get; set; }

        byte IXl { get; set; }

        byte IXh { get; set; }

        byte IYl { get; set; }

        byte IYh { get; set; }

        byte I { get; set; }

        byte R { get; set; }
        
        void SwitchToAlternativeGeneralPurposeRegisters();
        void SwitchToAlternativeAccumulatorAndFlagsRegisters();
    }
}