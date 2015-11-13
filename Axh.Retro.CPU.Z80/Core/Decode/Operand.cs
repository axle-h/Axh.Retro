namespace Axh.Retro.CPU.Z80.Core.Decode
{
    internal enum Operand
    {
        None = 0,

        // 8-bit registers
        A,
        B,
        C,
        D,
        E,
        F,
        H,
        L,

        // 16-bit registers
        HL,
        BC,
        DE,
        AF,
        SP,

        // Indexes
        mHL,
        mBC,
        mDE,
        mSP,
        
        // Literals
        mnn,
        nn,
        n,
        d,

        // Z80 indexes
        IX,
        mIXd,
        IXl,
        IXh,
        IY,
        mIYd,
        IYl,
        IYh
    }
}
