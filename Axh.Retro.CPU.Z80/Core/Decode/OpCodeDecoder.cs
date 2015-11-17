namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System.Collections.Generic;

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

        public OpCodeDecoder(IPlatformConfig platformConfig, IPrefetchQueue prefetch, IInstructionTimingsBuilder timer)
        {
            this.cpuMode = platformConfig.CpuMode;
            this.undefinedInstruction = platformConfig.LockOnUndefinedInstruction ? OpCode.Halt : OpCode.NoOperation;
            this.prefetch = prefetch;
            this.timer = timer;
            this.indexRegisterOperands = new Dictionary<IndexRegister, IndexRegisterOperands> { { IndexRegister.HL, new IndexRegisterOperands(Operand.HL, Operand.mHL, Operand.L, Operand.H, false) } };

            if (cpuMode == CpuMode.Z80)
            {
                indexRegisterOperands.Add(IndexRegister.IX, new IndexRegisterOperands(Operand.IX, Operand.mIXd, Operand.IXl, Operand.IXh, true));
                indexRegisterOperands.Add(IndexRegister.IY, new IndexRegisterOperands(Operand.IY, Operand.mIYd, Operand.IYl, Operand.IYh, true));
            }
        }

        public IEnumerable<Operation> DecodeNextBlock(ushort address)
        {
            timer.Reset();
            index = indexRegisterOperands[IndexRegister.HL];
            
            while (true)
            {
                var result = DecodePrimary();

                if (result == null)
                {
                    continue;
                }

                if (result.OpCodeMeta.HasFlag(OpCodeMeta.Displacement) && index.IsDisplaced)
                {
                    result.AddDisplacement(prefetch.NextByte());
                }

                if (result.OpCodeMeta.HasFlag(OpCodeMeta.ByteLiteral))
                {
                    result.AddLiteral(prefetch.NextByte());
                }

                if (result.OpCodeMeta.HasFlag(OpCodeMeta.WordLiteral))
                {
                    result.AddLiteral(prefetch.NextWord());
                }

                result.Address = address;
                yield return result;

                if (result.OpCodeMeta.HasFlag(OpCodeMeta.EndBlock))
                {
                    yield break;
                }

                index = this.indexRegisterOperands[IndexRegister.HL];
                address += (ushort)(prefetch.TotalBytesRead - address);
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
