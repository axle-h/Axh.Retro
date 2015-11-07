namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.Z80.Contracts;
    using Axh.Retro.CPU.Z80.Util;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    internal partial class DynaRecBlockBuilder<TRegisters> where TRegisters : IRegisters
    {
        private IEnumerable<Expression> GetBlockExpressions()
        {
            LastDecodeResult = DecodeResult.Continue;
            index = Xpr.IndexRegisterExpressions[IndexRegister.HL];

            while (this.LastDecodeResult == DecodeResult.Continue)
            {
                if (prefetchQueue.TotalBytesRead == ushort.MaxValue)
                {
                    LastDecodeResult = DecodeResult.FinalizeAndSync;
                    yield break;
                }

                var opCode = (PrimaryOpCode)this.prefetchQueue.NextByte();

                if (index.UsesDisplacedIndexTimings && OpCodeUsesDisplacedIndex(opCode))
                {
                    // Read the displacement as the next byte
                    yield return Expression.Assign(Xpr.LocalByte, NextByte);
                }

                switch (opCode)
                {
                    case PrimaryOpCode.NOP:
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.HALT:
                        timingsBuilder.Add(1, 4);
                        LastDecodeResult = DecodeResult.Halt;
                        yield break;

                    // ********* Prefixes *********
                    case PrimaryOpCode.Prefix_DD:
                        // Add a NOP timing for now and take a NOP off known indexed timings later.
                        timingsBuilder.Add(1, 4);
                        if (cpuMode == CpuMode.Z80)
                        {
                            // Only Z80 has prefix DD
                            this.index = Xpr.IndexRegisterExpressions[IndexRegister.IX];
                        }
                        continue;

                    case PrimaryOpCode.Prefix_FD:
                        // Add a NOP timing for now and take a NOP off known indexed timings later.
                        timingsBuilder.Add(1, 4);
                        if (cpuMode == CpuMode.Z80)
                        {
                            // Only Z80 has prefix FD
                            this.index = Xpr.IndexRegisterExpressions[IndexRegister.IY];
                        }
                        continue;

                    case PrimaryOpCode.Prefix_ED:
                        if (cpuMode == CpuMode.Z80)
                        {
                            // Only Z80 has prefix ED
                            foreach (var expression in TryDecodeNextEdPrefixOperation())
                            {
                                yield return expression;
                            }
                        }
                        else
                        {
                            // CPU's without ED prefix treat this as a NOP.
                            timingsBuilder.Add(1, 4);
                        }
                        break;

                    case PrimaryOpCode.Prefix_CB:
                        if (cpuMode == CpuMode.Z80 || cpuMode == CpuMode.GameBoy)
                        {
                            // Only Gameboy & Z80 have prefix CB opcodes.
                            if (index.UsesDisplacedIndexTimings)
                            {
                                foreach (var expression in TryDecodeNextDdFdCbPrefixOperation())
                                {
                                    yield return expression;
                                }
                            }
                            else
                            {
                                yield return TryDecodeNextCbPrefixOperation();
                            }
                        }
                        else
                        {
                            // CPU's without CB prefix treat this as a NOP.
                            timingsBuilder.Add(1, 4);
                        }
                        break;

                    // ********* 8-bit load *********
                    // LD r, r'
                    case PrimaryOpCode.LD_A_A:
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_A:
                        yield return Expression.Assign(Xpr.B, Xpr.A);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_A:
                        yield return Expression.Assign(Xpr.C, Xpr.A);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_A:
                        yield return Expression.Assign(Xpr.D, Xpr.A);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_A:
                        yield return Expression.Assign(Xpr.E, Xpr.A);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_A:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.A);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_A:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.A);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_B:
                        yield return Expression.Assign(Xpr.A, Xpr.B);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_B:
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_B:
                        yield return Expression.Assign(Xpr.C, Xpr.B);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_B:
                        yield return Expression.Assign(Xpr.D, Xpr.B);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_B:
                        yield return Expression.Assign(Xpr.E, Xpr.B);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_B:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.B);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_B:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.B);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_C:
                        yield return Expression.Assign(Xpr.A, Xpr.C);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_C:
                        yield return Expression.Assign(Xpr.B, Xpr.C);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_C:
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_C:
                        yield return Expression.Assign(Xpr.D, Xpr.C);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_C:
                        yield return Expression.Assign(Xpr.E, Xpr.C);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_C:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.C);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_C:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.C);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_D:
                        yield return Expression.Assign(Xpr.A, Xpr.D);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_D:
                        yield return Expression.Assign(Xpr.B, Xpr.D);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_D:
                        yield return Expression.Assign(Xpr.C, Xpr.D);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_D:
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_D:
                        yield return Expression.Assign(Xpr.E, Xpr.D);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_D:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.D);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_D:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.D);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_E:
                        yield return Expression.Assign(Xpr.A, Xpr.E);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_E:
                        yield return Expression.Assign(Xpr.B, Xpr.E);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_E:
                        yield return Expression.Assign(Xpr.C, Xpr.E);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_E:
                        yield return Expression.Assign(Xpr.D, Xpr.E);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_E:
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_E:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.E);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_E:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.E);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_H:
                        yield return Expression.Assign(Xpr.A, index.RegisterHighOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_H:
                        yield return Expression.Assign(Xpr.B, index.RegisterHighOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_H:
                        yield return Expression.Assign(Xpr.C, index.RegisterHighOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_H:
                        yield return Expression.Assign(Xpr.D, index.RegisterHighOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_H:
                        yield return Expression.Assign(Xpr.E, index.RegisterHighOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_H:
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_H:
                        yield return Expression.Assign(index.RegisterLowOrder, index.RegisterHighOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_L:
                        yield return Expression.Assign(Xpr.A, index.RegisterLowOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_L:
                        yield return Expression.Assign(Xpr.B, index.RegisterLowOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_L:
                        yield return Expression.Assign(Xpr.C, index.RegisterLowOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_L:
                        yield return Expression.Assign(Xpr.D, index.RegisterLowOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_L:
                        yield return Expression.Assign(Xpr.E, index.RegisterLowOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_L:
                        yield return Expression.Assign(index.RegisterHighOrder, index.RegisterLowOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_L:
                        timingsBuilder.Add(1, 4);
                        break;

                    // LD r,n
                    case PrimaryOpCode.LD_A_n:
                        yield return Expression.Assign(Xpr.A, NextByte);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_B_n:
                        yield return Expression.Assign(Xpr.B, NextByte);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_C_n:
                        yield return Expression.Assign(Xpr.C, NextByte);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_D_n:
                        yield return Expression.Assign(Xpr.D, NextByte);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_E_n:
                        yield return Expression.Assign(Xpr.E, NextByte);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_H_n:
                        yield return Expression.Assign(index.RegisterHighOrder, NextByte);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_L_n:
                        yield return Expression.Assign(index.RegisterLowOrder, NextByte);
                        timingsBuilder.Add(2, 7);
                        break;

                    // LD r, (HL)
                    case PrimaryOpCode.LD_A_mHL:
                        yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_B_mHL:
                        yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_C_mHL:
                        yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_D_mHL:
                        yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_E_mHL:
                        yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_H_mHL:
                        // H register is always assigned here
                        yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_L_mHL:
                        // L register is always assigned here
                        yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // LD (HL), r
                    case PrimaryOpCode.LD_mHL_A:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.A);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_B:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.B);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_C:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.C);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_D:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.D);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_E:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.E);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_H:
                        // Value of H register is always used here
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.H);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_L:
                        // Value of L register is always used here
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.L);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // LD (HL), n
                    case PrimaryOpCode.LD_mHL_n:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, NextByte);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(1, 5);
                        timingsBuilder.Add(3, 10);
                        break;

                    // LD A, (BC)
                    case PrimaryOpCode.LD_A_mBC:
                        yield return Expression.Assign(Xpr.A, Xpr.ReadByteAtBC);
                        timingsBuilder.Add(2, 7);
                        break;

                    // LD A, (DE)
                    case PrimaryOpCode.LD_A_mDE:
                        yield return Expression.Assign(Xpr.A, Xpr.ReadByteAtDE);
                        timingsBuilder.Add(2, 7);
                        break;

                    // LD A, (nn)
                    case PrimaryOpCode.LD_A_mnn:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LDD  A, (HL) on GB
                            yield return Expression.Assign(Xpr.A, Xpr.ReadByteAtHL);
                            yield return Expression.PreDecrementAssign(Xpr.HL);
                            timingsBuilder.Add(2, 8);
                        }
                        else
                        {
                            yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, NextWord));
                            timingsBuilder.Add(4, 13);
                        }
                        break;

                    // LD (BC), A
                    case PrimaryOpCode.LD_mBC_A:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.BC, Xpr.A);
                        timingsBuilder.Add(2, 7);
                        break;

                    // LD (DE), A
                    case PrimaryOpCode.LD_mDE_A:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.DE, Xpr.A);
                        timingsBuilder.Add(2, 7);
                        break;

                    // LD (nn), A
                    case PrimaryOpCode.LD_mnn_A:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LDD  (HL), A on GB
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Xpr.A);
                            yield return Expression.PreDecrementAssign(Xpr.HL);
                            timingsBuilder.Add(2, 8);
                        }
                        else
                        {
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, NextWord, Xpr.A);
                            timingsBuilder.Add(2, 7);
                        }
                        break;

                    // ********* 16-bit load *********
                    // LD dd, nn
                    case PrimaryOpCode.LD_BC_nn:
                        yield return Expression.Assign(Xpr.BC, NextWord);
                        timingsBuilder.Add(2, 10);
                        break;
                    case PrimaryOpCode.LD_DE_nn:
                        yield return Expression.Assign(Xpr.DE, NextWord);
                        timingsBuilder.Add(2, 10);
                        break;
                    case PrimaryOpCode.LD_HL_nn:
                        yield return Expression.Assign(index.Register, NextWord);
                        timingsBuilder.Add(index.UsesDisplacedIndexTimings ? 3 : 2, 10);
                        break;
                    case PrimaryOpCode.LD_SP_nn:
                        yield return Expression.Assign(Xpr.SP, NextWord);
                        timingsBuilder.Add(2, 10);
                        break;

                    // LD HL, (nn)
                    case PrimaryOpCode.LD_HL_mnn:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LDI  A, (HL) on GB
                            yield return Expression.Assign(Xpr.A, Xpr.ReadByteAtHL);
                            yield return Expression.PreIncrementAssign(Xpr.HL);
                            timingsBuilder.Add(2, 8);
                        }
                        else
                        {
                            yield return Expression.Assign(index.Register, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                            timingsBuilder.Add(5, 16);
                        }
                        break;

                    // LD (nn), HL
                    case PrimaryOpCode.LD_mnn_HL:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LDI (HL), A on GB
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Xpr.A);
                            yield return Expression.PreIncrementAssign(Xpr.HL);
                            timingsBuilder.Add(2, 8);
                        }
                        else
                        {
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, index.Register);
                            timingsBuilder.Add(5, 16);
                        }
                        break;

                    // LD SP, HL
                    case PrimaryOpCode.LD_SP_HL:
                        yield return Expression.Assign(Xpr.SP, index.Register);
                        timingsBuilder.Add(1, 6);
                        break;

                    // PUSH qq
                    case PrimaryOpCode.PUSH_BC:
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.B);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.C);
                        timingsBuilder.Add(3, 11);
                        break;
                    case PrimaryOpCode.PUSH_DE:
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.D);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.E);
                        timingsBuilder.Add(3, 11);
                        break;
                    case PrimaryOpCode.PUSH_HL:
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, index.RegisterHighOrder);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, index.RegisterLowOrder);
                        timingsBuilder.Add(3, 11);
                        break;
                    case PrimaryOpCode.PUSH_AF:
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.A);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.F);
                        timingsBuilder.Add(3, 11);
                        break;

                    // POP qq
                    case PrimaryOpCode.POP_BC:
                        yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        timingsBuilder.Add(3, 10);
                        break;
                    case PrimaryOpCode.POP_DE:
                        yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        timingsBuilder.Add(3, 10);
                        break;
                    case PrimaryOpCode.POP_HL:
                        yield return Expression.Assign(index.RegisterLowOrder, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        yield return Expression.Assign(index.RegisterHighOrder, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        timingsBuilder.Add(3, 10);
                        break;
                    case PrimaryOpCode.POP_AF:
                        yield return Expression.Assign(Xpr.F, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        timingsBuilder.Add(3, 10);
                        break;

                    // ********* Exchange *********
                    // EX DE, HL
                    case PrimaryOpCode.EX_DE_HL:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            timingsBuilder.Add(1, 4);
                        }
                        else
                        {
                            // This affects HL register directly, always ignoring index register prefixes
                            yield return Expression.Assign(Xpr.LocalWord, Xpr.DE);
                            yield return Expression.Assign(Xpr.DE, Xpr.HL);
                            yield return Expression.Assign(Xpr.HL, Xpr.LocalWord);
                            timingsBuilder.Add(1, 4);
                        }
                        break;

                    // EX AF, AF′
                    case PrimaryOpCode.EX_AF:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LD (nn),SP on GB
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.SP);
                            timingsBuilder.Add(6, 20);
                        }
                        else
                        {
                            yield return Xpr.SwitchToAlternativeAccumulatorAndFlagsRegisters;
                            timingsBuilder.Add(1, 4);
                        }
                        break;

                    // EXX
                    case PrimaryOpCode.EXX:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as RETI on GB
                            yield return Xpr.ReadPCFromStack;
                            yield return Xpr.PopPopSP;
                            timingsBuilder.Add(4, 16);
                            LastDecodeResult = DecodeResult.Finalize;
                            yield break;
                        }
                        else
                        {
                            yield return Xpr.SwitchToAlternativeGeneralPurposeRegisters;
                            timingsBuilder.Add(1, 4);
                            break;
                        }

                    // EX (SP), HL
                    case PrimaryOpCode.EX_mSP_HL:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            timingsBuilder.Add(1, 4);
                        }
                        else
                        {
                            // Exchange L
                            yield return Expression.Assign(Xpr.LocalByte, index.RegisterLowOrder);
                            yield return Expression.Assign(index.RegisterLowOrder, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.LocalByte);

                            // Exchange H
                            yield return Expression.Assign(Xpr.LocalByte, index.RegisterHighOrder);
                            yield return Expression.Assign(Xpr.LocalWord, Expression.Increment(Xpr.SP));
                            yield return Expression.Assign(index.RegisterHighOrder, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.LocalWord));
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.LocalWord, Xpr.LocalByte);

                            timingsBuilder.Add(5, 19);
                        }
                        break;

                    // ********* 8-Bit Arithmetic *********
                    // ADD A, r
                    case PrimaryOpCode.ADD_A_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.B));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.C));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.D));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.E));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, index.RegisterHighOrder));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, index.RegisterLowOrder));
                        timingsBuilder.Add(1, 4);
                        break;

                    // ADD A, n
                    case PrimaryOpCode.ADD_A_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, NextByte));
                        timingsBuilder.Add(2, 7);
                        break;

                    // ADD A, (HL)
                    case PrimaryOpCode.ADD_A_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // ADC A, r
                    case PrimaryOpCode.ADC_A_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.B));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.C));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.D));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.E));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, index.RegisterHighOrder));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, index.RegisterLowOrder));
                        timingsBuilder.Add(1, 4);
                        break;

                    // ADC A, n
                    case PrimaryOpCode.ADC_A_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, NextByte));
                        timingsBuilder.Add(2, 7);
                        break;

                    // ADC A, (HL)
                    case PrimaryOpCode.ADC_A_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // SUB A, r
                    case PrimaryOpCode.SUB_A_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.B));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.C));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.D));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.E));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, index.RegisterHighOrder));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, index.RegisterLowOrder));
                        timingsBuilder.Add(1, 4);
                        break;

                    // SUB A, n
                    case PrimaryOpCode.SUB_A_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, NextByte));
                        timingsBuilder.Add(2, 7);
                        break;

                    // SUB A, (HL)
                    case PrimaryOpCode.SUB_A_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // SBC A, r
                    case PrimaryOpCode.SBC_A_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.B));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.C));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.D));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.E));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, index.RegisterHighOrder));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, index.RegisterLowOrder));
                        timingsBuilder.Add(1, 4);
                        break;

                    // SBC A, n
                    case PrimaryOpCode.SBC_A_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, NextByte));
                        timingsBuilder.Add(2, 7);
                        break;

                    // SBC A, (HL)
                    case PrimaryOpCode.SBC_A_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // AND r
                    case PrimaryOpCode.AND_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.B));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.C));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.D));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.E));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, index.RegisterHighOrder));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, index.RegisterLowOrder));
                        timingsBuilder.Add(1, 4);
                        break;

                    // AND n
                    case PrimaryOpCode.AND_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, NextByte));
                        timingsBuilder.Add(2, 7);
                        break;

                    // AND (HL)
                    case PrimaryOpCode.AND_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // OR r
                    case PrimaryOpCode.OR_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.B));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.C));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.D));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.E));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, index.RegisterHighOrder));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, index.RegisterLowOrder));
                        timingsBuilder.Add(1, 4);
                        break;

                    // OR n
                    case PrimaryOpCode.OR_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, NextByte));
                        timingsBuilder.Add(2, 7);
                        break;

                    // OR (HL)
                    case PrimaryOpCode.OR_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // XOR r
                    case PrimaryOpCode.XOR_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.B));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.C));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.D));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.E));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, index.RegisterHighOrder));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, index.RegisterLowOrder));
                        timingsBuilder.Add(1, 4);
                        break;

                    // XOR n
                    case PrimaryOpCode.XOR_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, NextByte));
                        timingsBuilder.Add(2, 7);
                        break;

                    // XOR (HL)
                    case PrimaryOpCode.XOR_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // CP r
                    case PrimaryOpCode.CP_A:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.A);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_B:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.B);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_C:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.C);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_D:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.D);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_E:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.E);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_H:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, index.RegisterHighOrder);
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_L:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, index.RegisterLowOrder);
                        timingsBuilder.Add(1, 4);
                        break;

                    // CP n
                    case PrimaryOpCode.CP_n:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, NextByte);
                        timingsBuilder.Add(2, 7);
                        break;

                    // CP (HL)
                    case PrimaryOpCode.CP_mHL:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(2, 7);
                        break;

                    // INC r
                    case PrimaryOpCode.INC_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.B));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.C));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.D));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.E));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, index.RegisterHighOrder));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, index.RegisterLowOrder));
                        timingsBuilder.Add(1, 4);
                        break;

                    // INC (HL)
                    case PrimaryOpCode.INC_mHL:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluIncrement, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(3, 11);
                        break;

                    // DEC r
                    case PrimaryOpCode.DEC_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.B));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.C));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.D));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.E));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, index.RegisterHighOrder));
                        timingsBuilder.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, index.RegisterLowOrder));
                        timingsBuilder.Add(1, 4);
                        break;

                    // DEC (HL)
                    case PrimaryOpCode.DEC_mHL:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluDecrement, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timingsBuilder.Add(2, 8);
                        timingsBuilder.Add(3, 11);
                        break;

                    // ********* 16-Bit Arithmetic *********
                    // ADD HL, ss
                    case PrimaryOpCode.ADD_HL_BC:
                        yield return Expression.Assign(index.Register, Expression.Call(Xpr.Alu, Xpr.AluAdd16, index.Register, Xpr.BC));
                        timingsBuilder.Add(3, 11);
                        break;
                    case PrimaryOpCode.ADD_HL_DE:
                        yield return Expression.Assign(index.Register, Expression.Call(Xpr.Alu, Xpr.AluAdd16, index.Register, Xpr.DE));
                        timingsBuilder.Add(3, 11);
                        break;
                    case PrimaryOpCode.ADD_HL_HL:
                        yield return Expression.Assign(index.Register, Expression.Call(Xpr.Alu, Xpr.AluAdd16, index.Register, index.Register));
                        timingsBuilder.Add(3, 11);
                        break;
                    case PrimaryOpCode.ADD_HL_SP:
                        yield return Expression.Assign(index.Register, Expression.Call(Xpr.Alu, Xpr.AluAdd16, index.Register, Xpr.SP));
                        timingsBuilder.Add(3, 11);
                        break;

                    // INC ss (no flags changes so implemented directly)
                    case PrimaryOpCode.INC_BC:
                        yield return Expression.PreIncrementAssign(Xpr.BC);
                        timingsBuilder.Add(1, 6);
                        break;
                    case PrimaryOpCode.INC_DE:
                        yield return Expression.PreIncrementAssign(Xpr.DE);
                        timingsBuilder.Add(1, 6);
                        break;
                    case PrimaryOpCode.INC_HL:
                        yield return Expression.PreIncrementAssign(index.Register);
                        timingsBuilder.Add(1, 6);
                        break;
                    case PrimaryOpCode.INC_SP:
                        yield return Expression.PreIncrementAssign(Xpr.SP);
                        timingsBuilder.Add(1, 6);
                        break;

                    // DEC ss (no flags changes so implemented directly)
                    case PrimaryOpCode.DEC_BC:
                        yield return Expression.PreDecrementAssign(Xpr.BC);
                        timingsBuilder.Add(1, 6);
                        break;
                    case PrimaryOpCode.DEC_DE:
                        yield return Expression.PreDecrementAssign(Xpr.DE);
                        timingsBuilder.Add(1, 6);
                        break;
                    case PrimaryOpCode.DEC_HL:
                        yield return Expression.PreDecrementAssign(index.Register);
                        timingsBuilder.Add(1, 6);
                        break;
                    case PrimaryOpCode.DEC_SP:
                        yield return Expression.PreDecrementAssign(Xpr.SP);
                        timingsBuilder.Add(1, 6);
                        break;

                    // ********* General-Purpose Arithmetic *********
                    // DAA
                    case PrimaryOpCode.DAA:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecimalAdjust, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;

                    // CPL
                    case PrimaryOpCode.CPL:
                        yield return Expression.Assign(Xpr.A, Expression.Not(Xpr.A));
                        yield return Expression.Call(Xpr.Flags, Xpr.SetUndocumentedFlags, Xpr.A);
                        yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(true));
                        yield return Expression.Assign(Xpr.Subtract, Expression.Constant(true));
                        timingsBuilder.Add(1, 4);
                        break;

                    // CCF
                    case PrimaryOpCode.CCF:
                        yield return Expression.Assign(Xpr.HalfCarry, Xpr.Carry);
                        yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.Carry, Expression.Not(Xpr.Carry));
                        timingsBuilder.Add(1, 4);
                        break;

                    // SCF
                    case PrimaryOpCode.SCF:
                        yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.Carry, Expression.Constant(true));
                        timingsBuilder.Add(1, 4);
                        break;

                    // DI
                    case PrimaryOpCode.DI:
                        yield return Expression.Assign(Xpr.IFF1, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.IFF2, Expression.Constant(false));
                        timingsBuilder.Add(1, 4);
                        break;

                    // EI
                    case PrimaryOpCode.EI:
                        yield return Expression.Assign(Xpr.IFF1, Expression.Constant(true));
                        yield return Expression.Assign(Xpr.IFF2, Expression.Constant(true));
                        timingsBuilder.Add(1, 4);
                        break;

                    // ********* Rotate *********
                    // RLCA
                    case PrimaryOpCode.RLCA:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;

                    //RLA
                    case PrimaryOpCode.RLA:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;

                    // RRCA
                    case PrimaryOpCode.RRCA:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;

                    //RRA
                    case PrimaryOpCode.RRA:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, Xpr.A));
                        timingsBuilder.Add(1, 4);
                        break;

                    // ********* Jump *********
                    case PrimaryOpCode.JP:
                        yield return Expression.Assign(Xpr.PC, NextWord);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.JP_NZ:
                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return Expression.IfThenElse(Expression.Not(Xpr.Zero), Expression.Assign(Xpr.PC, Xpr.LocalWord), SyncProgramCounter);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.JP_Z:
                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return Expression.IfThenElse(Xpr.Zero, Expression.Assign(Xpr.PC, Xpr.LocalWord), SyncProgramCounter);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.JP_NC:
                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return Expression.IfThenElse(Expression.Not(Xpr.Carry), Expression.Assign(Xpr.PC, Xpr.LocalWord), SyncProgramCounter);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.JP_C:
                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return Expression.IfThenElse(Xpr.Carry, Expression.Assign(Xpr.PC, Xpr.LocalWord), SyncProgramCounter);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.JP_PO:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LD (FF00+C), A on GB
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Expression.Add(Expression.Convert(Xpr.C, typeof(ushort)), Expression.Constant((ushort)0xff00)), Xpr.A);
                            timingsBuilder.Add(2, 8);
                            break;
                        }

                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return Expression.IfThenElse(Expression.Not(Xpr.ParityOverflow), Expression.Assign(Xpr.PC, Xpr.LocalWord), SyncProgramCounter);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.JP_PE:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LD (nn), A on GB
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, NextWord, Xpr.A);
                            timingsBuilder.Add(2, 8);
                            break;
                        }

                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return Expression.IfThenElse(Xpr.ParityOverflow, Expression.Assign(Xpr.PC, Xpr.LocalWord), SyncProgramCounter);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;
                        
                    case PrimaryOpCode.JP_P:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LD A, (FF00+C) on GB
                            yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Expression.Add(Expression.Convert(Xpr.C, typeof(ushort)), Expression.Constant((ushort)0xff00))));
                            timingsBuilder.Add(2, 8);
                            break;
                        }

                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return Expression.IfThenElse(Expression.Not(Xpr.Sign), Expression.Assign(Xpr.PC, Xpr.LocalWord), SyncProgramCounter);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.JP_M:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LD A, (nn) on GB
                            yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, NextWord));
                            timingsBuilder.Add(4, 16);
                            break;
                        }

                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return Expression.IfThenElse(Xpr.Sign, Expression.Assign(Xpr.PC, Xpr.LocalWord), SyncProgramCounter);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.JR:
                        yield return Expression.Assign(Xpr.LocalByte, NextByte);
                        yield return Xpr.JumpToDisplacement;
                        timingsBuilder.Add(3, 12);
                        LastDecodeResult = DecodeResult.FinalizeAndSync;
                        yield break;

                    case PrimaryOpCode.JR_C:
                        yield return Expression.Assign(Xpr.LocalByte, NextByte);
                        yield return Expression.IfThen(Xpr.Carry, Expression.Block(Xpr.JumpToDisplacement, Xpr.GetDynamicTimings(1, 5)));
                        timingsBuilder.Add(2, 7);
                        LastDecodeResult = DecodeResult.FinalizeAndSync;
                        yield break;
                        
                    case PrimaryOpCode.JR_NC:
                        yield return Expression.Assign(Xpr.LocalByte, NextByte);
                        yield return Expression.IfThen(Expression.Not(Xpr.Carry), Expression.Block(Xpr.JumpToDisplacement, Xpr.GetDynamicTimings(1, 5)));
                        timingsBuilder.Add(2, 7);
                        LastDecodeResult = DecodeResult.FinalizeAndSync;
                        yield break;

                    case PrimaryOpCode.JR_Z:
                        yield return Expression.Assign(Xpr.LocalByte, NextByte);
                        yield return Expression.IfThen(Xpr.Zero, Expression.Block(Xpr.JumpToDisplacement, Xpr.GetDynamicTimings(1, 5)));
                        timingsBuilder.Add(2, 7);
                        LastDecodeResult = DecodeResult.FinalizeAndSync;
                        yield break;

                    case PrimaryOpCode.JR_NZ:
                        yield return Expression.Assign(Xpr.LocalByte, NextByte);
                        yield return Expression.IfThen(Expression.Not(Xpr.Zero), Expression.Block(Xpr.JumpToDisplacement, Xpr.GetDynamicTimings(1, 5)));
                        timingsBuilder.Add(2, 7);
                        LastDecodeResult = DecodeResult.FinalizeAndSync;
                        yield break;

                    case PrimaryOpCode.JP_mHL:
                        yield return Expression.Assign(Xpr.PC, index.Register);
                        timingsBuilder.Add(1, 4);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.DJNZ:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as STOP on GB
                            timingsBuilder.Add(1, 4);
                            LastDecodeResult = DecodeResult.Stop;
                            yield break;
                        }
                        else
                        {
                            yield return Expression.Assign(Xpr.LocalByte, NextByte);
                            yield return Expression.Assign(Xpr.B, Expression.Convert(Expression.Decrement(Expression.Convert(Xpr.B, typeof(int))), typeof(byte)));
                            yield return Expression.IfThen(Expression.NotEqual(Xpr.B, Expression.Constant((byte)0)), Expression.Block(Xpr.JumpToDisplacement, Xpr.GetDynamicTimings(1, 5)));
                            timingsBuilder.Add(2, 8);
                            LastDecodeResult = DecodeResult.FinalizeAndSync;
                            yield break;
                        }
                        break;
                        
                    // ********* Call *********
                    case PrimaryOpCode.CALL:
                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return SyncProgramCounter;
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, Xpr.LocalWord);
                        timingsBuilder.Add(5, 17);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.CALL_NZ:
                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return SyncProgramCounter;
                        yield return CallIf(Xpr.Zero, true);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;
                        
                    case PrimaryOpCode.CALL_Z:
                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return SyncProgramCounter;
                        yield return CallIf(Xpr.Zero);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.CALL_NC:
                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return SyncProgramCounter;
                        yield return CallIf(Xpr.Carry, true);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.CALL_C:
                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return SyncProgramCounter;
                        yield return CallIf(Xpr.Carry);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;
                        
                    case PrimaryOpCode.CALL_PO:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            timingsBuilder.Add(1, 4);
                            break;
                        }

                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return SyncProgramCounter;
                        yield return CallIf(Xpr.ParityOverflow, true);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.CALL_PE:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            timingsBuilder.Add(1, 4);
                            break;
                        }

                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return SyncProgramCounter;
                        yield return CallIf(Xpr.ParityOverflow);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;
                        
                    case PrimaryOpCode.CALL_P:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            timingsBuilder.Add(1, 4);
                            break;
                        }

                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return SyncProgramCounter;
                        yield return CallIf(Xpr.Sign, true);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.CALL_M:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            timingsBuilder.Add(1, 4);
                            break;
                        }

                        yield return Expression.Assign(Xpr.LocalWord, NextWord);
                        yield return SyncProgramCounter;
                        yield return CallIf(Xpr.Sign);
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    // ********* Return *********
                    case PrimaryOpCode.RET:
                        yield return Xpr.ReadPCFromStack;
                        yield return Xpr.PopPopSP;
                        timingsBuilder.Add(3, 10);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;
                        
                    case PrimaryOpCode.RET_NZ:
                        yield return SyncProgramCounter;
                        yield return ReturnIf(Xpr.Zero, true);
                        timingsBuilder.Add(1, 5);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RET_Z:
                        yield return SyncProgramCounter;
                        yield return ReturnIf(Xpr.Zero);
                        timingsBuilder.Add(1, 5);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RET_NC:
                        yield return SyncProgramCounter;
                        yield return ReturnIf(Xpr.Carry, true);
                        timingsBuilder.Add(1, 5);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RET_C:
                        yield return SyncProgramCounter;
                        yield return ReturnIf(Xpr.Carry);
                        timingsBuilder.Add(1, 5);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RET_PO:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LD (FF00+n), A on GB
                            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Expression.Add(Expression.Convert(NextByte, typeof(ushort)), Expression.Constant((ushort)0xff00)), Xpr.A);
                            timingsBuilder.Add(3, 12);
                            break;
                        }

                        yield return SyncProgramCounter;
                        yield return ReturnIf(Xpr.ParityOverflow, true);
                        timingsBuilder.Add(1, 5);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RET_PE:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as ADD SP, dd on GB
                            yield return Expression.Assign(Xpr.SP, Expression.Call(Xpr.Alu, Xpr.AluAddDisplacement, Xpr.SP, Expression.Convert(NextByte, typeof(sbyte))));
                            timingsBuilder.Add(4, 16);
                            break;
                        }
                        yield return SyncProgramCounter;
                        yield return ReturnIf(Xpr.ParityOverflow);
                        timingsBuilder.Add(1, 5);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RET_P:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LD A, (FF00+n) on GB
                            yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Expression.Add(Expression.Convert(NextByte, typeof(ushort)), Expression.Constant((ushort)0xff00))));
                            timingsBuilder.Add(3, 12);
                            break;
                        }

                        yield return SyncProgramCounter;
                        yield return ReturnIf(Xpr.Sign, true);
                        timingsBuilder.Add(1, 5);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RET_M:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // Runs as LD HL, SP+dd on GB
                            yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAddDisplacement, Xpr.SP, Expression.Convert(NextByte, typeof(sbyte))));
                            timingsBuilder.Add(3, 12);
                            break;
                        }

                        yield return SyncProgramCounter;
                        yield return ReturnIf(Xpr.Sign);
                        timingsBuilder.Add(1, 5);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    // ********* Reset *********
                    case PrimaryOpCode.RST_00:
                        yield return SyncProgramCounter;
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, Expression.Constant((ushort)0x0000));
                        timingsBuilder.Add(3, 11);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RST_08:
                        yield return SyncProgramCounter;
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, Expression.Constant((ushort)0x0008));
                        timingsBuilder.Add(3, 11);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RST_10:
                        yield return SyncProgramCounter;
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, Expression.Constant((ushort)0x0010));
                        timingsBuilder.Add(3, 11);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RST_18:
                        yield return SyncProgramCounter;
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, Expression.Constant((ushort)0x0018));
                        timingsBuilder.Add(3, 11);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RST_20:
                        yield return SyncProgramCounter;
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, Expression.Constant((ushort)0x0020));
                        timingsBuilder.Add(3, 11);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RST_28:
                        yield return SyncProgramCounter;
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, Expression.Constant((ushort)0x0028));
                        timingsBuilder.Add(3, 11);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RST_30:
                        yield return SyncProgramCounter;
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, Expression.Constant((ushort)0x0030));
                        timingsBuilder.Add(3, 11);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    case PrimaryOpCode.RST_38:
                        yield return SyncProgramCounter;
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, Expression.Constant((ushort)0x0038));
                        timingsBuilder.Add(3, 11);
                        LastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    // ********* IO *********
                    // IN A, (n)
                    case PrimaryOpCode.IN_A_n:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // No IO on GB
                            timingsBuilder.Add(1, 4);
                        }
                        else
                        {
                            yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.IO, Xpr.IoReadByte, NextByte, Xpr.A));
                            timingsBuilder.Add(3, 11);
                        }
                        break;
                    
                    // OUT A, (n)
                    case PrimaryOpCode.OUT_A_n:
                        if (this.cpuMode == CpuMode.GameBoy)
                        {
                            // No IO on GB
                            timingsBuilder.Add(1, 4);
                        }
                        else
                        {
                            yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, NextByte, Xpr.A, Xpr.A);
                            timingsBuilder.Add(3, 11);
                        }
                        break;

                    default:
                        throw new NotImplementedException(opCode.ToString());
                }

                // Set index back
                if (this.index.IndexRegister != IndexRegister.HL)
                {
                    this.index = Xpr.IndexRegisterExpressions[IndexRegister.HL];
                }
            }

            // Don't add anything here
        }
        
        /// <summary>
        /// Checks that when a DD or FD index prefix is applied whether the opcode will need a displacement adding to the index register i.e. (IX + d) and (IY + d) opcodes.
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        private static bool OpCodeUsesDisplacedIndex(PrimaryOpCode opCode)
        {
            switch (opCode)
            {
                case PrimaryOpCode.LD_A_mHL:
                case PrimaryOpCode.LD_B_mHL:
                case PrimaryOpCode.LD_C_mHL:
                case PrimaryOpCode.LD_D_mHL:
                case PrimaryOpCode.LD_E_mHL:
                case PrimaryOpCode.LD_H_mHL:
                case PrimaryOpCode.LD_L_mHL:
                case PrimaryOpCode.LD_mHL_A:
                case PrimaryOpCode.LD_mHL_B:
                case PrimaryOpCode.LD_mHL_C:
                case PrimaryOpCode.LD_mHL_D:
                case PrimaryOpCode.LD_mHL_E:
                case PrimaryOpCode.LD_mHL_H:
                case PrimaryOpCode.LD_mHL_L:
                case PrimaryOpCode.LD_mHL_n:
                case PrimaryOpCode.ADD_A_mHL:
                case PrimaryOpCode.ADC_A_mHL:
                case PrimaryOpCode.SUB_A_mHL:
                case PrimaryOpCode.SBC_A_mHL:
                case PrimaryOpCode.AND_mHL:
                case PrimaryOpCode.OR_mHL:
                case PrimaryOpCode.XOR_mHL:
                case PrimaryOpCode.CP_mHL:
                case PrimaryOpCode.INC_mHL:
                case PrimaryOpCode.DEC_mHL:
                    return true;
                default:
                    return false;
            }
        }
    }
}
