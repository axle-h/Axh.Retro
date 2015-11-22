namespace Axh.Retro.GameBoy.Console.Config
{
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Graphics;

    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;

    internal class GameBoyConsoleModule : NinjectModule
    {
        private readonly string cpuContextScope;

        public GameBoyConsoleModule(string cpuContextScope)
        {
            this.cpuContextScope = cpuContextScope;
        }

        public override void Load()
        {
            this.Kernel.Bind<IRenderHandler>().To<RenderHandler>().InNamedScope(cpuContextScope);
            this.Kernel.Bind<IGameBoyConfig>().To<GameBoyConfig>().InSingletonScope();
        }
    }
}
