namespace Axh.Emulation.CPU.Z80.Factories
{
    using Axh.Emulation.CPU.Z80.Contracts.Config;
    using Axh.Emulation.CPU.Z80.Contracts.Factories;
    using Axh.Emulation.CPU.Z80.Contracts.Registers;
    using Axh.Emulation.CPU.Z80.Registers;

    public class RegisterFactory : IRegisterFactory
    {
        private readonly IInitialStateConfig initialStateConfig;

        public RegisterFactory(IInitialStateConfig initialStateConfig)
        {
            this.initialStateConfig = initialStateConfig;
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
            return new FlagsRegister();
        }
    }
}
