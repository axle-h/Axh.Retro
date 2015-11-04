namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    public interface IInitialStateConfig<out TRegisterState> where TRegisterState : struct
    {
        TRegisterState GetInitialRegisterState();
    }
}