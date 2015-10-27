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
        /// Run CB prefixed opcodes without an index
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Expression> TryDecodeNextCbPrefixOperation()
        {
            var opCode = (PrefixCbOpCode)mmuCache.NextByte();

            switch (opCode)
            {
                // RLC r
                case PrefixCbOpCode.RLC_B:
                    yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, Xpr.B));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RLC_C:
                    yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, Xpr.C));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RLC_D:
                    yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, Xpr.D));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RLC_E:
                    yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, Xpr.E));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RLC_H:
                    yield return Expression.Assign(Xpr.H, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, Xpr.H));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RLC_L:
                    yield return Expression.Assign(Xpr.L, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, Xpr.L));
                    timer.Add(2, 8);
                    break;

                // RLC (HL)
                case PrefixCbOpCode.RLC_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, Xpr.ReadByteAtHL));
                    timer.Add(4, 15);
                    break;

                // RLC A
                case PrefixCbOpCode.RLC_A:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, Xpr.A));
                    timer.Add(2, 8);
                    break;

                // RL r
                case PrefixCbOpCode.RL_B:
                    yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, Xpr.B));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RL_C:
                    yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, Xpr.C));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RL_D:
                    yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, Xpr.D));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RL_E:
                    yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, Xpr.E));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RL_H:
                    yield return Expression.Assign(Xpr.H, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, Xpr.H));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RL_L:
                    yield return Expression.Assign(Xpr.L, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, Xpr.L));
                    timer.Add(2, 8);
                    break;

                // RL (HL)
                case PrefixCbOpCode.RL_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, Xpr.ReadByteAtHL));
                    timer.Add(4, 15);
                    break;

                // RL A
                case PrefixCbOpCode.RL_A:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, Xpr.A));
                    timer.Add(2, 8);
                    break;

                // RRC r
                case PrefixCbOpCode.RRC_B:
                    yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, Xpr.B));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RRC_C:
                    yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, Xpr.C));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RRC_D:
                    yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, Xpr.D));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RRC_E:
                    yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, Xpr.E));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RRC_H:
                    yield return Expression.Assign(Xpr.H, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, Xpr.H));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RRC_L:
                    yield return Expression.Assign(Xpr.L, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, Xpr.L));
                    timer.Add(2, 8);
                    break;

                // RRC (HL)
                case PrefixCbOpCode.RRC_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, Xpr.ReadByteAtHL));
                    timer.Add(4, 15);
                    break;

                // RRC A
                case PrefixCbOpCode.RRC_A:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, Xpr.A));
                    timer.Add(2, 8);
                    break;

                // RR r
                case PrefixCbOpCode.RR_B:
                    yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, Xpr.B));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RR_C:
                    yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, Xpr.C));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RR_D:
                    yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, Xpr.D));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RR_E:
                    yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, Xpr.E));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RR_H:
                    yield return Expression.Assign(Xpr.H, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, Xpr.H));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.RR_L:
                    yield return Expression.Assign(Xpr.L, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, Xpr.L));
                    timer.Add(2, 8);
                    break;

                // RR (HL)
                case PrefixCbOpCode.RR_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, Xpr.ReadByteAtHL));
                    timer.Add(4, 15);
                    break;

                // RR A
                case PrefixCbOpCode.RR_A:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, Xpr.A));
                    timer.Add(2, 8);
                    break;

                // SLA r
                case PrefixCbOpCode.SLA_B:
                    yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Alu, Xpr.AluShiftLeft, Xpr.B));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLA_C:
                    yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Alu, Xpr.AluShiftLeft, Xpr.C));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLA_D:
                    yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Alu, Xpr.AluShiftLeft, Xpr.D));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLA_E:
                    yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Alu, Xpr.AluShiftLeft, Xpr.E));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLA_H:
                    yield return Expression.Assign(Xpr.H, Expression.Call(Xpr.Alu, Xpr.AluShiftLeft, Xpr.H));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLA_L:
                    yield return Expression.Assign(Xpr.L, Expression.Call(Xpr.Alu, Xpr.AluShiftLeft, Xpr.L));
                    timer.Add(2, 8);
                    break;

                // SLA (HL)
                case PrefixCbOpCode.SLA_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluShiftLeft, Xpr.ReadByteAtHL));
                    timer.Add(4, 15);
                    break;

                // SLA A
                case PrefixCbOpCode.SLA_A:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluShiftLeft, Xpr.A));
                    timer.Add(2, 8);
                    break;

                // SLS r (undocumented)
                case PrefixCbOpCode.SLS_B:
                    yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Alu, Xpr.AluShiftLeftSet, Xpr.B));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLS_C:
                    yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Alu, Xpr.AluShiftLeftSet, Xpr.C));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLS_D:
                    yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Alu, Xpr.AluShiftLeftSet, Xpr.D));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLS_E:
                    yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Alu, Xpr.AluShiftLeftSet, Xpr.E));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLS_H:
                    yield return Expression.Assign(Xpr.H, Expression.Call(Xpr.Alu, Xpr.AluShiftLeftSet, Xpr.H));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SLS_L:
                    yield return Expression.Assign(Xpr.L, Expression.Call(Xpr.Alu, Xpr.AluShiftLeftSet, Xpr.L));
                    timer.Add(2, 8);
                    break;

                // SLS (HL) (undocumented)
                case PrefixCbOpCode.SLS_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluShiftLeftSet, Xpr.ReadByteAtHL));
                    timer.Add(4, 15);
                    break;

                // SLS A (undocumented)
                case PrefixCbOpCode.SLS_A:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluShiftLeftSet, Xpr.A));
                    timer.Add(2, 8);
                    break;

                // SRA r
                case PrefixCbOpCode.SRA_B:
                    yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Alu, Xpr.AluShiftRight, Xpr.B));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRA_C:
                    yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Alu, Xpr.AluShiftRight, Xpr.C));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRA_D:
                    yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Alu, Xpr.AluShiftRight, Xpr.D));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRA_E:
                    yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Alu, Xpr.AluShiftRight, Xpr.E));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRA_H:
                    yield return Expression.Assign(Xpr.H, Expression.Call(Xpr.Alu, Xpr.AluShiftRight, Xpr.H));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRA_L:
                    yield return Expression.Assign(Xpr.L, Expression.Call(Xpr.Alu, Xpr.AluShiftRight, Xpr.L));
                    timer.Add(2, 8);
                    break;

                // SRA (HL)
                case PrefixCbOpCode.SRA_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluShiftRight, Xpr.ReadByteAtHL));
                    timer.Add(4, 15);
                    break;

                // SRA A (undocumented)
                case PrefixCbOpCode.SRA_A:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluShiftRight, Xpr.A));
                    timer.Add(2, 8);
                    break;

                // SRL r
                case PrefixCbOpCode.SRL_B:
                    yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Alu, Xpr.AluShiftRightLogical, Xpr.B));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRL_C:
                    yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Alu, Xpr.AluShiftRightLogical, Xpr.C));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRL_D:
                    yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Alu, Xpr.AluShiftRightLogical, Xpr.D));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRL_E:
                    yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Alu, Xpr.AluShiftRightLogical, Xpr.E));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRL_H:
                    yield return Expression.Assign(Xpr.H, Expression.Call(Xpr.Alu, Xpr.AluShiftRightLogical, Xpr.H));
                    timer.Add(2, 8);
                    break;
                case PrefixCbOpCode.SRL_L:
                    yield return Expression.Assign(Xpr.L, Expression.Call(Xpr.Alu, Xpr.AluShiftRightLogical, Xpr.L));
                    timer.Add(2, 8);
                    break;

                // SRL (HL)
                case PrefixCbOpCode.SRL_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluShiftRightLogical, Xpr.ReadByteAtHL));
                    timer.Add(4, 15);
                    break;

                // SRL A (undocumented)
                case PrefixCbOpCode.SRL_A:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluShiftRightLogical, Xpr.A));
                    timer.Add(2, 8);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
