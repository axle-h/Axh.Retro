using System;

namespace Axh.Retro.CPU.Z80.Core.Decode
{
    /// <summary>
    ///     Internal decode meta flags for communicating with CPU execution.
    /// </summary>
    [Flags]
    internal enum OpCodeMeta : byte
    {
        None = 0,

        /// <summary>
        ///     Operation uses a byte literal.
        /// </summary>
        ByteLiteral = 0x01,

        /// <summary>
        ///     Operation uses a word literal.
        /// </summary>
        WordLiteral = 0x02,

        /// <summary>
        ///     Operation uses a displacement.
        /// </summary>
        Displacement = 0x04,

        /// <summary>
        ///     The end of this execution block. I.e. a jump, call, return, halt.
        /// </summary>
        EndBlock = 0x08,

        /// <summary>
        ///     Apply auto copy to register on CB DD/FD prefixed opcodes.
        /// </summary>
        AutoCopy = 0x10,

        /// <summary>
        ///     Use the alternative flag affection scheme e.g. RLCA and RLC A call same ALU function but effect flags slightly
        ///     differently.
        /// </summary>
        UseAlternativeFlagAffection = 0x20
    }
}