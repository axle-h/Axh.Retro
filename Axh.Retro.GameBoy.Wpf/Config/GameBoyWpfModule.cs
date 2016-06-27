using System.Threading;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;

namespace Axh.Retro.GameBoy.Wpf.Config
{
    internal class GameBoyWpfModule : NinjectModule
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        private readonly byte[] cartridge;

        private readonly string cpuContextScope;

        public GameBoyWpfModule(CancellationTokenSource cancellationTokenSource,
                                string cpuContextScope,
                                byte[] cartridge)
        {
            this.cancellationTokenSource = cancellationTokenSource;
            this.cpuContextScope = cpuContextScope;
            this.cartridge = cartridge;
        }

        public override void Load()
        {
            Kernel.Bind<IRenderHandler>()
                  .To<SimpleLcd>()
                  .InNamedScope(cpuContextScope)
                  .WithConstructorArgument(cancellationTokenSource);
            Kernel.Bind<IGameBoyConfig>()
                  .To<StaticGameBoyConfig>()
                  .InSingletonScope()
                  .WithConstructorArgument(cartridge);
        }
    }
}