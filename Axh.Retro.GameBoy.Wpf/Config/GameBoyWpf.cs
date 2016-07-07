using System.Threading;
using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Graphics;
using DryIoc;

namespace Axh.Retro.GameBoy.Wpf.Config
{
    public class GameBoyWpf : IZ80Module
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly byte[] _cartridge;

        public GameBoyWpf(CancellationTokenSource cancellationTokenSource, byte[] cartridge)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _cartridge = cartridge;
        }

        /// <summary>
        /// Registers all hardware in this module.
        /// </summary>
        /// <param name="container">The container.</param>
        public void Register(IContainer container)
        {
            container.RegisterInstance(_cancellationTokenSource, serviceKey: nameof(_cancellationTokenSource));
            container.RegisterInstance(_cartridge, serviceKey: nameof(_cartridge));

            container.Register<IRenderHandler, SimpleLcd>(
                                                          made:
                                                              Parameters.Of.Type<CancellationTokenSource>(
                                                                                                          serviceKey:
                                                                                                              nameof(
                                                                                                                     _cancellationTokenSource)));
            container.Register<IGameBoyConfig, StaticGameBoyConfig>(Reuse.Singleton,
                                                                    Parameters.Of.Type<byte[]>(serviceKey: nameof(_cartridge)));
        }
    }
}