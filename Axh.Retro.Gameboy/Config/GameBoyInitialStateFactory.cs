namespace Axh.Retro.GameBoy.Config
{
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;

    public class GameBoyInitialStateFactory : IInitialStateFactory<Intel8080RegisterState>
    {
        private const ushort InitialStackPointer = 0xfffe;
        private const ushort InitialProgramCounter = 0x0100;

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
            var state = new GeneralPurposeRegisterState { B = 0x00, C = 0x13, D = 0x00, E = 0xd8, H = 0x01, L = 0x4d };
            return state;
        }
        
        private static AccumulatorAndFlagsRegisterState GetInitialAccumulatorAndFlagsRegisterState()
        {
            var state = new AccumulatorAndFlagsRegisterState { A = 0x01, F = 0xb0 };
            return state;
        }
    }
}
