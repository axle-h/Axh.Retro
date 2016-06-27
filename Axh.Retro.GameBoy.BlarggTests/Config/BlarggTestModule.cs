using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;

namespace Axh.Retro.GameBoy.BlarggTests.Config
{
    internal class BlarggTestModule : NinjectModule
    {
        private readonly string cpuContextScope;

        public BlarggTestModule(string cpuContextScope)
        {
            this.cpuContextScope = cpuContextScope;
        }

        public override void Load()
        {
            Kernel.Bind<IRenderHandler>().To<NullRenderHandler>().InNamedScope(cpuContextScope);
            Kernel.Bind<IGameBoyConfig>().To<BlarggTestGameBoyConfig>().InSingletonScope();
        }
    }
}