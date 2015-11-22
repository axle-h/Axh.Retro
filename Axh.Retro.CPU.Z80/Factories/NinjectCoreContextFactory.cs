namespace Axh.Retro.CPU.Z80.Factories
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Common.Memory;
    using Axh.Retro.CPU.Z80.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core;
    using Axh.Retro.CPU.Z80.Core.DynaRec;
    using Axh.Retro.CPU.Z80.Core.Timing;
    using Axh.Retro.CPU.Z80.Memory;
    using Axh.Retro.CPU.Z80.Peripherals;
    using Axh.Retro.CPU.Z80.Registers;

    using Ninject;
    using Ninject.Modules;

    public class NinjectCoreContextFactory<TRegisters, TRegisterState> : ICoreContextFactory<TRegisters, TRegisterState>
        where TRegisterState : struct
        where TRegisters : IStateBackedRegisters<TRegisterState>
    {
        private readonly IRuntimeConfig runtimeConfig;

        private readonly IPlatformConfig platformConfig;

        private readonly IInitialStateFactory<TRegisterState> initialStateFactory;

        private readonly IPeripheralFactory peripheralFactory;

        public NinjectCoreContextFactory(IRuntimeConfig runtimeConfig, IPlatformConfig platformConfig, IInitialStateFactory<TRegisterState> initialStateFactory, IPeripheralFactory peripheralFactory)
        {
            this.runtimeConfig = runtimeConfig;
            this.platformConfig = platformConfig;
            this.initialStateFactory = initialStateFactory;
            this.peripheralFactory = peripheralFactory;
        }

        public ICoreContext<TRegisters, TRegisterState> GetContext()
        {
            // New kernel each context or instance of the emulator.
            var z80Module = new Z80Module(runtimeConfig, platformConfig, initialStateFactory, peripheralFactory);
            using (var kernel = new StandardKernel(z80Module))
            {
                return kernel.Get<ICoreContext<TRegisters, TRegisterState>>();
            }
        }

        private class Z80Module : NinjectModule
        {
            private readonly IRuntimeConfig runtimeConfig;
            private readonly IPlatformConfig platformConfig;
            private readonly TRegisterState initialState;
            private readonly IPeripheralFactory peripheralFactory;

            public Z80Module(IRuntimeConfig runtimeConfig, IPlatformConfig platformConfig, IInitialStateFactory<TRegisterState> initialStateFactory, IPeripheralFactory peripheralFactory)
            {
                this.runtimeConfig = runtimeConfig;
                this.platformConfig = platformConfig;
                this.initialState = initialStateFactory.GetInitialRegisterState();
                this.peripheralFactory = peripheralFactory;
            }

            public override void Load()
            {
                this.Kernel.Bind<IRuntimeConfig>().ToConstant(this.runtimeConfig).InSingletonScope();
                this.Kernel.Bind<IPlatformConfig>().ToConstant(this.platformConfig).InSingletonScope();
                this.Kernel.Bind<TRegisterState>().ToConstant(this.initialState).InSingletonScope();
                this.Kernel.Bind<IPeripheralFactory>().ToConstant(this.peripheralFactory).InSingletonScope();

                this.Kernel.Bind<IPeripheralManager>().To<PeripheralManager>().InSingletonScope();
                this.Kernel.Bind<IMmu>().To<Z80Mmu>().InSingletonScope();
                this.Kernel.Bind<IPrefetchQueue>().To<PrefetchQueue>().InSingletonScope();

                this.Kernel.Bind<IAlu>().To<Alu<TRegisters>>().InSingletonScope();

                this.Bind<IInstructionBlockCache<TRegisters>>().To<InstructionBlockCache<TRegisters>>().InSingletonScope();

                this.Kernel.Bind<IInterruptManager>().To<InterruptManager>().InSingletonScope();
                this.Kernel.Bind<IInstructionTimer>().To<MachineCycleTimer>().InSingletonScope();

                switch (this.runtimeConfig.CoreMode)
                {
                    case CoreMode.Interpreted:
                        throw new NotImplementedException(this.runtimeConfig.CoreMode.ToString());
                    case CoreMode.DynaRec:
                        this.Kernel.Bind<IInstructionBlockDecoder<TRegisters>>().To<DynaRec<TRegisters>>().InSingletonScope();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(this.runtimeConfig.CoreMode), this.runtimeConfig.CoreMode, null);
                }
                
                // Registers
                this.Bind<IGeneralPurposeRegisterSet>().To<GeneralPurposeRegisterSet>().InTransientScope();
                this.Bind<IAccumulatorAndFlagsRegisterSet>().To<AccumulatorAndFlagsRegisterSet>().InTransientScope();
                switch (this.platformConfig.CpuMode)
                {
                    case CpuMode.Intel8080:
                        this.Bind<IFlagsRegister>().To<Intel8080FlagsRegister>().InTransientScope();
                        this.Bind<IIntel8080Registers>().To<Intel8080Registers>().InSingletonScope();
                        break;
                    case CpuMode.GameBoy:
                        this.Bind<IFlagsRegister>().To<GameBoyFlagsRegister>().InTransientScope();
                        this.Bind<IIntel8080Registers>().To<Intel8080Registers>().InSingletonScope();
                        break;
                    case CpuMode.Z80:
                        this.Bind<IFlagsRegister>().To<Intel8080FlagsRegister>().InTransientScope();
                        this.Bind<IZ80Registers>().To<Z80Registers>().InSingletonScope();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.Bind<ICoreContext<TRegisters, TRegisterState>>().To<CoreContext<TRegisters, TRegisterState>>().InSingletonScope();
            }
        }
    }
}
