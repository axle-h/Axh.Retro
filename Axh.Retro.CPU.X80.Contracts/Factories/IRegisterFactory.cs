namespace Axh.Retro.CPU.X80.Contracts.Factories
{
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public interface IRegisterFactory<out TRegisters> where TRegisters : IRegisters
    {
        TRegisters GetInitialRegisters();

        IGeneralPurposeRegisterSet GetRegisterSet();

        IFlagsRegister GetFlagsRegister();
    }
}