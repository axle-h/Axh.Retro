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
    /// <summary>
    /// Intel 8080 wiring with DryIoc.
    /// </summary>
    /// <typeparam name="TRegisters">The type of the registers.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public abstract class Intel8080WiringBase<TRegisters> : IDisposable where TRegisters : IRegisters
    {
        private readonly IContainer _container;
        private bool _isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="Intel8080WiringBase{TRegisters}"/> class.
        /// </summary>
        protected Intel8080WiringBase()
        {
            _container =
                new Container(
                    rules =>
                    rules.WithDefaultReuseInsteadOfTransient(Reuse.InResolutionScopeOf(typeof(ICpuCore<>).MakeGenericType(typeof(TRegisters))))
                         .WithoutThrowOnRegisteringDisposableTransient()); // I've implemented a proper dispose chain through object graph from ICpuCore.
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => _container.Dispose();

        /// <summary>
        /// Gets a new core.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public ICpuCore<TRegisters> GetNewCore()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"Must call {nameof(Init)} first");
            }

            return _container.Resolve<ICpuCore<TRegisters>>();
        }

        /// <summary>
        /// Adds the specified module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns></returns>
        public Intel8080WiringBase<TRegisters> With(IZ80Module module)
        {
            module.Register(_container);
            return this;
        }

        /// <summary>
        /// Adds the specified module.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <returns></returns>
        public Intel8080WiringBase<TRegisters> With<TModule>() where TModule : IZ80Module, new()
        {
            new TModule().Register(_container);
            return this;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// null
        /// or
        /// null
        /// </exception>
        /// <exception cref="System.ArgumentException">Invalid register type for platform.</exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public Intel8080WiringBase<TRegisters> Init()
        {
            if (!_container.IsRegistered<IRuntimeConfig>())
            {
                throw new Exception($"Cannot resolve {nameof(IRuntimeConfig)}");
            }

            if (!_container.IsRegistered<IPlatformConfig>())
            {
                throw new Exception($"Cannot resolve {nameof(IPlatformConfig)}");
            }

            _container.Register<GeneralPurposeRegisterSet, GeneralPurposeRegisterSet>();
            _container.Register<AccumulatorAndFlagsRegisterSet, AccumulatorAndFlagsRegisterSet>();

            Type registerType;
            var platformConfig = _container.Resolve<IPlatformConfig>();
            switch (platformConfig.CpuMode)
            {
                case CpuMode.Intel8080:
                    _container.Register<IFlagsRegister, Intel8080FlagsRegister>();
                    registerType = typeof(Z80Registers);
                    break;
                case CpuMode.Z80:
                    _container.Register<IFlagsRegister, Intel8080FlagsRegister>();
                    registerType = typeof(Intel8080Registers);
                    break;
                case CpuMode.GameBoy:
                    _container.Register<IFlagsRegister, GameBoyFlagsRegister>();
                    registerType = typeof(Intel8080Registers);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platformConfig.CpuMode), platformConfig.CpuMode, null);
            }

            _container.Register(typeof(IRegisters), registerType);
            if (!typeof(TRegisters).IsAssignableFrom(registerType))
            {
                throw new ArgumentException("Invalid register type for platform.");
            }

            // Since we know that we're always resolving IRegisters as registerType, we can avoid generic resolution with MakeGenericType.
            _container.Register<ICpuCore<TRegisters>, CachingCpuCore<TRegisters>>();

            _container.Register<IPeripheralManager, PeripheralManager>();
            _container.Register<IMmu, Z80Mmu<TRegisters>>();
            _container.Register<IPrefetchQueue, PrefetchQueue>();

            _container.Register<IAlu, Alu>();

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

            _isInitialized = true;
            return this;
        }
    }
}
