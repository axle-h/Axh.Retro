namespace Axh.Emulation.CPU.X80.Contracts.Config
{
    using Axh.Emulation.CPU.X80.Contracts.State;

    public interface IInitialStateConfig
    {
        Z80RegisterState GetInitialRegisterState();
    }
}