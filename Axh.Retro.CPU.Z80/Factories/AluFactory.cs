namespace Axh.Retro.CPU.Z80.Factories
{
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core;

    public class AluFactory : IAluFactory
    {
        public IArithmeticLogicUnit GetArithmeticLogicUnit(IFlagsRegister flagsRegister)
        {
            return new ArithmeticLogicUnit(flagsRegister);
        }
    }
}
