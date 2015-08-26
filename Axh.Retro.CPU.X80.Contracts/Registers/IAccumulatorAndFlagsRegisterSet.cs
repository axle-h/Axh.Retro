namespace Axh.Retro.CPU.X80.Contracts.Registers
{
    using Axh.Retro.CPU.X80.Contracts.State;

    public interface IAccumulatorAndFlagsRegisterSet
    {
        byte A { get; set; }
        IFlagsRegister Flags { get; }
        ushort AF { get; set; }

        void Reset();

        void ResetToState(AccumulatorAndFlagsRegisterState state);
        AccumulatorAndFlagsRegisterState GetRegisterState();
    }
}
