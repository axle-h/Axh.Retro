namespace Axh.Retro.CPU.X80.Factories
{
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Contracts.State;
    using Axh.Retro.CPU.X80.Registers;

    public class Z80RegisterFactory : IRegisterFactory<IZ80Registers>
    {
        private readonly IInitialStateConfig<Z80RegisterState> initialStateConfig;

        public Z80RegisterFactory(IInitialStateConfig<Z80RegisterState> initialStateConfig)
        {
            this.initialStateConfig = initialStateConfig;
        }

        public IZ80Registers GetInitialRegisters()
        {
            var primaryRegisterSet = new GeneralPurposeRegisterSet();
            var alternativeRegisterSet = new GeneralPurposeRegisterSet();

            var primaryAccumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(new Intel8080FlagsRegister());
            var alternativeAccumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(new Intel8080FlagsRegister());

            var registers = new Z80Registers(primaryRegisterSet, alternativeRegisterSet, primaryAccumulatorAndFlagsRegisterSet, alternativeAccumulatorAndFlagsRegisterSet);

            var initialRegisterState = this.initialStateConfig.GetInitialRegisterState();
            registers.ResetToState(initialRegisterState);

            return registers;
        }
    }
}
