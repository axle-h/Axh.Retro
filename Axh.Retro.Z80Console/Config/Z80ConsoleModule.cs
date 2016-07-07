using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.CPU.Z80.Peripherals;
using Axh.Retro.CPU.Z80.Wiring;
using DryIoc;

namespace Axh.Retro.Z80Console.Config
{
    internal class Z80ConsoleModule : IZ80Module
    {
        /// <summary>
        /// Registers all hardware in this module.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Register(IContainer container)
        {
            // Z80 Console specific
            container.Register<IPeripheral, AsciiSystemConsole>(Reuse.Singleton);
            container.Register<IPeripheral, SystemConsoleStatus>(Reuse.Singleton);

            container.Register<IInitialStateFactory<Z80RegisterState>, Z80InitialStateFactory>(Reuse.Singleton);
            container.Register<IPlatformConfig, Z8064KBootstrappedConfig>(Reuse.Singleton);
            container.Register<IRuntimeConfig, RuntimeConfig>(Reuse.Singleton);
        }
    }
}