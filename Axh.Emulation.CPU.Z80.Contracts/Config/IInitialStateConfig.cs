namespace Axh.Emulation.CPU.Z80.Contracts.Config
{
    using Axh.Emulation.CPU.Z80.Contracts.State;

    public interface IInitialStateConfig
    {
        Z80RegisterState GetInitialRegisterState();
    }
}