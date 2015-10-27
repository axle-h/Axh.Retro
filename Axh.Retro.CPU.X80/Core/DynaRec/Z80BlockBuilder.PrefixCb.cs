namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Xpr = Z80Expressions;

    internal partial class Z80BlockBuilder
    {

        /// <summary>
        /// Run CB prefixed opcodes without an index
        /// </summary>
        /// <returns></returns>
        private Expression TryDecodeNextCbPrefixOperation()
        {
            var opCode = (PrefixCbOpCode)mmuCache.NextByte();

            switch (opCode)
            {
                // ********* Rotate *********
                // RLC r
                case PrefixCbOpCode.RLC_A:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.A);
                case PrefixCbOpCode.RLC_B:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.B);
                case PrefixCbOpCode.RLC_C:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.C);
                case PrefixCbOpCode.RLC_D:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.D);
                case PrefixCbOpCode.RLC_E:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.E);
                case PrefixCbOpCode.RLC_H:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.H);
                case PrefixCbOpCode.RLC_L:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.L);
                case PrefixCbOpCode.RLC_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluRotateLeftWithCarry);
                    
                // RL r
                case PrefixCbOpCode.RL_A:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.A);
                case PrefixCbOpCode.RL_B:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.B);
                case PrefixCbOpCode.RL_C:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.C);
                case PrefixCbOpCode.RL_D:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.D);
                case PrefixCbOpCode.RL_E:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.E);
                case PrefixCbOpCode.RL_H:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.H);
                case PrefixCbOpCode.RL_L:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.L);
                case PrefixCbOpCode.RL_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluRotateLeft);
                    
                // RRC r
                case PrefixCbOpCode.RRC_A:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.A);
                case PrefixCbOpCode.RRC_B:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.B);
                case PrefixCbOpCode.RRC_C:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.C);
                case PrefixCbOpCode.RRC_D:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.D);
                case PrefixCbOpCode.RRC_E:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.E);
                case PrefixCbOpCode.RRC_H:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.H);
                case PrefixCbOpCode.RRC_L:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.L);
                case PrefixCbOpCode.RRC_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluRotateRightWithCarry);
                    
                // RR r
                case PrefixCbOpCode.RR_A:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.A);
                case PrefixCbOpCode.RR_B:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.B);
                case PrefixCbOpCode.RR_C:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.C);
                case PrefixCbOpCode.RR_D:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.D);
                case PrefixCbOpCode.RR_E:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.E);
                case PrefixCbOpCode.RR_H:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.H);
                case PrefixCbOpCode.RR_L:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.L);
                case PrefixCbOpCode.RR_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluRotateRight);

                // ********* Shift *********
                // SLA r
                case PrefixCbOpCode.SLA_A:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.A);
                case PrefixCbOpCode.SLA_B:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.B);
                case PrefixCbOpCode.SLA_C:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.C);
                case PrefixCbOpCode.SLA_D:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.D);
                case PrefixCbOpCode.SLA_E:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.E);
                case PrefixCbOpCode.SLA_H:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.H);
                case PrefixCbOpCode.SLA_L:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.L);
                case PrefixCbOpCode.SLA_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluShiftLeft);

                // SLS r (undocumented)
                case PrefixCbOpCode.SLS_A:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeftSet, Xpr.A);
                case PrefixCbOpCode.SLS_B:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeftSet, Xpr.B);
                case PrefixCbOpCode.SLS_C:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeftSet, Xpr.C);
                case PrefixCbOpCode.SLS_D:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeftSet, Xpr.D);
                case PrefixCbOpCode.SLS_E:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeftSet, Xpr.E);
                case PrefixCbOpCode.SLS_H:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeftSet, Xpr.H);
                case PrefixCbOpCode.SLS_L:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeftSet, Xpr.L);
                case PrefixCbOpCode.SLS_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluShiftLeftSet);

                // SRA r
                case PrefixCbOpCode.SRA_A:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.A);
                case PrefixCbOpCode.SRA_B:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.B);
                case PrefixCbOpCode.SRA_C:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.C);
                case PrefixCbOpCode.SRA_D:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.D);
                case PrefixCbOpCode.SRA_E:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.E);
                case PrefixCbOpCode.SRA_H:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.H);
                case PrefixCbOpCode.SRA_L:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.L);
                case PrefixCbOpCode.SRA_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluShiftRight);
                    
                // SRL r
                case PrefixCbOpCode.SRL_A:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.A);
                case PrefixCbOpCode.SRL_B:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.B);
                case PrefixCbOpCode.SRL_C:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.C);
                case PrefixCbOpCode.SRL_D:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.D);
                case PrefixCbOpCode.SRL_E:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.E);
                case PrefixCbOpCode.SRL_H:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.H);
                case PrefixCbOpCode.SRL_L:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.L);
                case PrefixCbOpCode.SRL_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluShiftRightLogical);

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
