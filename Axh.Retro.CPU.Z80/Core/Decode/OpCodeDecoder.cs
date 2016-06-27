﻿using Axh.Retro.CPU.Z80.Timing;

namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;

    internal partial class OpCodeDecoder
    {
        private readonly IDictionary<IndexRegister, IndexRegisterOperands> indexRegisterOperands;

        private readonly CpuMode cpuMode;

        private readonly OpCode undefinedInstruction;

        private readonly IInstructionTimingsBuilder timer;

        private readonly IPrefetchQueue prefetch;

        private IndexRegisterOperands index;
        
        private Operand operand1;
        private Operand operand2;
        private FlagTest flagTest;
        private OpCodeMeta opCodeMeta;
        private byte byteLiteral;
        private ushort wordLiteral;
        private byte displacement;
        
        public OpCodeDecoder(IPlatformConfig platformConfig, IPrefetchQueue prefetch)
        {
            this.cpuMode = platformConfig.CpuMode;
            this.undefinedInstruction = platformConfig.LockOnUndefinedInstruction ? OpCode.Halt : OpCode.NoOperation;
            this.prefetch = prefetch;
            this.timer = new InstructionTimingsBuilder();
            this.indexRegisterOperands = new Dictionary<IndexRegister, IndexRegisterOperands> { { IndexRegister.HL, new IndexRegisterOperands(Operand.HL, Operand.mHL, Operand.L, Operand.H, false) } };

            if (cpuMode == CpuMode.Z80)
            {
                indexRegisterOperands.Add(IndexRegister.IX, new IndexRegisterOperands(Operand.IX, Operand.mIXd, Operand.IXl, Operand.IXh, true));
                indexRegisterOperands.Add(IndexRegister.IY, new IndexRegisterOperands(Operand.IY, Operand.mIYd, Operand.IYl, Operand.IYh, true));
            }
        }

        public DecodedBlock GetNextBlock(ushort address)
        {
            var operations = this.DecodeOperations(address);
            return new DecodedBlock(operations.ToArray(), this.timer.GetInstructionTimings());
        }

        public IEnumerable<Operation> DecodeOperations(ushort address)
        {
            timer.Reset();
            index = indexRegisterOperands[IndexRegister.HL];
            this.prefetch.ReBuildCache(address);
            var totalBytesRead = 0;

            while (true)
            {
                // Reset
                operand1 = Operand.None;
                operand2 = Operand.None;
                flagTest = FlagTest.None;
                opCodeMeta = OpCodeMeta.None;
                byteLiteral = 0x00;
                wordLiteral = 0x0000;
                displacement = 0x00;
                
                var result = DecodePrimary();

                if (!result.HasValue)
                {
                    continue;
                }

                if (opCodeMeta.HasFlag(OpCodeMeta.Displacement) && index.IsDisplaced)
                {
                    displacement = prefetch.NextByte();
                }

                if (opCodeMeta.HasFlag(OpCodeMeta.ByteLiteral))
                {
                    byteLiteral = prefetch.NextByte();
                }

                if (opCodeMeta.HasFlag(OpCodeMeta.WordLiteral))
                {
                    wordLiteral = prefetch.NextWord();
                }
                
                yield return new Operation(address, result.Value, operand1, operand2, flagTest, opCodeMeta, byteLiteral, wordLiteral, (sbyte)displacement);

                if (opCodeMeta.HasFlag(OpCodeMeta.EndBlock))
                {
                    yield break;
                }
                
                index = this.indexRegisterOperands[IndexRegister.HL];
                var opBytes = prefetch.TotalBytesRead - totalBytesRead;
                address += (ushort)(opBytes);
                totalBytesRead += opBytes;
            }
        }
        
        private struct IndexRegisterOperands
        {
            public IndexRegisterOperands(Operand register, Operand index, Operand lowRegister, Operand highRegister, bool isDisplaced) : this()
            {
                Register = register;
                Index = index;
                LowRegister = lowRegister;
                HighRegister = highRegister;
                IsDisplaced = isDisplaced;
            }

            public Operand Register { get; }

            public Operand Index { get; }

            public Operand LowRegister { get; }

            public Operand HighRegister { get; }

            public bool IsDisplaced { get; }
        }
    }
}
