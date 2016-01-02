namespace Axh.Retro.CPU.Z80.Binding
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Common.Memory;
    using Axh.Retro.CPU.Z80.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core;
    using Axh.Retro.CPU.Z80.Core.DynaRec;
    using Axh.Retro.CPU.Z80.Core.Timing;
    using Axh.Retro.CPU.Z80.Memory;
    using Axh.Retro.CPU.Z80.Peripherals;
    using Axh.Retro.CPU.Z80.Registers;

    using Ninject;
    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;

    public class Z80Module<TRegisters, TRegisterState> : NinjectModule
        where TRegisterState : struct
        where TRegisters : IStateBackedRegisters<TRegisterState>
    {
                private readonly string cpuContextScope;

        public Z80Module(string cpuContextScope)
        {
            this.cpuContextScope = cpuContextScope;
        }

        public override void Load()
        {
            if (!this.Kernel.CanResolve<IRuntimeConfig>())
            {
                throw new Exception("Cannot resolve IRuntimeConfig");
            }

            if (!this.Kernel.CanResolve<IPlatformConfig>())
            {
                throw new Exception("Cannot resolve IPlatformConfig");
            }

            // Every time ninject resolves ICpuCore we get a brand new scope for everything in that named scope
            this.Bind<ICpuCore<TRegisters, TRegisterState>>().To<CachingCpuCore<TRegisters, TRegisterState>>().DefinesNamedScope(cpuContextScope);

            this.Bind<ICoreContext<TRegisters, TRegisterState>>().To<CoreContext<TRegisters, TRegisterState>>().InNamedScope(cpuContextScope);
            
            this.Kernel.Bind<IPeripheralManager>().To<PeripheralManager>().InNamedScope(cpuContextScope);
            this.Kernel.Bind<IMmu>().To<Z80Mmu<TRegisters>>().InNamedScope(cpuContextScope);
            this.Kernel.Bind<IPrefetchQueue>().To<PrefetchQueue>().InNamedScope(cpuContextScope);

            this.Kernel.Bind<IAlu>().To<Alu<TRegisters>>().InNamedScope(cpuContextScope);

            this.Bind<IInstructionBlockCache<TRegisters>>().To<InstructionBlockCache<TRegisters>>().InNamedScope(cpuContextScope);

            this.Kernel.Bind<IInterruptManager>().To<InterruptManager>().InNamedScope(cpuContextScope);
            this.Kernel.Bind<IInstructionTimer>().To<MachineCycleTimer>().InNamedScope(cpuContextScope);

            var runtimeConfig = Kernel.Get<IRuntimeConfig>();
            switch (runtimeConfig.CoreMode)
            {
                case CoreMode.Interpreted:
                    throw new NotImplementedException(runtimeConfig.CoreMode.ToString());
                case CoreMode.DynaRec:
                    this.Kernel.Bind<IInstructionBlockDecoder<TRegisters>>().To<DynaRec<TRegisters>>().InNamedScope(cpuContextScope);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(runtimeConfig.CoreMode), runtimeConfig.CoreMode, null);
            }
            
            // Registers
            this.Bind<IGeneralPurposeRegisterSet>().To<GeneralPurposeRegisterSet>().InTransientScope(); // Transient so never inject this except for into IRegisters classes
            this.Bind<IAccumulatorAndFlagsRegisterSet>().To<AccumulatorAndFlagsRegisterSet>().InTransientScope(); // Transient so never inject this except for into IRegisters classes

            var platformConfig = Kernel.Get<IPlatformConfig>();
            switch (platformConfig.CpuMode)
            {
                case CpuMode.Intel8080:
                    this.Bind<IFlagsRegister>().To<Intel8080FlagsRegister>().InTransientScope(); // Transient so never inject this except for into IAccumulatorAndFlagsRegisterSet classes
                    this.Bind<IRegisters, IIntel8080Registers>().To<Intel8080Registers>().InNamedScope(cpuContextScope);
                    break;
                case CpuMode.GameBoy:
                    this.Bind<IFlagsRegister>().To<GameBoyFlagsRegister>().InTransientScope(); // Transient so never inject this except for into IAccumulatorAndFlagsRegisterSet classes
                    this.Bind<IRegisters, IIntel8080Registers>().To<Intel8080Registers>().InNamedScope(cpuContextScope);
                    break;
                case CpuMode.Z80:
                    this.Bind<IFlagsRegister>().To<Intel8080FlagsRegister>().InTransientScope(); // Transient so never inject this except for into IAccumulatorAndFlagsRegisterSet classes
                    this.Bind<IRegisters, IZ80Registers>().To<Z80Registers>().InNamedScope(cpuContextScope);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platformConfig.CpuMode), platformConfig.CpuMode, null);
            }
        }
    }
}
