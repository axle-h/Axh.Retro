namespace Axh.Retro.GameBoy.Config
{
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Contracts.State;

    public class GameBoyInitialStateConfig : IInitialStateConfig<Z80RegisterState>
    {
        private const ushort InitialStackPointer = 0x0;
        private const ushort InitialProgramCounter = 0x1000;

        public Z80RegisterState GetInitialRegisterState()
        {
            var state = new Z80RegisterState
                        {
                            PrimaryGeneralPurposeRegisterState = GetInitialGeneralPurposeRegisterState(),
                            AlternativeGeneralPurposeRegisterState = GetInitialGeneralPurposeRegisterState(),
                            PrimaryAccumulatorAndFlagsRegisterState = GetInitialAccumulatorAndFlagsRegisterState(),
                            AlternativeAccumulatorAndFlagsRegisterState =  GetInitialAccumulatorAndFlagsRegisterState(),
                            IsAccumulatorAndFlagsAlternative = false,
                            IsGeneralPurposeAlternative = false,
                            IX = 0,
                            IY = 0,
                            I = 0,
                            R = 0,
                            StackPointer = InitialStackPointer,
                            ProgramCounter = InitialProgramCounter,
                            InterruptFlipFlop1 = false,
                            InterruptFlipFlop2 = false,
                            InterruptMode = InterruptMode.InterruptMode0
                        };
            return state;
        }

        private static GeneralPurposeRegisterState GetInitialGeneralPurposeRegisterState()
        {
            var state = new GeneralPurposeRegisterState();
            state.B = state.C = state.D = state.E = state.H = state.L = 0;
            return state;
        }
        
        private static AccumulatorAndFlagsRegisterState GetInitialAccumulatorAndFlagsRegisterState()
        {
            var state = new AccumulatorAndFlagsRegisterState();
            state.A = state.F = 0;
            return state;
        }
    }
}
