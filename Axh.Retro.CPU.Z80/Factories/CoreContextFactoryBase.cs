namespace Axh.Retro.CPU.Z80.Factories
{
    using System;
    using System.Linq;

    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Common.Memory;
    using Axh.Retro.CPU.Z80.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core;
    using Axh.Retro.CPU.Z80.Core.Timing;
    using Axh.Retro.CPU.Z80.Peripherals;

    public abstract class CoreContextFactoryBase<TRegisters, TRegisterState> : ICoreContextFactory<TRegisters, TRegisterState>
        where TRegisterState : struct
        where TRegisters : IStateBackedRegisters<TRegisterState>
    {
        protected readonly IPlatformConfig PlatformConfig;
        protected readonly IRuntimeConfig RuntimeConfig;
        protected readonly IPeripheralFactory PeripheralFactory;
        protected readonly IInitialStateConfig<TRegisterState> InitialStateConfig;
        
        protected CoreContextFactoryBase(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPeripheralFactory peripheralFactory, IInitialStateConfig<TRegisterState> initialStateConfig)
        {
            this.PlatformConfig = platformConfig;
            this.RuntimeConfig = runtimeConfig;
            this.PeripheralFactory = peripheralFactory;
            this.InitialStateConfig = initialStateConfig;
        }

        public ICoreContext<TRegisters, TRegisterState> GetContext()
        {
            var interruptManager = new InterruptManager();
            var ioPeripherals = this.PeripheralFactory.GetIOMappedPeripherals(interruptManager).ToArray();
            var memorymappedPeripherals = this.PeripheralFactory.GetMemoryMappedPeripherals(interruptManager);

            var peripheralManager = new PeripheralManager(ioPeripherals.Cast<IPeripheral>().Concat(memorymappedPeripherals));

            var memoryBanks = this.PlatformConfig.MemoryBanks.OfType<IMemoryBankConfig>().Select(GetAddressSegment).ToArray();
            var mmu = new SegmentMmu(memoryBanks.Concat(peripheralManager.GetAllMemoryMappedPeripherals().SelectMany(x => x.AddressSegments)));
            peripheralManager.RegisterDma(mmu);

            var registers = this.GetInitialRegisters();
            var initialRegisterState = this.InitialStateConfig.GetInitialRegisterState();
            registers.ResetToState(initialRegisterState);

            var timer = new MachineCycleTimer(PlatformConfig);
            var alu = new Alu(registers.AccumulatorAndFlagsRegisters.Flags);

            var prefetchQueue = new PrefetchQueue(mmu, registers.ProgramCounter);

            var cache = new InstructionBlockCache<TRegisters>(this.RuntimeConfig);

            return new CoreContext<TRegisters, TRegisterState>(registers, interruptManager, peripheralManager, mmu, timer, alu, prefetchQueue, cache);
        }
        
        protected abstract TRegisters GetInitialRegisters();

        private static IAddressSegment GetAddressSegment(IMemoryBankConfig config)
        {
            switch (config.Type)
            {
                case MemoryBankType.RandomAccessMemory:
                    return new ArrayBackedMemoryBank(config);
                case MemoryBankType.ReadOnlyMemory:
                    return new ReadOnlyMemoryBank(config);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
