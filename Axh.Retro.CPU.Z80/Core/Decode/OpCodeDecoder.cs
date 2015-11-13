namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;
    using Axh.Retro.CPU.Z80.Core.Timing;

    internal partial class OpCodeDecoder
    {
        private readonly IDictionary<IndexRegister, IndexRegisterOperands> indexRegisterOperands;

        private readonly CpuMode cpuMode;

        private readonly IInstructionTimingsBuilder timer;

        private readonly IPrefetchQueue prefetch;

        private IndexRegisterOperands index;

        public OpCodeDecoder(CpuMode cpuMode, IPrefetchQueue prefetch)
        {
            this.timer = new InstructionTimingsBuilder();
            this.cpuMode = cpuMode;
            this.prefetch = prefetch;
            this.indexRegisterOperands = new Dictionary<IndexRegister, IndexRegisterOperands> { { IndexRegister.HL, new IndexRegisterOperands(Operand.HL, Operand.mHL, Operand.L, Operand.H, false) } };

            if (cpuMode == CpuMode.Z80)
            {
                indexRegisterOperands.Add(IndexRegister.IX, new IndexRegisterOperands(Operand.IX, Operand.mIXd, Operand.IXl, Operand.IXh, true));
                indexRegisterOperands.Add(IndexRegister.IY, new IndexRegisterOperands(Operand.IY, Operand.mIYd, Operand.IYl, Operand.IYh, true));
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
