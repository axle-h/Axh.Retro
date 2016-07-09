using System;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Tests.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class ShiftTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public ShiftTests() : base(CpuMode.Z80)
        {
        }

        [TestCase(PrefixCbOpCode.SLA_A)]
        [TestCase(PrefixCbOpCode.SLA_B)]
        [TestCase(PrefixCbOpCode.SLA_C)]
        [TestCase(PrefixCbOpCode.SLA_E)]
        [TestCase(PrefixCbOpCode.SLA_H)]
        [TestCase(PrefixCbOpCode.SLA_L)]
        public void SLA_r(PrefixCbOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x4e;
            Alu.Setup(x => x.ShiftLeft(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.SLA_A:
                    Alu.Verify(x => x.ShiftLeft(A), Times.Once);
                    Assert.AreEqual(Value, AfRegisters.A);
                    break;
                case PrefixCbOpCode.SLA_B:
                    Alu.Verify(x => x.ShiftLeft(B), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.B);
                    break;
                case PrefixCbOpCode.SLA_C:
                    Alu.Verify(x => x.ShiftLeft(C), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.C);
                    break;
                case PrefixCbOpCode.SLA_D:
                    Alu.Verify(x => x.ShiftLeft(D), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.D);
                    break;
                case PrefixCbOpCode.SLA_E:
                    Alu.Verify(x => x.ShiftLeft(E), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.E);
                    break;
                case PrefixCbOpCode.SLA_H:
                    Alu.Verify(x => x.ShiftLeft(H), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.H);
                    break;
                case PrefixCbOpCode.SLA_L:
                    Alu.Verify(x => x.ShiftLeft(L), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrefixCbOpCode.SLS_A)]
        [TestCase(PrefixCbOpCode.SLS_B)]
        [TestCase(PrefixCbOpCode.SLS_C)]
        [TestCase(PrefixCbOpCode.SLS_E)]
        [TestCase(PrefixCbOpCode.SLS_H)]
        [TestCase(PrefixCbOpCode.SLS_L)]
        public void SLS_r(PrefixCbOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x4e;
            Alu.Setup(x => x.ShiftLeftSet(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.SLS_A:
                    Alu.Verify(x => x.ShiftLeftSet(A), Times.Once);
                    Assert.AreEqual(Value, AfRegisters.A);
                    break;
                case PrefixCbOpCode.SLS_B:
                    Alu.Verify(x => x.ShiftLeftSet(B), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.B);
                    break;
                case PrefixCbOpCode.SLS_C:
                    Alu.Verify(x => x.ShiftLeftSet(C), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.C);
                    break;
                case PrefixCbOpCode.SLS_D:
                    Alu.Verify(x => x.ShiftLeftSet(D), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.D);
                    break;
                case PrefixCbOpCode.SLS_E:
                    Alu.Verify(x => x.ShiftLeftSet(E), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.E);
                    break;
                case PrefixCbOpCode.SLS_H:
                    Alu.Verify(x => x.ShiftLeftSet(H), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.H);
                    break;
                case PrefixCbOpCode.SLS_L:
                    Alu.Verify(x => x.ShiftLeftSet(L), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrefixCbOpCode.SRA_A)]
        [TestCase(PrefixCbOpCode.SRA_B)]
        [TestCase(PrefixCbOpCode.SRA_C)]
        [TestCase(PrefixCbOpCode.SRA_E)]
        [TestCase(PrefixCbOpCode.SRA_H)]
        [TestCase(PrefixCbOpCode.SRA_L)]
        public void SRA_r(PrefixCbOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x4e;
            Alu.Setup(x => x.ShiftRight(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.SRA_A:
                    Alu.Verify(x => x.ShiftRight(A), Times.Once);
                    Assert.AreEqual(Value, AfRegisters.A);
                    break;
                case PrefixCbOpCode.SRA_B:
                    Alu.Verify(x => x.ShiftRight(B), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.B);
                    break;
                case PrefixCbOpCode.SRA_C:
                    Alu.Verify(x => x.ShiftRight(C), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.C);
                    break;
                case PrefixCbOpCode.SRA_D:
                    Alu.Verify(x => x.ShiftRight(D), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.D);
                    break;
                case PrefixCbOpCode.SRA_E:
                    Alu.Verify(x => x.ShiftRight(E), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.E);
                    break;
                case PrefixCbOpCode.SRA_H:
                    Alu.Verify(x => x.ShiftRight(H), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.H);
                    break;
                case PrefixCbOpCode.SRA_L:
                    Alu.Verify(x => x.ShiftRight(L), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TestCase(PrefixCbOpCode.SRL_A)]
        [TestCase(PrefixCbOpCode.SRL_B)]
        [TestCase(PrefixCbOpCode.SRL_C)]
        [TestCase(PrefixCbOpCode.SRL_E)]
        [TestCase(PrefixCbOpCode.SRL_H)]
        [TestCase(PrefixCbOpCode.SRL_L)]
        public void SRL_r(PrefixCbOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x4e;
            Alu.Setup(x => x.ShiftRightLogical(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case PrefixCbOpCode.SRL_A:
                    Alu.Verify(x => x.ShiftRightLogical(A), Times.Once);
                    Assert.AreEqual(Value, AfRegisters.A);
                    break;
                case PrefixCbOpCode.SRL_B:
                    Alu.Verify(x => x.ShiftRightLogical(B), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.B);
                    break;
                case PrefixCbOpCode.SRL_C:
                    Alu.Verify(x => x.ShiftRightLogical(C), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.C);
                    break;
                case PrefixCbOpCode.SRL_D:
                    Alu.Verify(x => x.ShiftRightLogical(D), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.D);
                    break;
                case PrefixCbOpCode.SRL_E:
                    Alu.Verify(x => x.ShiftRightLogical(E), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.E);
                    break;
                case PrefixCbOpCode.SRL_H:
                    Alu.Verify(x => x.ShiftRightLogical(H), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.H);
                    break;
                case PrefixCbOpCode.SRL_L:
                    Alu.Verify(x => x.ShiftRightLogical(L), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            Alu.Setup(x => x.ShiftLeft(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), opcode);

            Alu.Verify(x => x.ShiftLeft(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            AssertShift(ValueAtIndex, ParseRegister(opcode));
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SLA_mIXYd(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            Alu.Setup(x => x.ShiftLeft(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), PrefixCbOpCode.SLA_mHL);

            Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            Alu.Verify(x => x.ShiftLeft(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
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
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            Alu.Setup(x => x.ShiftLeftSet(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), opcode);

            Alu.Verify(x => x.ShiftLeftSet(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            AssertShift(ValueAtIndex, ParseRegister(opcode));
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SLS_mIXYd(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            Alu.Setup(x => x.ShiftLeftSet(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), PrefixCbOpCode.SLS_mHL);

            Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            Alu.Verify(x => x.ShiftLeftSet(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
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
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            Alu.Setup(x => x.ShiftRight(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), opcode);

            Alu.Verify(x => x.ShiftRight(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            AssertShift(ValueAtIndex, ParseRegister(opcode));
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SRA_mIXYd(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            Alu.Setup(x => x.ShiftRight(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), PrefixCbOpCode.SRA_mHL);

            Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            Alu.Verify(x => x.ShiftRight(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
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
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x24;
            const int Expected = 0xb4;

            Alu.Setup(x => x.ShiftRightLogical(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(8, 31, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), opcode);

            Alu.Verify(x => x.ShiftRightLogical(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

            AssertShift(ValueAtIndex, ParseRegister(opcode));
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SRL_mIXYd(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const int ValueAtIndex = 0x85;
            const int Expected = 0x60;

            Alu.Setup(x => x.ShiftRightLogical(ValueAtIndex)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), PrefixCbOpCode.SRL_mHL);

            Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            Alu.Verify(x => x.ShiftRightLogical(ValueAtIndex), Times.Once);
            Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

        [Test]
        public void SLA_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            Alu.Setup(x => x.ShiftLeft(ValueAtHL)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.SLA_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.ShiftLeft(ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [Test]
        public void SLS_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            Alu.Setup(x => x.ShiftLeftSet(ValueAtHL)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.SLS_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.ShiftLeftSet(ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [Test]
        public void SRA_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            Alu.Setup(x => x.ShiftRight(ValueAtHL)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.SRA_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.ShiftRight(ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        [Test]
        public void SRL_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtHL = 0x38;
            const byte Expected = 0xf2;
            Alu.Setup(x => x.ShiftRightLogical(ValueAtHL)).Returns(Expected);
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_CB, PrefixCbOpCode.SRL_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.ShiftRightLogical(ValueAtHL), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }

        private static CpuRegister ParseRegister(PrefixCbOpCode opcode)
        {
            var tokens = opcode.ToString().Split(new[] { '_' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return (CpuRegister)Enum.Parse(typeof(CpuRegister), tokens[1]);
        }

        private void AssertShift(byte expected, CpuRegister target)
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