namespace Axh.Retro.CPU.X80.Contracts.Registers
{
    using Axh.Retro.CPU.X80.Contracts.State;

    public interface IGeneralPurposeRegisterSet
    {
        byte B { get; set; }
        byte C { get; set; }
        byte D { get; set; }
        byte E { get; set; }
        byte H { get; set; }
        byte L { get; set; }
        ushort BC { get; set; }
        ushort DE { get; set; }
        ushort HL { get; set; }

        void Reset();

        void ResetToState(GeneralPurposeRegisterState state);
        GeneralPurposeRegisterState GetRegisterState();
    }
}