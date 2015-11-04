namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;

    internal static class RegisterTestObjects
    {
        public static Z80RegisterState Z80RegisterState => new Z80RegisterState
                                                           {
                                                               PrimaryGeneralPurposeRegisterState = GeneralPurposeRegisterState,
                                                               AlternativeGeneralPurposeRegisterState = GeneralPurposeRegisterState,
                                                               AlternativeAccumulatorAndFlagsRegisterState = AccumulatorAndFlagsRegisterState,
                                                               PrimaryAccumulatorAndFlagsRegisterState = AccumulatorAndFlagsRegisterState,
                                                               IsGeneralPurposeAlternative = true,
                                                               IsAccumulatorAndFlagsAlternative = true,
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

        public static GeneralPurposeRegisterState GeneralPurposeRegisterState => new GeneralPurposeRegisterState
                                                                                 {
                                                                                     B = 0x22,
                                                                                     C = 0x33,
                                                                                     D = 0x44,
                                                                                     E = 0x55,
                                                                                     H = 0x77,
                                                                                     L = 0x88
                                                                                 };

        public static AccumulatorAndFlagsRegisterState AccumulatorAndFlagsRegisterState => new AccumulatorAndFlagsRegisterState { A = 0x11, F = 0x66 };
    }
}
