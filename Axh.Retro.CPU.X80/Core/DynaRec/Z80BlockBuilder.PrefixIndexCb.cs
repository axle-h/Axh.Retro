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
        private IEnumerable<Expression> TryDecodeNextCbPrefixOperationWithDisplacedIndexTimings(IndexRegisterExpressions index)
        {
            // Read the displacement as the next byte
            yield return Expression.Assign(Xpr.LocalByte, NextByte);

            var opCode = (PrefixCbOpCode)mmuCache.NextByte();
            
            switch (opCode)
            {
                // RLC r (undocumented)
                // Actually RLC (IX+d) & LD r,(IX+d) or RLC (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RLC_B:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RLC_C:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RLC_D:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RLC_E:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RLC_H:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RLC_L:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;

                // RLC (IX+d)
                // RLC (IY+d)
                case PrefixCbOpCode.RLC_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, index.ReadIndexedValue));
                    timer.Add(5, 19);
                    break;

                // RLC A (undocumented)
                // Actually RLC (IX+d) & LD A,(IX+d) or RLC (IY+d) & LD A,(IY+d)
                case PrefixCbOpCode.RLC_A:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;

                // RL r (undocumented)
                // Actually RL (IX+d) & LD r,(IX+d) or RL (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RL_B:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RL_C:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RL_D:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RL_E:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RL_H:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RL_L:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;

                // RL (IX+d)
                // RL (IY+d)
                case PrefixCbOpCode.RL_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, index.ReadIndexedValue));
                    timer.Add(5, 19);
                    break;

                // RL A (undocumented)
                // Actually RL (IX+d) & LD A,(IX+d) or RL (IY+d) & LD A,(IY+d)
                case PrefixCbOpCode.RL_A:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;

                // RRC r (undocumented)
                // Actually RRC (IX+d) & LD r,(IX+d) or RRC (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RRC_B:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RRC_C:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RRC_D:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RRC_E:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RRC_H:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RRC_L:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;

                // RRC (IX+d)
                // RRC (IY+d)
                case PrefixCbOpCode.RRC_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, index.ReadIndexedValue));
                    timer.Add(5, 19);
                    break;

                // RRC A (undocumented)
                // Actually RRC (IX+d) & LD A,(IX+d) or RRC (IY+d) & LD A,(IY+d)
                case PrefixCbOpCode.RRC_A:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;

                // RR r (undocumented)
                // Actually RR (IX+d) & LD r,(IX+d) or RR (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RR_B:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RR_C:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RR_D:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RR_E:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RR_H:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;
                case PrefixCbOpCode.RR_L:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;

                // RR (IX+d)
                // RR (IY+d)
                case PrefixCbOpCode.RR_mHL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, index.ReadIndexedValue));
                    timer.Add(5, 19);
                    break;

                // RR A (undocumented)
                // Actually RR (IX+d) & LD A,(IX+d) or RR (IY+d) & LD A,(IY+d)
                case PrefixCbOpCode.RR_A:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, index.ReadIndexedValue));
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timer.Add(7, 26);
                    break;

                // SLA r (undocumented)
                // Actually SLA (IX+d) & LD r,(IX+d) or SLA (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SLA_B:
                    break;
                case PrefixCbOpCode.SLA_C:
                    break;
                case PrefixCbOpCode.SLA_D:
                    break;
                case PrefixCbOpCode.SLA_E:
                    break;
                case PrefixCbOpCode.SLA_H:
                    break;
                case PrefixCbOpCode.SLA_L:
                    break;

                // SLA (IX+d)
                // SLA (IY+d)
                case PrefixCbOpCode.SLA_mHL:
                    break;

                // SLA A (undocumented)
                // Actually SLA (IX+d) & LD A,(IX+d) or SLA (IY+d) & LD A,(IY+d)
                case PrefixCbOpCode.SLA_A:
                    break;

                // SRA r (undocumented)
                // Actually SRA (IX+d) & LD r,(IX+d) or SRA (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SRA_B:
                    break;
                case PrefixCbOpCode.SRA_C:
                    break;
                case PrefixCbOpCode.SRA_D:
                    break;
                case PrefixCbOpCode.SRA_E:
                    break;
                case PrefixCbOpCode.SRA_H:
                    break;
                case PrefixCbOpCode.SRA_L:
                    break;

                // SRA (IX+d)
                // SRA (IY+d)
                case PrefixCbOpCode.SRA_mHL:
                    break;

                // SRA A (undocumented)
                // Actually SRA (IX+d) & LD A,(IX+d) or SRA (IY+d) & LD A,(IY+d)
                case PrefixCbOpCode.SRA_A:
                    break;

                // SRL r (undocumented)
                // Actually SRL (IX+d) & LD r,(IX+d) or SRL (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SRL_B:
                    break;
                case PrefixCbOpCode.SRL_C:
                    break;
                case PrefixCbOpCode.SRL_D:
                    break;
                case PrefixCbOpCode.SRL_E:
                    break;
                case PrefixCbOpCode.SRL_H:
                    break;
                case PrefixCbOpCode.SRL_L:
                    break;

                // SRL (IX+d)
                // SRL (IY+d)
                case PrefixCbOpCode.SRL_mHL:
                    break;

                // SRL A (undocumented)
                // Actually SRL (IX+d) & LD A,(IX+d) or SRL (IY+d) & LD A,(IY+d)
                case PrefixCbOpCode.SRL_A:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
