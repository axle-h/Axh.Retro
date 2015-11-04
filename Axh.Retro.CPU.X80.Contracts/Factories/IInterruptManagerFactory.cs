namespace Axh.Retro.CPU.X80.Contracts.Factories
{
    using Axh.Retro.CPU.X80.Contracts.Core;

    public interface IInterruptManagerFactory
    {
        ICoreInterruptManager GetInterruptManager();
    }
}