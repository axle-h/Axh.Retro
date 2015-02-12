namespace Axh.Emulation.CPU.Z80.Factories
{
    using Axh.Emulation.CPU.Z80.Contracts.Factories;
    using Axh.Emulation.CPU.Z80.Contracts.Registers;
    using Axh.Emulation.CPU.Z80.Registers;

    public class FlagsRegisterFactory : IFlagsRegisterFactory
    {
        public IFlagsRegister GetFlagsRegister()
        {
            return new FlagsRegister();
        }
    }
}
