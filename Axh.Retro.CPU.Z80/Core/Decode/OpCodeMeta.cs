namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;

    [Flags]
    internal enum OpCodeMeta
    {
        None = 0,
        ByteLiteral = 0x01,
        WordLiteral = 0x02,
        Displacement = 0x04,
        EndBlock = 0x08,
        AutoCopy = 0x10
    }
}