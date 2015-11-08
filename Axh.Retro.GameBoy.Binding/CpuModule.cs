namespace Axh.Retro.GameBoy.Binding
{
    using Axh.Retro.CPU.Z80.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.CPU.Z80.Core;
    using Axh.Retro.CPU.Z80.Core.DynaRec;
    using Axh.Retro.CPU.Z80.Core.Timing;
    using Axh.Retro.CPU.Z80.Factories;
    using Axh.Retro.GameBoy.Config;
    using Axh.Retro.GameBoy.Factories;

    using Ninject.Modules;

    public class CpuModule : NinjectModule
    {
        public override void Load()
        {
            // Core
            this.Bind<ICpuCore<IIntel8080Registers, Intel8080RegisterState>>().To<CachingCpuCore<IIntel8080Registers, Intel8080RegisterState>>();
            
            // Factories
            this.Bind<IPeripheralFactory>().To<PeripheralFactory>().InSingletonScope();
            this.Bind<ICoreContextFactory<IIntel8080Registers, Intel8080RegisterState>>().To<Intel8080ContextFactory>().InSingletonScope();

            // Config
            this.Bind<IInitialStateConfig<Intel8080RegisterState>>().To<GameBoyInitialStateConfig>().InSingletonScope();
            this.Bind<IPlatformConfig>().To<GameBoyPlatformConfig>().InSingletonScope();
            this.Bind<IRuntimeConfig>().To<GameBoyRuntimeConfig>().InSingletonScope();
        }
    }
}
