using Axh.Retro.CPU.X80.Contracts.Registers;

namespace Axh.Retro.CPU.X80.Contracts.Factories
{
    using Retro.CPU.X80.Contracts.Registers;

    public interface IRegisterFactory
    {
        IZ80Registers GetInitialZ80Registers();

        IGeneralPurposeRegisterSet GetRegisterSet();

        IFlagsRegister GetFlagsRegister();
    }
}