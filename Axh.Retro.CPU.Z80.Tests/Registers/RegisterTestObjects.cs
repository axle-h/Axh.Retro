using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    internal static class RegisterTestObjects
    {
        public static Z80RegisterState Z80RegisterState
            =>
                new Z80RegisterState(GeneralPurposeRegisterState,
                                     GeneralPurposeRegisterState,
                                     AccumulatorAndFlagsRegisterState,
                                     AccumulatorAndFlagsRegisterState,
                                     true,
                                     true,
                                     0x11,
                                     0x22,
                                     0x33,
                                     0x44,
                                     0x12,
                                     0x34,
                                     true,
                                     true,
                                     InterruptMode.InterruptMode2);

        public static GeneralPurposeRegisterState GeneralPurposeRegisterState => new GeneralPurposeRegisterState(0x22, 0x33, 0x44, 0x55, 0x77, 0x88);

        public static AccumulatorAndFlagsRegisterState AccumulatorAndFlagsRegisterState => new AccumulatorAndFlagsRegisterState(0x11, 0x66);
    }
}