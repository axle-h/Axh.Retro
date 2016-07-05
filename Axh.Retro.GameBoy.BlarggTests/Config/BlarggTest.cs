using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Graphics;
using DryIoc;

namespace Axh.Retro.GameBoy.BlarggTests.Config
{
    /// <summary>
    /// Registers the Blargg test config and null render handler.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Wiring.IZ80Module" />
    internal class BlarggTest : IZ80Module
    {
        /// <summary>
        /// Registers all hardware in this module.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="scope">The reuse scope.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Register(IContainer container, IReuse scope)
        {
            container.Register<IRenderHandler, NullRenderHandler>(Reuse.Singleton);
            container.Register<IGameBoyConfig, BlarggTestGameBoyConfig>(Reuse.Singleton);

            Config = container.Resolve<IGameBoyConfig>() as BlarggTestGameBoyConfig;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public BlarggTestGameBoyConfig Config { get; private set; }
    }
}