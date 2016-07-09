using System.Collections.Generic;
using System.Linq;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Timing;
using Axh.Retro.CPU.Z80.Timing;

namespace Axh.Retro.CPU.Z80.Core.Decode
{
    /// <summary>
    /// Core op-code decoder.
    /// This will decode blocks of raw 8080/GBCPU/Z80 operands from a prefetch queue as collections of <see cref="DecodedBlock"/>.
    /// Blocks begin at the address specified when calling <see cref="DecodeNextBlock"/> and end with an operand that could potentially change the value of the PC register.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="OpCodeDecoder"/> class.
        /// </summary>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <param name="prefetch">The prefetch.</param>
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
                // Initi Z80 specific index registers operands.
                _indexRegisterOperands.Add(IndexRegister.IX,
                                           new IndexRegisterOperands(Operand.IX, Operand.mIXd, Operand.IXl, Operand.IXh, true));
                _indexRegisterOperands.Add(IndexRegister.IY,
                                           new IndexRegisterOperands(Operand.IY, Operand.mIYd, Operand.IYl, Operand.IYh, true));
            }
        }

        /// <summary>
        /// Decodes the next block.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public DecodedBlock DecodeNextBlock(ushort address)
        {
            var operations = DecodeOperations(address);
            return new DecodedBlock(operations.ToArray(), _timer.GetInstructionTimings());
        }

        /// <summary>
        /// Decodes the operations at the specific address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
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

                var opCode = DecodeNextOpCode();

                if (!opCode.HasValue)
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
                                  opCode.Value,
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
    }
}