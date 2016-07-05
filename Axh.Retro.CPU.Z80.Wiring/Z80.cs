using System;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Common.Memory;
using Axh.Retro.CPU.Z80.Cache;
using Axh.Retro.CPU.Z80.Contracts.Cache;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Core;
using Axh.Retro.CPU.Z80.Core.DynaRec;
using Axh.Retro.CPU.Z80.Memory;
using Axh.Retro.CPU.Z80.Peripherals;
using Axh.Retro.CPU.Z80.Registers;
using Axh.Retro.CPU.Z80.Timing;
using DryIoc;

namespace Axh.Retro.CPU.Z80.Wiring
{
    public class Z80<TRegisters, TRegisterState> : IDisposable where TRegisters : IStateBackedRegisters<TRegisterState>
                                                               where TRegisterState : struct
    {
        private readonly IContainer container;
        private readonly IReuse reuse = Reuse.InResolutionScopeOf(typeof(ICpuCore<TRegisters, TRegisterState>));
        private bool isInitialized;
        
        public Z80()
        {
            container = new Container(rules => rules.WithDefaultReuseInsteadOfTransient(reuse));
        }

        public Z80<TRegisters, TRegisterState> With(IZ80Module module)
        {
            module.Register(container, reuse);
            return this;
        }

        public Z80<TRegisters, TRegisterState> With<TModule>() where TModule : IZ80Module, new()
        {
            new TModule().Register(container, reuse);
            return this;
        }

        public ICpuCore<TRegisters, TRegisterState> GetNewCore()
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException($"Must call {nameof(Init)} first");
            }

            return container.Resolve<ICpuCore<TRegisters, TRegisterState>>();
        }

        public Z80<TRegisters, TRegisterState> Init()
        {
            if (!container.IsRegistered<IRuntimeConfig>())
            {
                throw new Exception($"Cannot resolve {nameof(IRuntimeConfig)}");
            }

            if (!container.IsRegistered<IPlatformConfig>())
            {
                throw new Exception($"Cannot resolve {nameof(IPlatformConfig)}");
            }

            container.Register<ICpuCore<TRegisters, TRegisterState>, CachingCpuCore<TRegisters, TRegisterState>>(reuse);
            
            container.Register<ICoreContext<TRegisters, TRegisterState>, CoreContext<TRegisters, TRegisterState>>(reuse);

            container.Register<IPeripheralManager, PeripheralManager>(reuse);
            container.Register<IMmu, Z80Mmu<TRegisters>>(reuse);
            container.Register<IPrefetchQueue, PrefetchQueue>(reuse);

            container.Register<IAlu, Alu<TRegisters>>(reuse);

            container.Register<IInstructionBlockCache<TRegisters>, InstructionBlockCache<TRegisters>>(reuse);

            container.Register<IInterruptManager, InterruptManager>(reuse);
            container.Register<IInstructionTimer, MachineCycleTimer>(reuse);
            container.Register<IDmaController, DmaController>(reuse);

            var runtimeConfig = container.Resolve<IRuntimeConfig>();
            switch (runtimeConfig.CoreMode)
            {
                case CoreMode.Interpreted:
                    throw new NotImplementedException(runtimeConfig.CoreMode.ToString());
                case CoreMode.DynaRec:
                    container.Register<IInstructionBlockDecoder<TRegisters>, DynaRec<TRegisters>>(reuse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(runtimeConfig.CoreMode), runtimeConfig.CoreMode, null);
            }

            // Registers
            container.Register<IGeneralPurposeRegisterSet, GeneralPurposeRegisterSet>(reuse);
            container.Register<IAccumulatorAndFlagsRegisterSet, AccumulatorAndFlagsRegisterSet>(reuse);

            var platformConfig = container.Resolve<IPlatformConfig>();
            switch (platformConfig.CpuMode)
            {
                case CpuMode.Intel8080:
                    container.Register<IFlagsRegister, Intel8080FlagsRegister>(reuse);
                    container.RegisterMany(new [] { typeof(IIntel8080Registers), typeof(IRegisters) }, typeof(Intel8080Registers), reuse);
                    break;
                case CpuMode.GameBoy:
                    container.Register<IFlagsRegister, GameBoyFlagsRegister>(reuse);
                    container.RegisterMany(new[] { typeof(IIntel8080Registers), typeof(IRegisters) }, typeof(Intel8080Registers), reuse);
                    break;
                case CpuMode.Z80:
                    container.Register<IFlagsRegister, Intel8080FlagsRegister>(reuse);
                    container.RegisterMany(new[] { typeof(IIntel8080Registers), typeof(IRegisters) }, typeof(Z80Registers), reuse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platformConfig.CpuMode), platformConfig.CpuMode, null);
            }

            isInitialized = true;
            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => container.Dispose();
    }
}
