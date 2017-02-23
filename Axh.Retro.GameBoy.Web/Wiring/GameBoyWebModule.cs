using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Graphics;
using DryIoc;

namespace Axh.Retro.GameBoy.Web.Wiring
{
    public class GameBoyWebModule : IZ80Module
    {
        private readonly byte[] _cartridge;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameBoyWebModule"/> class.
        /// </summary>
        /// <param name="cartridge">The cartridge.</param>
        public GameBoyWebModule(byte[] cartridge)
        {
            _cartridge = cartridge;
        }

        /// <summary>
        /// Registers all hardware in this module.
        /// </summary>
        /// <param name="container">The container.</param>
        public void Register(IContainer container)
        {
            container.RegisterInstance(_cartridge, serviceKey: nameof(_cartridge));
            container.Register<IGameBoyConfig, StaticGameBoyConfig>(Reuse.Singleton,
                                                                    Parameters.Of.Type<byte[]>(serviceKey: nameof(_cartridge)));

            container.Register<IRenderHandler, SignalRRenderHandler>();
        }
    }
}