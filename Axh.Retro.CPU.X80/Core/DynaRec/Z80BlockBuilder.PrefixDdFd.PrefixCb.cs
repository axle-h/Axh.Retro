namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Xpr = Z80Expressions;

    internal partial class Z80BlockBuilder
    {
        /// <summary>
        /// Run CB prefixed opcodes with an index
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Expression> TryDecodeNextDdFdCbPrefixOperation()
        {
            // Read the displacement as the next byte
            yield return Expression.Assign(Xpr.LocalByte, NextByte);

            var opCode = (PrefixCbOpCode)mmuCache.NextByte();
            
            switch (opCode)
            {
                // ********* Rotate *********
                // RLC r (undocumented)
                // Actually RLC (IX+d) & LD r,(IX+d) or RLC (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RLC_A:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_B:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_C:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_D:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_E:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_H:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_L:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // RLC (IX+d)
                // RLC (IY+d)
                case PrefixCbOpCode.RLC_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluRotateLeftWithCarry);
                    break;

                // RL r (undocumented)
                // Actually RL (IX+d) & LD r,(IX+d) or RL (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RL_A:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_B:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_C:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_D:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_E:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_H:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_L:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // RL (IX+d)
                // RL (IY+d)
                case PrefixCbOpCode.RL_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluRotateLeft);
                    break;

                // RRC r (undocumented)
                // Actually RRC (IX+d) & LD r,(IX+d) or RRC (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RRC_A:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_B:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_C:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_D:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_E:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_H:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_L:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // RRC (IX+d)
                // RRC (IY+d)
                case PrefixCbOpCode.RRC_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluRotateRightWithCarry);
                    break;

                // RR r (undocumented)
                // Actually RR (IX+d) & LD r,(IX+d) or RR (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RR_A:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_B:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_C:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_D:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_E:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_H:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_L:
                    yield return GetAluCallWrite(7, 26, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // RR (IX+d)
                // RR (IY+d)
                case PrefixCbOpCode.RR_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluRotateRight);
                    break;

                // ********* Shift *********
                // SLA r (undocumented)
                // Actually SLA (IX+d) & LD r,(IX+d) or SLA (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SLA_A:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_B:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_C:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_D:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_E:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_H:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_L:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // SLA (IX+d)
                // SLA (IY+d)
                case PrefixCbOpCode.SLA_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluShiftLeft);
                    break;

                // SLS r (undocumented)
                // Actually SLS (IX+d) & LD r,(IX+d) or SLS (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SLS_A:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_B:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_C:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_D:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_E:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_H:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_L:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // SLS (IX+d) (undocumented)
                // SLS (IY+d) (undocumented)
                case PrefixCbOpCode.SLS_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluShiftLeftSet);
                    break;

                // SRA r (undocumented)
                // Actually SRA (IX+d) & LD r,(IX+d) or SRA (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SRA_A:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_B:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_C:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_D:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_E:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_H:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_L:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // SRA (IX+d)
                // SRA (IY+d)
                case PrefixCbOpCode.SRA_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluShiftRight);
                    break;

                // SRL r (undocumented)
                // Actually SRL (IX+d) & LD r,(IX+d) or SRL (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SRL_A:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_B:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_C:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_D:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_E:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_H:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_L:
                    yield return GetAluCallWrite(7, 26, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // SRL (IX+d)
                // SRL (IY+d)
                case PrefixCbOpCode.SRL_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluShiftRightLogical);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
