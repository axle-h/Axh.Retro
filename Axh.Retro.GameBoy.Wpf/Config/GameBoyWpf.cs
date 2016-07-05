
using System.Threading;
using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Graphics;
using DryIoc;

namespace Axh.Retro.GameBoy.Wpf.Config
{
    public class GameBoyWpf : IZ80Module
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        private readonly byte[] cartridge;

        public GameBoyWpf(CancellationTokenSource cancellationTokenSource, byte[] cartridge)
        {
            this.cancellationTokenSource = cancellationTokenSource;
            this.cartridge = cartridge;
        }

        /// <summary>
        /// Registers all hardware in this module.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="reuse">The reuse scope.</param>
        public void Register(IContainer container, IReuse reuse)
        {
            container.RegisterInstance(cancellationTokenSource, serviceKey: nameof(cancellationTokenSource));
            container.RegisterInstance(cartridge, serviceKey: nameof(cartridge));

            container.Register<IRenderHandler, SimpleLcd>(reuse, Parameters.Of.Type<CancellationTokenSource>(serviceKey: nameof(cancellationTokenSource)));
            container.Register<IGameBoyConfig, StaticGameBoyConfig>(Reuse.Singleton, Parameters.Of.Type<byte[]>(serviceKey: nameof(cartridge)));
        }
    }
}
