using System.Threading;

namespace Axh.Retro.GameBoy.Wpf.Config
{
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Graphics;
    using Axh.Retro.GameBoy.Wpf;
    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;

    internal class GameBoyWpfModule : NinjectModule
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        private readonly string cpuContextScope;

        private readonly byte[] cartridge;

        public GameBoyWpfModule(CancellationTokenSource cancellationTokenSource, string cpuContextScope, byte[] cartridge)
        {
            this.cancellationTokenSource = cancellationTokenSource;
            this.cpuContextScope = cpuContextScope;
            this.cartridge = cartridge;
        }

        public override void Load()
        {
            this.Kernel.Bind<IRenderHandler>().To<SimpleLcd>().InNamedScope(cpuContextScope).WithConstructorArgument(cancellationTokenSource);
            this.Kernel.Bind<IGameBoyConfig>().To<StaticGameBoyConfig>().InSingletonScope().WithConstructorArgument(cartridge);
        }
    }
}
