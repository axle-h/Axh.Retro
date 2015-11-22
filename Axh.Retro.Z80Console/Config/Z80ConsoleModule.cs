namespace Axh.Retro.Z80Console.Infrastructure
{
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.CPU.Z80.Peripherals;
    using Axh.Retro.Z80Console.Config;

    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;

    internal class Z80ConsoleModule : NinjectModule
    {
        private readonly string cpuContextScope;

        public Z80ConsoleModule(string cpuContextScope)
        {
            this.cpuContextScope = cpuContextScope;
        }

        public override void Load()
        {
            // Z80 Console specific
            Bind<IIOPeripheral>().To<AsciiSystemConsole>().InNamedScope(cpuContextScope);
            Bind<IIOPeripheral>().To<SystemConsoleStatus>().InNamedScope(cpuContextScope);

            Bind<IInitialStateFactory<Z80RegisterState>>().To<Z80InitialStateFactory>().InSingletonScope();
            Bind<IPlatformConfig>().To<Z8064KBootstrappedConfig>().InSingletonScope();
            Bind<IRuntimeConfig>().To<RuntimeConfig>().InSingletonScope();
        }
    }
}