namespace Axh.Retro.CPU.X80.Tests.Core.Z80InstructionDecoder
{
    using System;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class RotateTests : Z80InstructionDecoderTestsBase
    {

        [Test]
        public void RLCA()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x03;
            this.Alu.Setup(x => x.RotateLeftWithCarry(A)).Returns(Value);

            RunWithNOP(1, 4, PrimaryOpCode.RLCA);

            this.Alu.Verify(x => x.RotateLeftWithCarry(A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void RLA()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x27;
            this.Alu.Setup(x => x.RotateLeft(A)).Returns(Value);

            RunWithNOP(1, 4, PrimaryOpCode.RLA);

            this.Alu.Verify(x => x.RotateLeft(A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }
        
        [Test]
        public void RRCA()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xbc;
            this.Alu.Setup(x => x.RotateRightWithCarry(A)).Returns(Value);

            RunWithNOP(1, 4, PrimaryOpCode.RRCA);

            this.Alu.Verify(x => x.RotateRightWithCarry(A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void RRA()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xe3;
            this.Alu.Setup(x => x.RotateRight(A)).Returns(Value);

            RunWithNOP(1, 4, PrimaryOpCode.RRA);

            this.Alu.Verify(x => x.RotateRight(A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void RLD()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x43;
            const byte ExpectedAccumulator = 0x58;
            const byte ExpectedResult = 0x92;
            this.Alu.Setup(x => x.RotateLeftDigit(A, ValueAtHL)).Returns(new AccumulatorAndResult { Accumulator = ExpectedAccumulator, Result = ExpectedResult });
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(5, 18, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.RLD);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.RotateLeftDigit(A, ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, ExpectedResult), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = ExpectedAccumulator, Times.Once);
        }

        [Test]
        public void RRD()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x1b;
            const byte ExpectedAccumulator = 0x83;
            const byte ExpectedResult = 0x3a;
            this.Alu.Setup(x => x.RotateRightDigit(A, ValueAtHL)).Returns(new AccumulatorAndResult { Accumulator = ExpectedAccumulator, Result = ExpectedResult });
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(5, 18, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.RRD);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.RotateRightDigit(A, ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, ExpectedResult), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = ExpectedAccumulator, Times.Once);
        }

        [TestCase(PrefixCbOpCode.RLC_A)]
        [TestCase(PrefixCbOpCode.RLC_B)]
        [TestCase(PrefixCbOpCode.RLC_C)]
        [TestCase(PrefixCbOpCode.RLC_E)]
        [TestCase(PrefixCbOpCode.RLC_H)]
        [TestCase(PrefixCbOpCode.RLC_L)]
        public void RLC_r(PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x4e;
            this.Alu.Setup(x => x.RotateLeftWithCarry(It.IsAny<byte>())).Returns(Value);

            RunWithNOP(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.RLC_A:
                    this.Alu.Verify(x => x.RotateLeftWithCarry(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_B:
                    this.Alu.Verify(x => x.RotateLeftWithCarry(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_C:
                    this.Alu.Verify(x => x.RotateLeftWithCarry(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_D:
                    this.Alu.Verify(x => x.RotateLeftWithCarry(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_E:
                    this.Alu.Verify(x => x.RotateLeftWithCarry(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_H:
                    this.Alu.Verify(x => x.RotateLeftWithCarry(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_L:
                    this.Alu.Verify(x => x.RotateLeftWithCarry(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void RLC_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            this.Alu.Setup(x => x.RotateLeftWithCarry(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.RLC_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.RotateLeftWithCarry(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [TestCase(PrefixCbOpCode.RL_A)]
        [TestCase(PrefixCbOpCode.RL_B)]
        [TestCase(PrefixCbOpCode.RL_C)]
        [TestCase(PrefixCbOpCode.RL_E)]
        [TestCase(PrefixCbOpCode.RL_H)]
        [TestCase(PrefixCbOpCode.RL_L)]
        public void RL_r(PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x4e;
            this.Alu.Setup(x => x.RotateLeft(It.IsAny<byte>())).Returns(Value);

            RunWithNOP(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.RL_A:
                    this.Alu.Verify(x => x.RotateLeft(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RL_B:
                    this.Alu.Verify(x => x.RotateLeft(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RL_C:
                    this.Alu.Verify(x => x.RotateLeft(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RL_D:
                    this.Alu.Verify(x => x.RotateLeft(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RL_E:
                    this.Alu.Verify(x => x.RotateLeft(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RL_H:
                    this.Alu.Verify(x => x.RotateLeft(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RL_L:
                    this.Alu.Verify(x => x.RotateLeft(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void RL_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            this.Alu.Setup(x => x.RotateLeft(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.RL_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.RotateLeft(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RLC_A)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RLC_B)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RLC_C)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RLC_E)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RLC_H)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RLC_L)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RLC_A)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RLC_B)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RLC_C)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RLC_E)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RLC_H)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RLC_L)]
        public void RLC_r_mIXYd(PrimaryOpCode prefix, PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            this.Alu.Setup(x => x.RotateLeftWithCarry(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opcode);

            this.Alu.Verify(x => x.RotateLeftWithCarry(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            switch (opcode)
            {
                case PrefixCbOpCode.RLC_A:        
                    this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_B:
                    this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_C:
                    this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_D:
                    this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_E:
                    this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_H:
                    this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RLC_L:
                    this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RLC_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            this.Alu.Setup(x => x.RotateLeftWithCarry(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), PrefixCbOpCode.RLC_mHL);

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.RotateLeftWithCarry(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RL_A)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RL_B)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RL_C)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RL_E)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RL_H)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RL_L)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RL_A)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RL_B)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RL_C)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RL_E)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RL_H)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RL_L)]
        public void RL_r_mIXYd(PrimaryOpCode prefix, PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0xb5;
            const int Expected = 0x12;

            this.Alu.Setup(x => x.RotateLeft(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opcode);

            this.Alu.Verify(x => x.RotateLeft(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            switch (opcode)
            {
                case PrefixCbOpCode.RL_A:
                    this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RL_B:
                    this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RL_C:
                    this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RL_D:
                    this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RL_E:
                    this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RL_H:
                    this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RL_L:
                    this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RL_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            this.Alu.Setup(x => x.RotateLeft(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), PrefixCbOpCode.RL_mHL);

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.RotateLeft(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

        [TestCase(PrefixCbOpCode.RRC_A)]
        [TestCase(PrefixCbOpCode.RRC_B)]
        [TestCase(PrefixCbOpCode.RRC_C)]
        [TestCase(PrefixCbOpCode.RRC_E)]
        [TestCase(PrefixCbOpCode.RRC_H)]
        [TestCase(PrefixCbOpCode.RRC_L)]
        public void RRC_r(PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x4e;
            this.Alu.Setup(x => x.RotateRightWithCarry(It.IsAny<byte>())).Returns(Value);

            RunWithNOP(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.RRC_A:
                    this.Alu.Verify(x => x.RotateRightWithCarry(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_B:
                    this.Alu.Verify(x => x.RotateRightWithCarry(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_C:
                    this.Alu.Verify(x => x.RotateRightWithCarry(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_D:
                    this.Alu.Verify(x => x.RotateRightWithCarry(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_E:
                    this.Alu.Verify(x => x.RotateRightWithCarry(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_H:
                    this.Alu.Verify(x => x.RotateRightWithCarry(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_L:
                    this.Alu.Verify(x => x.RotateRightWithCarry(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void RRC_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            this.Alu.Setup(x => x.RotateRightWithCarry(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.RRC_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.RotateRightWithCarry(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [TestCase(PrefixCbOpCode.RR_A)]
        [TestCase(PrefixCbOpCode.RR_B)]
        [TestCase(PrefixCbOpCode.RR_C)]
        [TestCase(PrefixCbOpCode.RR_E)]
        [TestCase(PrefixCbOpCode.RR_H)]
        [TestCase(PrefixCbOpCode.RR_L)]
        public void RR_r(PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x4e;
            this.Alu.Setup(x => x.RotateRight(It.IsAny<byte>())).Returns(Value);

            RunWithNOP(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.RR_A:
                    this.Alu.Verify(x => x.RotateRight(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RR_B:
                    this.Alu.Verify(x => x.RotateRight(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RR_C:
                    this.Alu.Verify(x => x.RotateRight(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RR_D:
                    this.Alu.Verify(x => x.RotateRight(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RR_E:
                    this.Alu.Verify(x => x.RotateRight(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RR_H:
                    this.Alu.Verify(x => x.RotateRight(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixCbOpCode.RR_L:
                    this.Alu.Verify(x => x.RotateRight(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void RR_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            this.Alu.Setup(x => x.RotateRight(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.RR_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.RotateRight(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RRC_A)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RRC_B)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RRC_C)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RRC_E)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RRC_H)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RRC_L)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RRC_A)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RRC_B)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RRC_C)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RRC_E)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RRC_H)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RRC_L)]
        public void RRC_r_mIXYd(PrimaryOpCode prefix, PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            this.Alu.Setup(x => x.RotateRightWithCarry(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opcode);

            this.Alu.Verify(x => x.RotateRightWithCarry(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            switch (opcode)
            {
                case PrefixCbOpCode.RRC_A:
                    this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_B:
                    this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_C:
                    this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_D:
                    this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_E:
                    this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_H:
                    this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RRC_L:
                    this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RRC_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            this.Alu.Setup(x => x.RotateRightWithCarry(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), PrefixCbOpCode.RRC_mHL);

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.RotateRightWithCarry(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RR_A)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RR_B)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RR_C)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RR_E)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RR_H)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.RR_L)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RR_A)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RR_B)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RR_C)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RR_E)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RR_H)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.RR_L)]
        public void RR_r_mIXYd(PrimaryOpCode prefix, PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0xb5;
            const int Expected = 0x12;

            this.Alu.Setup(x => x.RotateRight(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opcode);

            this.Alu.Verify(x => x.RotateRight(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            switch (opcode)
            {
                case PrefixCbOpCode.RR_A:
                    this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RR_B:
                    this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RR_C:
                    this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RR_D:
                    this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RR_E:
                    this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RR_H:
                    this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.RR_L:
                    this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RR_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            this.Alu.Setup(x => x.RotateRight(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), PrefixCbOpCode.RR_mHL);

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.RotateRight(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

    }
}
