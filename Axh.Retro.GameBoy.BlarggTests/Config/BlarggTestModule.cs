namespace Axh.Retro.GameBoy.BlarggTests.Config
{
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Graphics;

    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;

    internal class BlarggTestModule : NinjectModule
    {
        private readonly string cpuContextScope;

        public BlarggTestModule(string cpuContextScope)
        {
            this.cpuContextScope = cpuContextScope;
        }

        public override void Load()
        {
            this.Kernel.Bind<IRenderHandler>().To<NullRenderHandler>().InNamedScope(cpuContextScope);
            this.Kernel.Bind<IGameBoyConfig>().To<BlarggTestGameBoyConfig>().InSingletonScope();
        }
    }
}
