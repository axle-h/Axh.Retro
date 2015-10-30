namespace Axh.Retro.CPU.X80.Factories
{
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Core;

    public class AluFactory : IAluFactory
    {
        public IArithmeticLogicUnit GetArithmeticLogicUnit(IFlagsRegister flagsRegister)
        {
            return new ArithmeticLogicUnit(flagsRegister);
        }
    }
}
