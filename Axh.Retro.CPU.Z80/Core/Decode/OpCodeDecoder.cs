using System.Collections.Generic;
using System.Linq;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
using Axh.Retro.CPU.Z80.Timing;

namespace Axh.Retro.CPU.Z80.Core.Decode
{
    internal partial class OpCodeDecoder
    {
        private readonly CpuMode _cpuMode;
        private readonly IDictionary<IndexRegister, IndexRegisterOperands> _indexRegisterOperands;

        private readonly IPrefetchQueue _prefetch;

        private readonly IInstructionTimingsBuilder _timer;

        private readonly OpCode _undefinedInstruction;
        private byte _byteLiteral;
        private byte _displacement;
        private FlagTest _flagTest;

        private IndexRegisterOperands _index;
        private OpCodeMeta _opCodeMeta;

        private Operand _operand1;
        private Operand _operand2;
        private ushort _wordLiteral;

        public OpCodeDecoder(IPlatformConfig platformConfig, IPrefetchQueue prefetch)
        {
            _cpuMode = platformConfig.CpuMode;
            _undefinedInstruction = platformConfig.LockOnUndefinedInstruction ? OpCode.Halt : OpCode.NoOperation;
            _prefetch = prefetch;
            _timer = new InstructionTimingsBuilder();
            _indexRegisterOperands = new Dictionary<IndexRegister, IndexRegisterOperands>
            {
                {
                    IndexRegister.HL,
                    new IndexRegisterOperands(
                        Operand.HL,
                        Operand.mHL,
                        Operand.L,
                        Operand.H,
                        false)
                }
            };

            if (_cpuMode == CpuMode.Z80)
            {
                _indexRegisterOperands.Add(IndexRegister.IX,
                    new IndexRegisterOperands(Operand.IX,
                        Operand.mIXd,
                        Operand.IXl,
                        Operand.IXh,
                        true));
                _indexRegisterOperands.Add(IndexRegister.IY,
                    new IndexRegisterOperands(Operand.IY,
                        Operand.mIYd,
                        Operand.IYl,
                        Operand.IYh,
                        true));
            }
        }

        public DecodedBlock GetNextBlock(ushort address)
        {
            var operations = DecodeOperations(address);
            return new DecodedBlock(operations.ToArray(), _timer.GetInstructionTimings());
        }

        public IEnumerable<Operation> DecodeOperations(ushort address)
        {
            _timer.Reset();
            _index = _indexRegisterOperands[IndexRegister.HL];
            _prefetch.ReBuildCache(address);
            var totalBytesRead = 0;

            while (true)
            {
                // Reset
                _operand1 = Operand.None;
                _operand2 = Operand.None;
                _flagTest = FlagTest.None;
                _opCodeMeta = OpCodeMeta.None;
                _byteLiteral = 0x00;
                _wordLiteral = 0x0000;
                _displacement = 0x00;

                var result = DecodePrimary();

                if (!result.HasValue)
                {
                    continue;
                }

                if (_opCodeMeta.HasFlag(OpCodeMeta.Displacement) && _index.IsDisplaced)
                {
                    _displacement = _prefetch.NextByte();
                }

                if (_opCodeMeta.HasFlag(OpCodeMeta.ByteLiteral))
                {
                    _byteLiteral = _prefetch.NextByte();
                }

                if (_opCodeMeta.HasFlag(OpCodeMeta.WordLiteral))
                {
                    _wordLiteral = _prefetch.NextWord();
                }

                yield return
                    new Operation(address,
                        result.Value,
                        _operand1,
                        _operand2,
                        _flagTest,
                        _opCodeMeta,
                        _byteLiteral,
                        _wordLiteral,
                        (sbyte) _displacement);

                if (_opCodeMeta.HasFlag(OpCodeMeta.EndBlock))
                {
                    yield break;
                }

                _index = _indexRegisterOperands[IndexRegister.HL];
                var opBytes = _prefetch.TotalBytesRead - totalBytesRead;
                address += (ushort) opBytes;
                totalBytesRead += opBytes;
            }
        }

        private struct IndexRegisterOperands
        {
            public IndexRegisterOperands(Operand register,
                Operand index,
                Operand lowRegister,
                Operand highRegister,
                bool isDisplaced) : this()
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