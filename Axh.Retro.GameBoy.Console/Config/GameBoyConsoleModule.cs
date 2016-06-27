using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;

namespace Axh.Retro.GameBoy.Console.Config
{
    internal class GameBoyConsoleModule : NinjectModule
    {
        private readonly byte[] cartridge;
        private readonly string cpuContextScope;

        public GameBoyConsoleModule(string cpuContextScope, byte[] cartridge)
        {
            this.cpuContextScope = cpuContextScope;
            this.cartridge = cartridge;
        }

        public override void Load()
        {
            Kernel.Bind<IRenderHandler>().To<RenderHandler>().InNamedScope(cpuContextScope);
            Kernel.Bind<IGameBoyConfig>()
                  .To<StaticGameBoyConfig>()
                  .InSingletonScope()
                  .WithConstructorArgument(cartridge);
        }
    }
}