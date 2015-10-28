namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ShiftTests : InstructionBlockDecoderTestsBase
    {
        [TestCase(PrefixCbOpCode.SLA_A)]
        [TestCase(PrefixCbOpCode.SLA_B)]
        [TestCase(PrefixCbOpCode.SLA_C)]
        [TestCase(PrefixCbOpCode.SLA_E)]
        [TestCase(PrefixCbOpCode.SLA_H)]
        [TestCase(PrefixCbOpCode.SLA_L)]
        public void SLA_r(PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x4e;
            this.Alu.Setup(x => x.ShiftLeft(It.IsAny<byte>())).Returns(Value);

            RunWithNOP(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.SLA_A:
                    this.Alu.Verify(x => x.ShiftLeft(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_B:
                    this.Alu.Verify(x => x.ShiftLeft(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_C:
                    this.Alu.Verify(x => x.ShiftLeft(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_D:
                    this.Alu.Verify(x => x.ShiftLeft(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_E:
                    this.Alu.Verify(x => x.ShiftLeft(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_H:
                    this.Alu.Verify(x => x.ShiftLeft(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_L:
                    this.Alu.Verify(x => x.ShiftLeft(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SLA_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            this.Alu.Setup(x => x.ShiftLeft(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.SLA_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.ShiftLeft(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [TestCase(PrefixCbOpCode.SLS_A)]
        [TestCase(PrefixCbOpCode.SLS_B)]
        [TestCase(PrefixCbOpCode.SLS_C)]
        [TestCase(PrefixCbOpCode.SLS_E)]
        [TestCase(PrefixCbOpCode.SLS_H)]
        [TestCase(PrefixCbOpCode.SLS_L)]
        public void SLS_r(PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x4e;
            this.Alu.Setup(x => x.ShiftLeftSet(It.IsAny<byte>())).Returns(Value);

            RunWithNOP(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.SLS_A:
                    this.Alu.Verify(x => x.ShiftLeftSet(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_B:
                    this.Alu.Verify(x => x.ShiftLeftSet(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_C:
                    this.Alu.Verify(x => x.ShiftLeftSet(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_D:
                    this.Alu.Verify(x => x.ShiftLeftSet(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_E:
                    this.Alu.Verify(x => x.ShiftLeftSet(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_H:
                    this.Alu.Verify(x => x.ShiftLeftSet(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_L:
                    this.Alu.Verify(x => x.ShiftLeftSet(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SLS_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            this.Alu.Setup(x => x.ShiftLeftSet(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.SLS_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.ShiftLeftSet(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [TestCase(PrefixCbOpCode.SRA_A)]
        [TestCase(PrefixCbOpCode.SRA_B)]
        [TestCase(PrefixCbOpCode.SRA_C)]
        [TestCase(PrefixCbOpCode.SRA_E)]
        [TestCase(PrefixCbOpCode.SRA_H)]
        [TestCase(PrefixCbOpCode.SRA_L)]
        public void SRA_r(PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x4e;
            this.Alu.Setup(x => x.ShiftRight(It.IsAny<byte>())).Returns(Value);

            RunWithNOP(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.SRA_A:
                    this.Alu.Verify(x => x.ShiftRight(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_B:
                    this.Alu.Verify(x => x.ShiftRight(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_C:
                    this.Alu.Verify(x => x.ShiftRight(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_D:
                    this.Alu.Verify(x => x.ShiftRight(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_E:
                    this.Alu.Verify(x => x.ShiftRight(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_H:
                    this.Alu.Verify(x => x.ShiftRight(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_L:
                    this.Alu.Verify(x => x.ShiftRight(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SRA_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            this.Alu.Setup(x => x.ShiftRight(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.SRA_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.ShiftRight(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [TestCase(PrefixCbOpCode.SRL_A)]
        [TestCase(PrefixCbOpCode.SRL_B)]
        [TestCase(PrefixCbOpCode.SRL_C)]
        [TestCase(PrefixCbOpCode.SRL_E)]
        [TestCase(PrefixCbOpCode.SRL_H)]
        [TestCase(PrefixCbOpCode.SRL_L)]
        public void SRL_r(PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x4e;
            this.Alu.Setup(x => x.ShiftRightLogical(It.IsAny<byte>())).Returns(Value);

            RunWithNOP(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.SRL_A:
                    this.Alu.Verify(x => x.ShiftRightLogical(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_B:
                    this.Alu.Verify(x => x.ShiftRightLogical(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_C:
                    this.Alu.Verify(x => x.ShiftRightLogical(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_D:
                    this.Alu.Verify(x => x.ShiftRightLogical(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_E:
                    this.Alu.Verify(x => x.ShiftRightLogical(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_H:
                    this.Alu.Verify(x => x.ShiftRightLogical(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_L:
                    this.Alu.Verify(x => x.ShiftRightLogical(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SRL_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            this.Alu.Setup(x => x.ShiftRightLogical(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithNOP(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.SRL_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.ShiftRightLogical(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLA_A)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLA_B)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLA_C)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLA_E)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLA_H)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLA_L)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLA_A)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLA_B)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLA_C)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLA_E)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLA_H)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLA_L)]
        public void SLA_r_mIXYd(PrimaryOpCode prefix, PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            this.Alu.Setup(x => x.ShiftLeft(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opcode);

            this.Alu.Verify(x => x.ShiftLeft(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            switch (opcode)
            {
                case PrefixCbOpCode.SLA_A:
                    this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_B:
                    this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_C:
                    this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_D:
                    this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_E:
                    this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_H:
                    this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLA_L:
                    this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SLA_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            this.Alu.Setup(x => x.ShiftLeft(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), PrefixCbOpCode.SLA_mHL);

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.ShiftLeft(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLS_A)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLS_B)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLS_C)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLS_E)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLS_H)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SLS_L)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLS_A)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLS_B)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLS_C)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLS_E)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLS_H)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SLS_L)]
        public void SLS_r_mIXYd(PrimaryOpCode prefix, PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            this.Alu.Setup(x => x.ShiftLeftSet(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opcode);

            this.Alu.Verify(x => x.ShiftLeftSet(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            switch (opcode)
            {
                case PrefixCbOpCode.SLS_A:
                    this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_B:
                    this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_C:
                    this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_D:
                    this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_E:
                    this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_H:
                    this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SLS_L:
                    this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SLS_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            this.Alu.Setup(x => x.ShiftLeftSet(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), PrefixCbOpCode.SLS_mHL);

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.ShiftLeftSet(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRA_A)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRA_B)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRA_C)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRA_E)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRA_H)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRA_L)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRA_A)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRA_B)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRA_C)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRA_E)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRA_H)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRA_L)]
        public void SRA_r_mIXYd(PrimaryOpCode prefix, PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            this.Alu.Setup(x => x.ShiftRight(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opcode);

            this.Alu.Verify(x => x.ShiftRight(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            switch (opcode)
            {
                case PrefixCbOpCode.SRA_A:
                    this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_B:
                    this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_C:
                    this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_D:
                    this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_E:
                    this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_H:
                    this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRA_L:
                    this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SRA_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            this.Alu.Setup(x => x.ShiftRight(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), PrefixCbOpCode.SRA_mHL);

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.ShiftRight(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }
        
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRL_A)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRL_B)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRL_C)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRL_E)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRL_H)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrefixCbOpCode.SRL_L)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRL_A)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRL_B)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRL_C)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRL_E)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRL_H)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrefixCbOpCode.SRL_L)]
        public void SRL_r_mIXYd(PrimaryOpCode prefix, PrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            this.Alu.Setup(x => x.ShiftRightLogical(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opcode);

            this.Alu.Verify(x => x.ShiftRightLogical(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            switch (opcode)
            {
                case PrefixCbOpCode.SRL_A:
                    this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_B:
                    this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_C:
                    this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_D:
                    this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_E:
                    this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_H:
                    this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                    break;
                case PrefixCbOpCode.SRL_L:
                    this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SRL_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            this.Alu.Setup(x => x.ShiftRightLogical(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithNOP(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), PrefixCbOpCode.SRL_mHL);

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.ShiftRightLogical(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

    }
}
