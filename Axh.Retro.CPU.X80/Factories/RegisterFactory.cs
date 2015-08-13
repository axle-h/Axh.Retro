namespace Axh.Retro.CPU.X80.Factories
{
    using System;

    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Registers;

    public class RegisterFactory : IRegisterFactory
    {
        private readonly IInitialStateConfig initialStateConfig;

        private readonly IPlatformConfig platformConfig;

        public RegisterFactory(IInitialStateConfig initialStateConfig, IPlatformConfig platformConfig)
        {
            this.initialStateConfig = initialStateConfig;
            this.platformConfig = platformConfig;
        }

        public IZ80Registers GetInitialZ80Registers()
        {
            var primaryRegisterSet = this.GetRegisterSet();
            var alternativeRegisterSet = this.GetRegisterSet();
            
            var registers = new Z80Registers(primaryRegisterSet, alternativeRegisterSet);

            var initialRegisterState = this.initialStateConfig.GetInitialRegisterState();
            registers.ResetToState(initialRegisterState);

            return registers;
        }


        public IGeneralPurposeRegisterSet GetRegisterSet()
        {
            var flagsRegister = this.GetFlagsRegister();
            return new GeneralPurposeRegisterSet(flagsRegister);
        }

        public IFlagsRegister GetFlagsRegister()
        {
            switch (platformConfig.CpuMode)
            {
                case CpuMode.Intel8080:
                    throw new NotImplementedException();
                case CpuMode.GameBoy:
                    // This might be the same as 8080?
                    throw new NotImplementedException();
                case CpuMode.Z80:
                    return new Z80FlagsRegister();
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}
