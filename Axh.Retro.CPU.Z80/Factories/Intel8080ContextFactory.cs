namespace Axh.Retro.CPU.Z80.Factories
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.CPU.Z80.Registers;

    public class Intel8080ContextFactory : CoreContextFactoryBase<IIntel8080Registers, Intel8080RegisterState>
    {
        public Intel8080ContextFactory(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPeripheralFactory peripheralFactory, IInitialStateConfig<Intel8080RegisterState> initialStateConfig)
            : base(platformConfig, runtimeConfig, peripheralFactory, initialStateConfig)
        {
        }

        protected override IIntel8080Registers GetInitialRegisters()
        {
            var registerSet = new GeneralPurposeRegisterSet();
            var accumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(GetFlagsRegister(this.PlatformConfig.CpuMode));

            return new Intel8080Registers(registerSet, accumulatorAndFlagsRegisterSet);
        }

        private static IFlagsRegister GetFlagsRegister(CpuMode cpuMode)
        {
            switch (cpuMode)
            {
                case CpuMode.GameBoy:
                    return new GameBoyFlagsRegister();
                case CpuMode.Z80:
                case CpuMode.Intel8080:
                    return new Intel8080FlagsRegister();
                default:
                    throw new ArgumentOutOfRangeException(nameof(cpuMode), cpuMode, null);
            }
        }
    }
}
