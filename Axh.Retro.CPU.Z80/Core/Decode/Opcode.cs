namespace Axh.Retro.CPU.Z80.Core.Decode
{
    internal enum Opcode
    {
        NOP,
        HALT,

        LD,
        LD16,

        PUSH,
        POP,

        ADD,
        ADC,
        SUB,
        SBC,
        AND,
        OR,
        XOR,
        CP,
        INC,
        DEC,

        ADD16,
        INC16,
        DEC16,

        EX,
        EXX,

        DAA,
        CPL,
        CCF,
        SCF,

        DI,
        EI,

        RLCA,
        RLA,
        RRCA,
        RRA,

        JP,
        JR,
        DJNZ,
        CALL,
        RET,

        IN,

        OUT
    }
}
