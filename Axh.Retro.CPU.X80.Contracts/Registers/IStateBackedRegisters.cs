namespace Axh.Retro.CPU.X80.Contracts.Registers
{
    public interface IStateBackedRegisters<TRegisterState> where TRegisterState : struct
    {
        void Reset();

        void ResetToState(TRegisterState state);

        TRegisterState GetRegisterState();
    }
}
