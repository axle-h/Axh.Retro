namespace Axh.Retro.CPU.Z80.Factories
{
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.CPU.Z80.Registers;

    public class Z80ContextFactory : CoreContextFactoryBase<IZ80Registers, Z80RegisterState>
    {
        public Z80ContextFactory(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPeripheralFactory peripheralFactory, IInitialStateConfig<Z80RegisterState> initialStateConfig)
            : base(platformConfig, runtimeConfig, peripheralFactory, initialStateConfig)
        {
        }

        protected override IZ80Registers GetInitialRegisters()
        {
            var primaryRegisterSet = new GeneralPurposeRegisterSet();
            var alternativeRegisterSet = new GeneralPurposeRegisterSet();

            var primaryAccumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(new Intel8080FlagsRegister());
            var alternativeAccumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(new Intel8080FlagsRegister());

            return new Z80Registers(primaryRegisterSet, alternativeRegisterSet, primaryAccumulatorAndFlagsRegisterSet, alternativeAccumulatorAndFlagsRegisterSet);
        }
    }
}
