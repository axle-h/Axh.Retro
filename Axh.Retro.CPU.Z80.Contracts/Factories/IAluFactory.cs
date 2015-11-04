namespace Axh.Retro.CPU.Z80.Contracts.Factories
{
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public interface IAluFactory
    {
        IArithmeticLogicUnit GetArithmeticLogicUnit(IFlagsRegister flagsRegister);
    }
}