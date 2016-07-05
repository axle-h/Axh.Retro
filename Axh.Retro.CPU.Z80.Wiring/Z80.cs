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
        private readonly IContainer _container;
        private bool _isInitialized;

        public Z80()
        {
            _container =
                new Container(
                    rules =>
                        rules.WithDefaultReuseInsteadOfTransient(
                            Reuse.InResolutionScopeOf(typeof (ICpuCore<TRegisters, TRegisterState>)))
                            .WithoutThrowOnRegisteringDisposableTransient()); // I've implemented a proper dispose chain through object graph from ICpuCore.
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => _container.Dispose();

        public Z80<TRegisters, TRegisterState> With(IZ80Module module)
        {
            module.Register(_container);
            return this;
        }

        public Z80<TRegisters, TRegisterState> With<TModule>() where TModule : IZ80Module, new()
        {
            new TModule().Register(_container);
            return this;
        }

        public ICpuCore<TRegisters, TRegisterState> GetNewCore()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"Must call {nameof(Init)} first");
            }

            return _container.Resolve<ICpuCore<TRegisters, TRegisterState>>();
        }

        public Z80<TRegisters, TRegisterState> Init()
        {
            if (!_container.IsRegistered<IRuntimeConfig>())
            {
                throw new Exception($"Cannot resolve {nameof(IRuntimeConfig)}");
            }

            if (!_container.IsRegistered<IPlatformConfig>())
            {
                throw new Exception($"Cannot resolve {nameof(IPlatformConfig)}");
            }

            _container.Register<ICpuCore<TRegisters, TRegisterState>, CachingCpuCore<TRegisters, TRegisterState>>();

            _container.Register<ICoreContext<TRegisters, TRegisterState>, CoreContext<TRegisters, TRegisterState>>();

            _container.Register<IPeripheralManager, PeripheralManager>();
            _container.Register<IMmu, Z80Mmu<TRegisters>>();
            _container.Register<IPrefetchQueue, PrefetchQueue>();

            _container.Register<IAlu, Alu<TRegisters>>();

            _container.Register<IInstructionBlockCache<TRegisters>, InstructionBlockCache<TRegisters>>();

            _container.Register<IInterruptManager, InterruptManager>();
            _container.Register<IInstructionTimer, MachineCycleTimer>();
            _container.Register<IDmaController, DmaController>();

            var runtimeConfig = _container.Resolve<IRuntimeConfig>();
            switch (runtimeConfig.CoreMode)
            {
                case CoreMode.Interpreted:
                    throw new NotImplementedException(runtimeConfig.CoreMode.ToString());
                case CoreMode.DynaRec:
                    _container.Register<IInstructionBlockDecoder<TRegisters>, DynaRec<TRegisters>>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(runtimeConfig.CoreMode), runtimeConfig.CoreMode, null);
            }

            // Registers
            _container.Register<IGeneralPurposeRegisterSet, GeneralPurposeRegisterSet>();
            _container.Register<IAccumulatorAndFlagsRegisterSet, AccumulatorAndFlagsRegisterSet>();

            var platformConfig = _container.Resolve<IPlatformConfig>();
            switch (platformConfig.CpuMode)
            {
                case CpuMode.Intel8080:
                    _container.Register<IFlagsRegister, Intel8080FlagsRegister>();
                    _container.RegisterMany(new[] {typeof (IIntel8080Registers), typeof (IRegisters)},
                        typeof (Intel8080Registers));
                    break;
                case CpuMode.GameBoy:
                    _container.Register<IFlagsRegister, GameBoyFlagsRegister>();
                    _container.RegisterMany(new[] {typeof (IIntel8080Registers), typeof (IRegisters)},
                        typeof (Intel8080Registers));
                    break;
                case CpuMode.Z80:
                    _container.Register<IFlagsRegister, Intel8080FlagsRegister>();
                    _container.RegisterMany(new[] {typeof (IIntel8080Registers), typeof (IRegisters)},
                        typeof (Z80Registers));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platformConfig.CpuMode), platformConfig.CpuMode, null);
            }

            _isInitialized = true;
            return this;
        }
    }
}