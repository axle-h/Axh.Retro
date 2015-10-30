namespace Axh.Retro.Z80Console.Infrastructure
{
    using Axh.Retro.CPU.X80.Cache;
    using Axh.Retro.CPU.X80.Contracts.Cache;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.IO;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Contracts.State;
    using Axh.Retro.CPU.X80.Core;
    using Axh.Retro.CPU.X80.Core.DynaRec;
    using Axh.Retro.CPU.X80.Factories;
    using Axh.Retro.CPU.X80.IO;
    using Axh.Retro.Z80Console.Config;

    using Ninject.Modules;

    internal class Z80Module : NinjectModule
    {
        public override void Load()
        {
            // Core
            this.Bind<ICpuCore>().To<CachingCpuCore<IZ80Registers>>();
            this.Bind<IInstructionBlockDecoder<IZ80Registers>>().To<DynaRecInstructionBlockDecoder<IZ80Registers>>();

            // Cache
            this.Bind<IInstructionBlockCache<IZ80Registers>>().To<NaiveInstructionBlockCache<IZ80Registers>>();

            // IO
            this.Bind<IInputOutputManager>().To<InputOutputManager>();
            this.Bind<IPeripheral>().To<AsciiSystemConsole>().WithConstructorArgument("port", (byte)0);
            this.Bind<IPeripheral>().To<SystemConsoleStatus>().WithConstructorArgument("port", (byte)1);

            // Factories
            this.Bind<IRegisterFactory<IZ80Registers>>().To<Z80RegisterFactory>().InSingletonScope();
            this.Bind<IMmuFactory>().To<MmuFactory>().InSingletonScope();
            this.Bind<IAluFactory>().To<AluFactory>();

            // Config
            this.Bind<IInitialStateConfig<Z80RegisterState>>().To<Z80InitialStateConfig>().InSingletonScope();
            this.Bind<IPlatformConfig>().To<Z8064KBootstrappedConfig>().InSingletonScope();


        }
    }
}
