namespace Axh.Emulation.CPU.Z80.Tests.Registers
{
    using Axh.Emulation.CPU.Z80.Contracts.Registers;
    using Axh.Emulation.CPU.Z80.Contracts.State;

    internal static class RegisterTestObjects
    {
        public static Z80RegisterState Z80RegisterState
        {
            get
            {
                var state = new Z80RegisterState
                {
                    PrimaryRegisterState = Z80GeneralPurposeRegisterState,
                    AlternativeRegisterState = Z80GeneralPurposeRegisterState,
                    IsAlternative = true,
                    IX = 0x11,
                    IY = 0x22,
                    I = 0x33,
                    R = 0x44,
                    StackPointer = 0x12,
                    ProgramCounter = 0x34,
                    InterruptFlipFlop1 = true,
                    InterruptFlipFlop2 = true,
                    InterruptMode = InterruptMode.InterruptMode2
                };
                return state;
            }
        }

        public static Z80GeneralPurposeRegisterState Z80GeneralPurposeRegisterState
        {
            get
            {
                return new Z80GeneralPurposeRegisterState
                {
                    A = 0x11,
                    B = 0x22,
                    C = 0x33,
                    D = 0x44,
                    E = 0x55,
                    F = 0x66,
                    H = 0x77,
                    L = 0x88
                };
            }
        }
    }
}
