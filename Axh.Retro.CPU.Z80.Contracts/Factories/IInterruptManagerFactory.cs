namespace Axh.Retro.CPU.Z80.Contracts.Factories
{
    using Axh.Retro.CPU.Z80.Contracts.Core;

    public interface IInterruptManagerFactory
    {
        ICoreInterruptManager GetInterruptManager();
    }
}