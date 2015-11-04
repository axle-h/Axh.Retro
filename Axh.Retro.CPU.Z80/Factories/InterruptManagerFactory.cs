namespace Axh.Retro.CPU.Z80.Factories
{
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Core;

    public class InterruptManagerFactory : IInterruptManagerFactory
    {
        public ICoreInterruptManager GetInterruptManager()
        {
            return new InterruptManager();
        }
    }
}
