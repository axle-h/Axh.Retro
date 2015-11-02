namespace Axh.Retro.CPU.X80.Factories
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Registers;

    public class RegisterFactory : IRegisterFactory<IIntel8080Registers>
    {
        private readonly IPlatformConfig platformConfig;

        public RegisterFactory(IPlatformConfig platformConfig)
        {
            this.platformConfig = platformConfig;
        }

        public IIntel8080Registers GetInitialRegisters()
        {
            var registerSet = new GeneralPurposeRegisterSet();
            var accumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(GetFlagsRegister(this.platformConfig.CpuMode));

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
