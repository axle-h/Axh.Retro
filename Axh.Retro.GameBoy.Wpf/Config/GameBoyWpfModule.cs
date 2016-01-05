namespace Axh.Retro.GameBoy.Wpf.Config
{
    using Axh.Retro.GameBoy.Console.Config;
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Graphics;
    using Axh.Retro.GameBoy.Wpf;
    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;

    internal class GameBoyWpfModule : NinjectModule
    {
        private readonly string cpuContextScope;

        private readonly byte[] cartridge;

        public GameBoyWpfModule(string cpuContextScope, byte[] cartridge)
        {
            this.cpuContextScope = cpuContextScope;
            this.cartridge = cartridge;
        }

        public override void Load()
        {
            this.Kernel.Bind<IRenderHandler>().To<SimpleLcd>().InNamedScope(cpuContextScope);
            this.Kernel.Bind<IGameBoyConfig>().To<StaticGameBoyConfig>().InSingletonScope().WithConstructorArgument(cartridge);
        }
    }
}
