namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    public interface IInitialStateFactory<out TRegisterState> where TRegisterState : struct
    {
        TRegisterState GetInitialRegisterState();
    }
}