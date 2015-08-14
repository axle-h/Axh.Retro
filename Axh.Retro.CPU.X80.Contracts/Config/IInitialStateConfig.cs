namespace Axh.Retro.CPU.X80.Contracts.Config
{
    using Axh.Retro.CPU.X80.Contracts.State;

    public interface IInitialStateConfig<out TRegisterState> where TRegisterState : struct
    {
        TRegisterState GetInitialRegisterState();
    }
}