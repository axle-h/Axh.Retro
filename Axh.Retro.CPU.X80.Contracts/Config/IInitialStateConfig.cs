namespace Axh.Retro.CPU.X80.Contracts.Config
{
    public interface IInitialStateConfig<out TRegisterState> where TRegisterState : struct
    {
        TRegisterState GetInitialRegisterState();
    }
}