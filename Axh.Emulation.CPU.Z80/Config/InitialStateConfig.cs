namespace Axh.Emulation.CPU.Z80.Config
{
    using Axh.Emulation.CPU.Z80.Contracts.Config;
    using Axh.Emulation.CPU.Z80.Contracts.Opcodes;
    using Axh.Emulation.CPU.Z80.Contracts.State;

    public class InitialStateConfig : IInitialStateConfig
    {
        private const ushort InitialStackPointer = 0x0;
        private const ushort InitialProgramCounter = 0x1000;

        public Z80RegisterState GetInitialRegisterState()
        {
            var state = new Z80RegisterState
                        {
                            PrimaryRegisterState = GetInitialZ80GeneralPurposeRegisterState(),
                            AlternativeRegisterState = GetInitialZ80GeneralPurposeRegisterState(),
                            IsAlternative = false,
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

        private static Z80GeneralPurposeRegisterState GetInitialZ80GeneralPurposeRegisterState()
        {
            var state = new Z80GeneralPurposeRegisterState();
            state.A = state.B = state.C = state.D = state.E = state.F = state.H = state.L = 0;
            return state;
        }
    }
}
