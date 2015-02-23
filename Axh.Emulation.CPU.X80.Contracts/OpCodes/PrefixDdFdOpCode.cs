namespace Axh.Emulation.CPU.X80.Contracts.OpCodes
{
    public enum PrefixDdFdOpCode : byte
    {
        LD_A_mIXYd = 0x7E,
        LD_B_mIXYd = 0x46,
        LD_C_mIXYd = 0x4E,
        LD_D_mIXYd = 0x56,
        LD_E_mIXYd = 0x5E,
        LD_H_mIXYd = 0x66,
        LD_L_mIXYd = 0x6E,

        LD_mIXYd_A = 0x77,
        LD_mIXYd_B = 0x70,
        LD_mIXYd_C = 0x71,
        LD_mIXYd_D = 0x72,
        LD_mIXYd_E = 0x73,
        LD_mIXYd_H = 0x74,
        LD_mIXYd_L = 0x75,

        LD_mIXYd_n = 0x36,

        LD_IXY_nn = 0x21,

        LD_IXY_mnn = 0x2A,

        LD_mnn_IXY = 0x22,

        LD_SP_IXY = 0xF9,

        PUSH_IXY = 0xE5,

        POP_IXY = 0xE1,
    }
}
