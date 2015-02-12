namespace Axh.Emulation.CPU.Z80.Factories
{
    using Axh.Emulation.CPU.Z80.Contracts.Factories;
    using Axh.Emulation.CPU.Z80.Contracts.Registers;
    using Axh.Emulation.CPU.Z80.Registers;

    public class RegisterSetFactory : IRegisterSetFactory
    {
        private readonly IFlagsRegisterFactory flagsRegisterFactory;

        public RegisterSetFactory(IFlagsRegisterFactory flagsRegisterFactory)
        {
            this.flagsRegisterFactory = flagsRegisterFactory;
        }

        public IRegisterSet GetRegisterSet()
        {
            var flagsRegister = this.flagsRegisterFactory.GetFlagsRegister();
            return new RegisterSet(flagsRegister);
        }
    }
}
