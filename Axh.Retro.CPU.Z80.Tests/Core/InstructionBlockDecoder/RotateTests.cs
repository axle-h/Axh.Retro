using System;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Tests.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class RotateTests : InstructionBlockDecoderTestsBase
    {
        public RotateTests() : base(CpuMode.Z80)
        {
        }

        [TestCase(PrefixCbOpCode.RLC_A)]
        [TestCase(PrefixCbOpCode.RLC_B)]
        [TestCase(PrefixCbOpCode.RLC_C)]
        [TestCase(PrefixCbOpCode.RLC_E)]
        [TestCase(PrefixCbOpCode.RLC_H)]
        [TestCase(PrefixCbOpCode.RLC_L)]
        public void RLC_r(PrefixCbOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x4e;
            Alu.Setup(x => x.RotateLeftWithCarry(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.RLC_A:
                    Alu.Verify(x => x.RotateLeftWithCarry(A), Times.Once);
                    Assert.AreEqual(Value, AfRegisters.A);
                    break;
                case PrefixCbOpCode.RLC_B:
                    Alu.Verify(x => x.RotateLeftWithCarry(B), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.B);
                    break;
                case PrefixCbOpCode.RLC_C:
                    Alu.Verify(x => x.RotateLeftWithCarry(C), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.C);
                    break;
                case PrefixCbOpCode.RLC_D:
                    Alu.Verify(x => x.RotateLeftWithCarry(D), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.D);
                    break;
                case PrefixCbOpCode.RLC_E:
                    Alu.Verify(x => x.RotateLeftWithCarry(E), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.E);
                    break;
                case PrefixCbOpCode.RLC_H:
                    Alu.Verify(x => x.RotateLeftWithCarry(H), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.H);
                    break;
                case PrefixCbOpCode.RLC_L:
                    Alu.Verify(x => x.RotateLeftWithCarry(L), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrefixCbOpCode.RL_A)]
        [TestCase(PrefixCbOpCode.RL_B)]
        [TestCase(PrefixCbOpCode.RL_C)]
        [TestCase(PrefixCbOpCode.RL_E)]
        [TestCase(PrefixCbOpCode.RL_H)]
        [TestCase(PrefixCbOpCode.RL_L)]
        public void RL_r(PrefixCbOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x4e;
            Alu.Setup(x => x.RotateLeft(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.RL_A:
                    Alu.Verify(x => x.RotateLeft(A), Times.Once);
                    Assert.AreEqual(Value, AfRegisters.A);
                    break;
                case PrefixCbOpCode.RL_B:
                    Alu.Verify(x => x.RotateLeft(B), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.B);
                    break;
                case PrefixCbOpCode.RL_C:
                    Alu.Verify(x => x.RotateLeft(C), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.C);
                    break;
                case PrefixCbOpCode.RL_D:
                    Alu.Verify(x => x.RotateLeft(D), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.D);
                    break;
                case PrefixCbOpCode.RL_E:
                    Alu.Verify(x => x.RotateLeft(E), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.E);
                    break;
                case PrefixCbOpCode.RL_H:
                    Alu.Verify(x => x.RotateLeft(H), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.H);
                    break;
                case PrefixCbOpCode.RL_L:
                    Alu.Verify(x => x.RotateLeft(L), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            Alu.Setup(x => x.RotateLeftWithCarry(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), opcode);

            Alu.Verify(x => x.RotateLeftWithCarry(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            AssertRotate(ValueAtIndex, ParseRegister(opcode));
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RLC_mIXYd(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            Alu.Setup(x => x.RotateLeftWithCarry(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), PrefixCbOpCode.RLC_mHL);

            Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            Alu.Verify(x => x.RotateLeftWithCarry(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
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
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0xb5;
            const int Expected = 0x12;

            Alu.Setup(x => x.RotateLeft(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), opcode);

            Alu.Verify(x => x.RotateLeft(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            AssertRotate(ValueAtIndex, ParseRegister(opcode));
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RL_mIXYd(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            Alu.Setup(x => x.RotateLeft(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), PrefixCbOpCode.RL_mHL);

            Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            Alu.Verify(x => x.RotateLeft(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

        [TestCase(PrefixCbOpCode.RRC_A)]
        [TestCase(PrefixCbOpCode.RRC_B)]
        [TestCase(PrefixCbOpCode.RRC_C)]
        [TestCase(PrefixCbOpCode.RRC_E)]
        [TestCase(PrefixCbOpCode.RRC_H)]
        [TestCase(PrefixCbOpCode.RRC_L)]
        public void RRC_r(PrefixCbOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x4e;
            Alu.Setup(x => x.RotateRightWithCarry(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.RRC_A:
                    Alu.Verify(x => x.RotateRightWithCarry(A), Times.Once);
                    Assert.AreEqual(Value, AfRegisters.A);
                    break;
                case PrefixCbOpCode.RRC_B:
                    Alu.Verify(x => x.RotateRightWithCarry(B), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.B);
                    break;
                case PrefixCbOpCode.RRC_C:
                    Alu.Verify(x => x.RotateRightWithCarry(C), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.C);
                    break;
                case PrefixCbOpCode.RRC_D:
                    Alu.Verify(x => x.RotateRightWithCarry(D), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.D);
                    break;
                case PrefixCbOpCode.RRC_E:
                    Alu.Verify(x => x.RotateRightWithCarry(E), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.E);
                    break;
                case PrefixCbOpCode.RRC_H:
                    Alu.Verify(x => x.RotateRightWithCarry(H), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.H);
                    break;
                case PrefixCbOpCode.RRC_L:
                    Alu.Verify(x => x.RotateRightWithCarry(L), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrefixCbOpCode.RR_A)]
        [TestCase(PrefixCbOpCode.RR_B)]
        [TestCase(PrefixCbOpCode.RR_C)]
        [TestCase(PrefixCbOpCode.RR_E)]
        [TestCase(PrefixCbOpCode.RR_H)]
        [TestCase(PrefixCbOpCode.RR_L)]
        public void RR_r(PrefixCbOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x4e;
            Alu.Setup(x => x.RotateRight(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.RR_A:
                    Alu.Verify(x => x.RotateRight(A), Times.Once);
                    Assert.AreEqual(Value, AfRegisters.A);
                    break;
                case PrefixCbOpCode.RR_B:
                    Alu.Verify(x => x.RotateRight(B), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.B);
                    break;
                case PrefixCbOpCode.RR_C:
                    Alu.Verify(x => x.RotateRight(C), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.C);
                    break;
                case PrefixCbOpCode.RR_D:
                    Alu.Verify(x => x.RotateRight(D), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.D);
                    break;
                case PrefixCbOpCode.RR_E:
                    Alu.Verify(x => x.RotateRight(E), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.E);
                    break;
                case PrefixCbOpCode.RR_H:
                    Alu.Verify(x => x.RotateRight(H), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.H);
                    break;
                case PrefixCbOpCode.RR_L:
                    Alu.Verify(x => x.RotateRight(L), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            Alu.Setup(x => x.RotateRightWithCarry(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), opcode);

            Alu.Verify(x => x.RotateRightWithCarry(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            AssertRotate(ValueAtIndex, ParseRegister(opcode));
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RRC_mIXYd(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            Alu.Setup(x => x.RotateRightWithCarry(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), PrefixCbOpCode.RRC_mHL);

            Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            Alu.Verify(x => x.RotateRightWithCarry(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
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
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0xb5;
            const int Expected = 0x12;

            Alu.Setup(x => x.RotateRight(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), opcode);

            Alu.Verify(x => x.RotateRight(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            AssertRotate(ValueAtIndex, ParseRegister(opcode));
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RR_mIXYd(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            Alu.Setup(x => x.RotateRight(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), PrefixCbOpCode.RR_mHL);

            Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            Alu.Verify(x => x.RotateRight(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

        [Test]
        public void RL_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            Alu.Setup(x => x.RotateLeft(ValueAtHL)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.RL_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.RotateLeft(ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [Test]
        public void RLA()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x27;
            Alu.Setup(x => x.RotateLeft(A)).Returns(Value);

            RunWithHalt(1, 4, PrimaryOpCode.RLA);

            Alu.Verify(x => x.RotateLeft(A), Times.Once);
            Assert.AreEqual(Value, AfRegisters.A);
        }

        [Test]
        public void RLC_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            Alu.Setup(x => x.RotateLeftWithCarry(ValueAtHL)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.RLC_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.RotateLeftWithCarry(ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [Test]
        public void RLCA()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x03;
            Alu.Setup(x => x.RotateLeftWithCarry(A)).Returns(Value);

            RunWithHalt(1, 4, PrimaryOpCode.RLCA);

            Alu.Verify(x => x.RotateLeftWithCarry(A), Times.Once);
            Assert.AreEqual(Value, AfRegisters.A);
        }

        [Test]
        public void RLD()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x43;
            const byte ExpectedAccumulator = 0x58;
            const byte ExpectedResult = 0x92;
            Alu.Setup(x => x.RotateLeftDigit(A, ValueAtHL))
               .Returns(new AccumulatorAndResult(ExpectedAccumulator, ExpectedResult));
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(5, 18, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.RLD);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.RotateLeftDigit(A, ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, ExpectedResult), Times.Once);
            Assert.AreEqual(ExpectedAccumulator, AfRegisters.A);
        }

        [Test]
        public void RR_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            Alu.Setup(x => x.RotateRight(ValueAtHL)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.RR_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.RotateRight(ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [Test]
        public void RRA()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xe3;
            Alu.Setup(x => x.RotateRight(A)).Returns(Value);

            RunWithHalt(1, 4, PrimaryOpCode.RRA);

            Alu.Verify(x => x.RotateRight(A), Times.Once);
            Assert.AreEqual(Value, AfRegisters.A);
        }

        [Test]
        public void RRC_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            Alu.Setup(x => x.RotateRightWithCarry(ValueAtHL)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.RRC_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.RotateRightWithCarry(ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [Test]
        public void RRCA()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xbc;
            Alu.Setup(x => x.RotateRightWithCarry(A)).Returns(Value);

            RunWithHalt(1, 4, PrimaryOpCode.RRCA);

            Alu.Verify(x => x.RotateRightWithCarry(A), Times.Once);
            Assert.AreEqual(Value, AfRegisters.A);
        }

        [Test]
        public void RRD()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x1b;
            const byte ExpectedAccumulator = 0x83;
            const byte ExpectedResult = 0x3a;
            Alu.Setup(x => x.RotateRightDigit(A, ValueAtHL))
               .Returns(new AccumulatorAndResult(ExpectedAccumulator, ExpectedResult));
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(5, 18, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.RRD);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.RotateRightDigit(A, ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, ExpectedResult), Times.Once);
            Assert.AreEqual(ExpectedAccumulator, AfRegisters.A);
        }

        private static CpuRegister ParseRegister(PrefixCbOpCode opcode)
        {
            var tokens = opcode.ToString().Split(new[] { '_' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return (CpuRegister)Enum.Parse(typeof(CpuRegister), tokens[1]);
        }

        private void AssertRotate(byte expected, CpuRegister target)
        {
            var targetValue = Get8BitRegisterValue(target);

            Assert.AreEqual(expected, targetValue);

            // Make sure all other registers have not changed.
            foreach (var register in target.Other8BitRegisters())
            {
                Assert.AreEqual(Get8BitRegisterInitialValue(register), Get8BitRegisterValue(register));
            }
        }
    }
}