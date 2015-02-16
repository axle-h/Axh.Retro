namespace Axh.Emulation.CPU.X80.Contracts.Factories
{
    using Axh.Emulation.CPU.X80.Contracts.Registers;

    public interface IRegisterFactory
    {
        IZ80Registers GetInitialZ80Registers();

        IGeneralPurposeRegisterSet GetRegisterSet();

        IFlagsRegister GetFlagsRegister();
    }
}