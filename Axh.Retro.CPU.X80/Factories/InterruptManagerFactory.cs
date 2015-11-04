namespace Axh.Retro.CPU.X80.Factories
{
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Core;

    public class InterruptManagerFactory : IInterruptManagerFactory
    {
        public ICoreInterruptManager GetInterruptManager()
        {
            return new InterruptManager();
        }
    }
}
