namespace Axh.Retro.CPU.X80.Factories
{
    using System;

    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Contracts.State;
    using Axh.Retro.CPU.X80.Registers;

    public class Z80RegisterFactory : IRegisterFactory<IZ80Registers> 
    {
        private readonly IInitialStateConfig<Z80RegisterState> initialStateConfig;

        private readonly IPlatformConfig platformConfig;

        public Z80RegisterFactory(IInitialStateConfig<Z80RegisterState> initialStateConfig, IPlatformConfig platformConfig)
        {
            this.initialStateConfig = initialStateConfig;
            this.platformConfig = platformConfig;
        }

        public IZ80Registers GetInitialRegisters()
        {
            var primaryRegisterSet = this.GetGeneralPurposeRegisterSet();
            var alternativeRegisterSet = this.GetGeneralPurposeRegisterSet();

            var primaryAccumulatorAndFlagsRegisterSet = this.GetAccumulatorAndFlagsRegisterSet();
            var alternativeAccumulatorAndFlagsRegisterSet = this.GetAccumulatorAndFlagsRegisterSet();

            var registers = new Z80Registers(primaryRegisterSet, alternativeRegisterSet, primaryAccumulatorAndFlagsRegisterSet, alternativeAccumulatorAndFlagsRegisterSet);

            var initialRegisterState = this.initialStateConfig.GetInitialRegisterState();
            registers.ResetToState(initialRegisterState);

            return registers;
        }
        
        public IGeneralPurposeRegisterSet GetGeneralPurposeRegisterSet()
        {
            return new GeneralPurposeRegisterSet();
        }
        
        public IAccumulatorAndFlagsRegisterSet GetAccumulatorAndFlagsRegisterSet()
        {
            var flagsRegister = this.GetFlagsRegister();
            return new AccumulatorAndFlagsRegisterSet(flagsRegister);
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
