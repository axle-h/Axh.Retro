using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
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