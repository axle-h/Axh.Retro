namespace Axh.Retro.GameBoy.Binding
{
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Factories;
    using Axh.Retro.GameBoy.Factories;

    using Ninject.Modules;

    public class GameBoyModule : NinjectModule
    {
        private readonly IGameBoyConfig config;

        public GameBoyModule(IGameBoyConfig config)
        {
            this.config = config;
        }

        public override void Load()
        {
            this.Bind<ICartridgeFactory>().To<CartridgeFactory>();
            this.Bind<IGameBoyConfig>().ToConstant(config);
        }
    }
}
