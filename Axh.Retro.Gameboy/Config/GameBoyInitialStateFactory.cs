namespace Axh.Retro.GameBoy.Config
{
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;

    public class GameBoyInitialStateFactory : IInitialStateFactory<Intel8080RegisterState>
    {
        private const ushort InitialStackPointer = 0x0;
        private const ushort InitialProgramCounter = 0x1000;

        public Intel8080RegisterState GetInitialRegisterState()
        {
            var state = new Intel8080RegisterState
                        {
                            GeneralPurposeRegisterState = GetInitialGeneralPurposeRegisterState(),
                            AccumulatorAndFlagsRegisterState = GetInitialAccumulatorAndFlagsRegisterState(),
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
