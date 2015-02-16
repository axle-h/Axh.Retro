namespace Axh.Emulation.CPU.X80.Opcodes
{
    public enum PrefixEdOpCodes : byte
    {
        //8 Bit Load
        LD_A_I = 0x57,
        LD_A_R = 0x5F,
        LD_I_A = 0x47,
        LD_R_A = 0x4F,

        //16 Bit Load
        LD_BC_mnn = 0x4B,
        LD_DE_mnn = 0x5B,
        LD_HL_mnn = 0x6B,
        LD_SP_mnn = 0x7B,

        LD_mnn_BC = 0x43,
        LD_mnn_DE = 0x53,
        LD_mnn_HL = 0x63,
        LD_mnn_SP = 0x73,

        //Exchange, block,, transfer and search
        LDI = 0xA0,
        LDIR = 0xB0,
        LDD = 0xA8,
        LDDR = 0xB8,
        CPI = 0xA1,
        CPIR = 0xB1,
        CPD = 0xA9,
        CPDR = 0xB9,

        //General purpose arithmetic and CPU control
        NEG = 0x44,
        IM0 = 0x46,
        IM1 = 0x56,
        IM2 = 0x5E,

        //16 Bit Arithmetic
        ADC_HL_BC = 0x4A,
        ADC_HL_DE = 0x5A,
        ADC_HL_HL = 0x6A,
        ADC_HL_SP = 0x7A,
        SBC_HL_BC = 0x42,
        SBC_HL_DE = 0x52,
        SBC_HL_HL = 0x62,
        SBC_HL_SP = 0x72,

    }
}
