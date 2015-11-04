namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    public interface IStateBackedRegisters<TRegisterState> where TRegisterState : struct
    {
        void Reset();

        void ResetToState(TRegisterState state);

        TRegisterState GetRegisterState();
    }
}
