using Axh.Retro.CPU.X80.Contracts.State;

namespace Axh.Retro.CPU.X80.Contracts.Config
{
    using Retro.CPU.X80.Contracts.State;

    public interface IInitialStateConfig
    {
        Z80RegisterState GetInitialRegisterState();
    }
}