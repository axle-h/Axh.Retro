namespace Axh.Retro.Z80Console.Infrastructure
{
    using Axh.Retro.CPU.X80.Cache;
    using Axh.Retro.CPU.X80.Contracts.Cache;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Core.Timing;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Contracts.State;
    using Axh.Retro.CPU.X80.Core;
    using Axh.Retro.CPU.X80.Core.DynaRec;
    using Axh.Retro.CPU.X80.Core.Timing;
    using Axh.Retro.CPU.X80.Factories;
    using Axh.Retro.Z80Console.Config;
    using Axh.Retro.Z80Console.Factories;

    using Ninject.Modules;

    internal class Z80Module : NinjectModule
    {
        public override void Load()
        {
            // Core
            this.Bind<ICpuCore>().To<CachingCpuCore<IZ80Registers>>();
            this.Bind<IInstructionBlockDecoder<IZ80Registers>>().To<DynaRecInstructionBlockDecoder<IZ80Registers>>();
            this.Bind<IInstructionTimer>().To<MachineCycleTimer>();

            // Cache
            this.Bind<IInstructionBlockCache<IZ80Registers>>().To<InstructionBlockCache<IZ80Registers>>();
            
            // Factories
            this.Bind<IRegisterFactory<IZ80Registers>>().To<Z80RegisterFactory>().InSingletonScope();
            this.Bind<IMmuFactory>().To<MmuFactory>().InSingletonScope();
            this.Bind<IAluFactory>().To<AluFactory>().InSingletonScope();
            this.Bind<IPeripheralManagerFactory>().To<PeripheralManagerFactory>().InSingletonScope();
            this.Bind<IPeripheralFactory>().To<PeripheralFactory>().InSingletonScope();
            this.Bind<IInterruptManagerFactory>().To<InterruptManagerFactory>().InSingletonScope();

            // Config
            this.Bind<IInitialStateConfig<Z80RegisterState>>().To<Z80InitialStateConfig>().InSingletonScope();
            this.Bind<IPlatformConfig>().To<Z8064KBootstrappedConfig>().InSingletonScope();
            this.Bind<IRuntimeConfig>().To<RuntimeConfig>().InSingletonScope();
        }
    }
}
