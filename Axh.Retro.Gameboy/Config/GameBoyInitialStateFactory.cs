using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.GameBoy.Config
{
    public class GameBoyInitialStateFactory : IInitialStateFactory
    {
        private const ushort InitialStackPointer = 0xfffe;
        private const ushort InitialProgramCounter = 0x0100;

        public Z80RegisterState GetInitialRegisterState()
        {
            var state = new Z80RegisterState
                        {
                            PrimaryGeneralPurposeRegisterState = GetInitialGeneralPurposeRegisterState(),
                            PrimaryAccumulatorAndFlagsRegisterState =
                                GetInitialAccumulatorAndFlagsRegisterState(),
                            StackPointer = InitialStackPointer,
                            ProgramCounter = InitialProgramCounter,
                            InterruptFlipFlop1 = true,
                            InterruptFlipFlop2 = true,
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