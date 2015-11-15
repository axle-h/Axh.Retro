namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Arithmetic8BitTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public Arithmetic8BitTests() : base(CpuMode.Z80)
        {
        }

        [TestCase(PrimaryOpCode.ADD_A_A)]
        [TestCase(PrimaryOpCode.ADD_A_B)]
        [TestCase(PrimaryOpCode.ADD_A_C)]
        [TestCase(PrimaryOpCode.ADD_A_D)]
        [TestCase(PrimaryOpCode.ADD_A_E)]
        [TestCase(PrimaryOpCode.ADD_A_H)]
        [TestCase(PrimaryOpCode.ADD_A_L)]
        public void ADD_A_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x18;
            this.Alu.Setup(x => x.Add(A, It.IsAny<byte>())).Returns(Value);

            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.ADD_A_A:
                    this.Alu.Verify(x => x.Add(A, A), Times.Once);
                    break;
                case PrimaryOpCode.ADD_A_B:
                    this.Alu.Verify(x => x.Add(A, B), Times.Once);
                    break;
                case PrimaryOpCode.ADD_A_C:
                    this.Alu.Verify(x => x.Add(A, C), Times.Once);
                    break;
                case PrimaryOpCode.ADD_A_D:
                    this.Alu.Verify(x => x.Add(A, D), Times.Once);
                    break;
                case PrimaryOpCode.ADD_A_E:
                    this.Alu.Verify(x => x.Add(A, E), Times.Once);
                    break;
                case PrimaryOpCode.ADD_A_H:
                    this.Alu.Verify(x => x.Add(A, H), Times.Once);
                    break;
                case PrimaryOpCode.ADD_A_L:
                    this.Alu.Verify(x => x.Add(A, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void ADD_A_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte N = 0xb1;
            const byte Expected = unchecked((byte)(N + A));
            this.Alu.Setup(x => x.Add(A, N)).Returns(Expected);

            RunWithHalt(2, 7, PrimaryOpCode.ADD_A_n, N);
            
            this.Alu.Verify(x => x.Add(A, N), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void ADD_A_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0xb6;
            const byte Expected = unchecked((byte)(ValueAtHL + A));
            this.Alu.Setup(x => x.Add(A, ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(2, 7, PrimaryOpCode.ADD_A_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Add(A, ValueAtHL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void ADD_A_mIXYd(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = opCode == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0xb6;
            const byte Expected = unchecked((byte)(ValueAtIndex + A));
            this.Alu.Setup(x => x.Add(A, ValueAtIndex)).Returns(Expected);
            
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(5, 19, opCode, PrimaryOpCode.ADD_A_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.Add(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.ADC_A_A)]
        [TestCase(PrimaryOpCode.ADC_A_B)]
        [TestCase(PrimaryOpCode.ADC_A_C)]
        [TestCase(PrimaryOpCode.ADC_A_D)]
        [TestCase(PrimaryOpCode.ADC_A_E)]
        [TestCase(PrimaryOpCode.ADC_A_H)]
        [TestCase(PrimaryOpCode.ADC_A_L)]
        public void ADC_A_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x18;
            this.Alu.Setup(x => x.AddWithCarry(A, It.IsAny<byte>())).Returns(Value);

            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.ADC_A_A:
                    this.Alu.Verify(x => x.AddWithCarry(A, A), Times.Once);
                    break;
                case PrimaryOpCode.ADC_A_B:
                    this.Alu.Verify(x => x.AddWithCarry(A, B), Times.Once);
                    break;
                case PrimaryOpCode.ADC_A_C:
                    this.Alu.Verify(x => x.AddWithCarry(A, C), Times.Once);
                    break;
                case PrimaryOpCode.ADC_A_D:
                    this.Alu.Verify(x => x.AddWithCarry(A, D), Times.Once);
                    break;
                case PrimaryOpCode.ADC_A_E:
                    this.Alu.Verify(x => x.AddWithCarry(A, E), Times.Once);
                    break;
                case PrimaryOpCode.ADC_A_H:
                    this.Alu.Verify(x => x.AddWithCarry(A, H), Times.Once);
                    break;
                case PrimaryOpCode.ADC_A_L:
                    this.Alu.Verify(x => x.AddWithCarry(A, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void ADC_A_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte N = 0xb4;
            const byte Expected = unchecked((byte)(N + A));
            this.Alu.Setup(x => x.AddWithCarry(A, N)).Returns(Expected);

            RunWithHalt(2, 7, PrimaryOpCode.ADC_A_n, N);

            this.Alu.Verify(x => x.AddWithCarry(A, N), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void ADC_A_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0xb6;
            const byte Expected = unchecked((byte)(ValueAtHL + A));
            this.Alu.Setup(x => x.AddWithCarry(A, ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(2, 7, PrimaryOpCode.ADC_A_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.AddWithCarry(A, ValueAtHL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void ADC_A_mIXYd(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = opCode == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0xb6;
            const byte Expected = unchecked((byte)(ValueAtIndex + A));
            this.Alu.Setup(x => x.AddWithCarry(A, ValueAtIndex)).Returns(Expected);

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(5, 19, opCode, PrimaryOpCode.ADC_A_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.AddWithCarry(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.SUB_A_A)]
        [TestCase(PrimaryOpCode.SUB_A_B)]
        [TestCase(PrimaryOpCode.SUB_A_C)]
        [TestCase(PrimaryOpCode.SUB_A_D)]
        [TestCase(PrimaryOpCode.SUB_A_E)]
        [TestCase(PrimaryOpCode.SUB_A_H)]
        [TestCase(PrimaryOpCode.SUB_A_L)]
        public void SUB_A_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x9c;
            this.Alu.Setup(x => x.Subtract(A, It.IsAny<byte>())).Returns(Value);

            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.SUB_A_A:
                    this.Alu.Verify(x => x.Subtract(A, A), Times.Once);
                    break;
                case PrimaryOpCode.SUB_A_B:
                    this.Alu.Verify(x => x.Subtract(A, B), Times.Once);
                    break;
                case PrimaryOpCode.SUB_A_C:
                    this.Alu.Verify(x => x.Subtract(A, C), Times.Once);
                    break;
                case PrimaryOpCode.SUB_A_D:
                    this.Alu.Verify(x => x.Subtract(A, D), Times.Once);
                    break;
                case PrimaryOpCode.SUB_A_E:
                    this.Alu.Verify(x => x.Subtract(A, E), Times.Once);
                    break;
                case PrimaryOpCode.SUB_A_H:
                    this.Alu.Verify(x => x.Subtract(A, H), Times.Once);
                    break;
                case PrimaryOpCode.SUB_A_L:
                    this.Alu.Verify(x => x.Subtract(A, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void SUB_A_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte N = 0xc1;
            const byte Expected = unchecked((byte)(A - N));
            this.Alu.Setup(x => x.Subtract(A, N)).Returns(Expected);

            RunWithHalt(2, 7, PrimaryOpCode.SUB_A_n, N);

            this.Alu.Verify(x => x.Subtract(A, N), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void SUB_A_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x3a;
            const byte Expected = unchecked((byte)(A - ValueAtHL));
            this.Alu.Setup(x => x.Subtract(A, ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(2, 7, PrimaryOpCode.SUB_A_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Subtract(A, ValueAtHL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SUB_A_mIXYd(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = opCode == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0xb6;
            const byte Expected = unchecked((byte)(A - ValueAtIndex));
            this.Alu.Setup(x => x.Subtract(A, ValueAtIndex)).Returns(Expected);

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(5, 19, opCode, PrimaryOpCode.SUB_A_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.Subtract(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }


        [TestCase(PrimaryOpCode.SBC_A_A)]
        [TestCase(PrimaryOpCode.SBC_A_B)]
        [TestCase(PrimaryOpCode.SBC_A_C)]
        [TestCase(PrimaryOpCode.SBC_A_D)]
        [TestCase(PrimaryOpCode.SBC_A_E)]
        [TestCase(PrimaryOpCode.SBC_A_H)]
        [TestCase(PrimaryOpCode.SBC_A_L)]
        public void SBC_A_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x9c;
            this.Alu.Setup(x => x.SubtractWithCarry(A, It.IsAny<byte>())).Returns(Value);

            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.SBC_A_A:
                    this.Alu.Verify(x => x.SubtractWithCarry(A, A), Times.Once);
                    break;
                case PrimaryOpCode.SBC_A_B:
                    this.Alu.Verify(x => x.SubtractWithCarry(A, B), Times.Once);
                    break;
                case PrimaryOpCode.SBC_A_C:
                    this.Alu.Verify(x => x.SubtractWithCarry(A, C), Times.Once);
                    break;
                case PrimaryOpCode.SBC_A_D:
                    this.Alu.Verify(x => x.SubtractWithCarry(A, D), Times.Once);
                    break;
                case PrimaryOpCode.SBC_A_E:
                    this.Alu.Verify(x => x.SubtractWithCarry(A, E), Times.Once);
                    break;
                case PrimaryOpCode.SBC_A_H:
                    this.Alu.Verify(x => x.SubtractWithCarry(A, H), Times.Once);
                    break;
                case PrimaryOpCode.SBC_A_L:
                    this.Alu.Verify(x => x.SubtractWithCarry(A, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void SBC_A_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte N = 0xc1;
            const byte Expected = unchecked((byte)(A - N));
            this.Alu.Setup(x => x.SubtractWithCarry(A, N)).Returns(Expected);

            RunWithHalt(2, 7, PrimaryOpCode.SBC_A_n, N);

            this.Alu.Verify(x => x.SubtractWithCarry(A, N), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void SBC_A_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x3a;
            const byte Expected = unchecked((byte)(A - ValueAtHL));
            this.Alu.Setup(x => x.SubtractWithCarry(A, ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(2, 7, PrimaryOpCode.SBC_A_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.SubtractWithCarry(A, ValueAtHL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SBC_A_mIXYd(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = opCode == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0xb6;
            const byte Expected = unchecked((byte)(A - ValueAtIndex));
            this.Alu.Setup(x => x.SubtractWithCarry(A, ValueAtIndex)).Returns(Expected);

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(5, 19, opCode, PrimaryOpCode.SBC_A_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.SubtractWithCarry(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }


        [TestCase(PrimaryOpCode.AND_A)]
        [TestCase(PrimaryOpCode.AND_B)]
        [TestCase(PrimaryOpCode.AND_C)]
        [TestCase(PrimaryOpCode.AND_D)]
        [TestCase(PrimaryOpCode.AND_E)]
        [TestCase(PrimaryOpCode.AND_H)]
        [TestCase(PrimaryOpCode.AND_L)]
        public void AND_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x9c;
            this.Alu.Setup(x => x.And(A, It.IsAny<byte>())).Returns(Value);

            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.AND_A:
                    this.Alu.Verify(x => x.And(A, A), Times.Once);
                    break;
                case PrimaryOpCode.AND_B:
                    this.Alu.Verify(x => x.And(A, B), Times.Once);
                    break;
                case PrimaryOpCode.AND_C:
                    this.Alu.Verify(x => x.And(A, C), Times.Once);
                    break;
                case PrimaryOpCode.AND_D:
                    this.Alu.Verify(x => x.And(A, D), Times.Once);
                    break;
                case PrimaryOpCode.AND_E:
                    this.Alu.Verify(x => x.And(A, E), Times.Once);
                    break;
                case PrimaryOpCode.AND_H:
                    this.Alu.Verify(x => x.And(A, H), Times.Once);
                    break;
                case PrimaryOpCode.AND_L:
                    this.Alu.Verify(x => x.And(A, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void AND_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte N = 0xc1;
            const byte Expected = unchecked((byte)(A - N));
            this.Alu.Setup(x => x.And(A, N)).Returns(Expected);

            RunWithHalt(2, 7, PrimaryOpCode.AND_n, N);

            this.Alu.Verify(x => x.And(A, N), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void AND_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x3a;
            const byte Expected = unchecked((byte)(A - ValueAtHL));
            this.Alu.Setup(x => x.And(A, ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(2, 7, PrimaryOpCode.AND_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.And(A, ValueAtHL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void AND_mIXYd(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = opCode == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0xb6;
            const byte Expected = unchecked((byte)(A - ValueAtIndex));
            this.Alu.Setup(x => x.And(A, ValueAtIndex)).Returns(Expected);

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(5, 19, opCode, PrimaryOpCode.AND_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.And(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }


        [TestCase(PrimaryOpCode.OR_A)]
        [TestCase(PrimaryOpCode.OR_B)]
        [TestCase(PrimaryOpCode.OR_C)]
        [TestCase(PrimaryOpCode.OR_D)]
        [TestCase(PrimaryOpCode.OR_E)]
        [TestCase(PrimaryOpCode.OR_H)]
        [TestCase(PrimaryOpCode.OR_L)]
        public void OR_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x9c;
            this.Alu.Setup(x => x.Or(A, It.IsAny<byte>())).Returns(Value);

            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.OR_A:
                    this.Alu.Verify(x => x.Or(A, A), Times.Once);
                    break;
                case PrimaryOpCode.OR_B:
                    this.Alu.Verify(x => x.Or(A, B), Times.Once);
                    break;
                case PrimaryOpCode.OR_C:
                    this.Alu.Verify(x => x.Or(A, C), Times.Once);
                    break;
                case PrimaryOpCode.OR_D:
                    this.Alu.Verify(x => x.Or(A, D), Times.Once);
                    break;
                case PrimaryOpCode.OR_E:
                    this.Alu.Verify(x => x.Or(A, E), Times.Once);
                    break;
                case PrimaryOpCode.OR_H:
                    this.Alu.Verify(x => x.Or(A, H), Times.Once);
                    break;
                case PrimaryOpCode.OR_L:
                    this.Alu.Verify(x => x.Or(A, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void OR_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte N = 0xc1;
            const byte Expected = unchecked((byte)(A - N));
            this.Alu.Setup(x => x.Or(A, N)).Returns(Expected);

            RunWithHalt(2, 7, PrimaryOpCode.OR_n, N);

            this.Alu.Verify(x => x.Or(A, N), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void OR_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x3a;
            const byte Expected = unchecked((byte)(A - ValueAtHL));
            this.Alu.Setup(x => x.Or(A, ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(2, 7, PrimaryOpCode.OR_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Or(A, ValueAtHL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void OR_mIXYd(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = opCode == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0xb6;
            const byte Expected = unchecked((byte)(A - ValueAtIndex));
            this.Alu.Setup(x => x.Or(A, ValueAtIndex)).Returns(Expected);

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(5, 19, opCode, PrimaryOpCode.OR_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.Or(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }


        [TestCase(PrimaryOpCode.XOR_A)]
        [TestCase(PrimaryOpCode.XOR_B)]
        [TestCase(PrimaryOpCode.XOR_C)]
        [TestCase(PrimaryOpCode.XOR_D)]
        [TestCase(PrimaryOpCode.XOR_E)]
        [TestCase(PrimaryOpCode.XOR_H)]
        [TestCase(PrimaryOpCode.XOR_L)]
        public void XOR_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x9c;
            this.Alu.Setup(x => x.Xor(A, It.IsAny<byte>())).Returns(Value);

            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.XOR_A:
                    this.Alu.Verify(x => x.Xor(A, A), Times.Once);
                    break;
                case PrimaryOpCode.XOR_B:
                    this.Alu.Verify(x => x.Xor(A, B), Times.Once);
                    break;
                case PrimaryOpCode.XOR_C:
                    this.Alu.Verify(x => x.Xor(A, C), Times.Once);
                    break;
                case PrimaryOpCode.XOR_D:
                    this.Alu.Verify(x => x.Xor(A, D), Times.Once);
                    break;
                case PrimaryOpCode.XOR_E:
                    this.Alu.Verify(x => x.Xor(A, E), Times.Once);
                    break;
                case PrimaryOpCode.XOR_H:
                    this.Alu.Verify(x => x.Xor(A, H), Times.Once);
                    break;
                case PrimaryOpCode.XOR_L:
                    this.Alu.Verify(x => x.Xor(A, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void XOR_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte N = 0xc1;
            const byte Expected = unchecked((byte)(A - N));
            this.Alu.Setup(x => x.Xor(A, N)).Returns(Expected);

            RunWithHalt(2, 7, PrimaryOpCode.XOR_n, N);

            this.Alu.Verify(x => x.Xor(A, N), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void XOR_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x3a;
            const byte Expected = unchecked((byte)(A - ValueAtHL));
            this.Alu.Setup(x => x.Xor(A, ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(2, 7, PrimaryOpCode.XOR_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Xor(A, ValueAtHL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void XOR_mIXYd(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = opCode == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0xb6;
            const byte Expected = unchecked((byte)(A - ValueAtIndex));
            this.Alu.Setup(x => x.Xor(A, ValueAtIndex)).Returns(Expected);

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(5, 19, opCode, PrimaryOpCode.XOR_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.Xor(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [TestCase(PrimaryOpCode.CP_A)]
        [TestCase(PrimaryOpCode.CP_B)]
        [TestCase(PrimaryOpCode.CP_C)]
        [TestCase(PrimaryOpCode.CP_D)]
        [TestCase(PrimaryOpCode.CP_E)]
        [TestCase(PrimaryOpCode.CP_H)]
        [TestCase(PrimaryOpCode.CP_L)]
        public void CP_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.CP_A:
                    this.Alu.Verify(x => x.Compare(A, A), Times.Once);
                    break;
                case PrimaryOpCode.CP_B:
                    this.Alu.Verify(x => x.Compare(A, B), Times.Once);
                    break;
                case PrimaryOpCode.CP_C:
                    this.Alu.Verify(x => x.Compare(A, C), Times.Once);
                    break;
                case PrimaryOpCode.CP_D:
                    this.Alu.Verify(x => x.Compare(A, D), Times.Once);
                    break;
                case PrimaryOpCode.CP_E:
                    this.Alu.Verify(x => x.Compare(A, E), Times.Once);
                    break;
                case PrimaryOpCode.CP_H:
                    this.Alu.Verify(x => x.Compare(A, H), Times.Once);
                    break;
                case PrimaryOpCode.CP_L:
                    this.Alu.Verify(x => x.Compare(A, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            this.AfRegisters.VerifySet(x => x.A = It.IsAny<byte>(), Times.Never);
        }

        [Test]
        public void CP_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte N = 0xc1;

            RunWithHalt(2, 7, PrimaryOpCode.CP_n, N);

            this.Alu.Verify(x => x.Compare(A, N), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = It.IsAny<byte>(), Times.Never);
        }

        [Test]
        public void CP_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x3a;
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(2, 7, PrimaryOpCode.CP_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Compare(A, ValueAtHL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = It.IsAny<byte>(), Times.Never);
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void CP_mIXYd(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = opCode == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0xb6;

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(5, 19, opCode, PrimaryOpCode.CP_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.Compare(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = It.IsAny<byte>(), Times.Never);
        }

        [TestCase(PrimaryOpCode.INC_A)]
        [TestCase(PrimaryOpCode.INC_B)]
        [TestCase(PrimaryOpCode.INC_C)]
        [TestCase(PrimaryOpCode.INC_D)]
        [TestCase(PrimaryOpCode.INC_E)]
        [TestCase(PrimaryOpCode.INC_H)]
        [TestCase(PrimaryOpCode.INC_L)]
        public void INC_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Expected = 0xab;
            this.Alu.Setup(x => x.Increment(It.IsAny<byte>())).Returns(Expected);

            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.INC_A:
                    this.Alu.Verify(x => x.Increment(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
                    break;
                case PrimaryOpCode.INC_B:
                    this.Alu.Verify(x => x.Increment(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Expected, Times.Once);
                    break;
                case PrimaryOpCode.INC_C:
                    this.Alu.Verify(x => x.Increment(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Expected, Times.Once);
                    break;
                case PrimaryOpCode.INC_D:
                    this.Alu.Verify(x => x.Increment(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Expected, Times.Once);
                    break;
                case PrimaryOpCode.INC_E:
                    this.Alu.Verify(x => x.Increment(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Expected, Times.Once);
                    break;
                case PrimaryOpCode.INC_H:
                    this.Alu.Verify(x => x.Increment(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Expected, Times.Once);
                    break;
                case PrimaryOpCode.INC_L:
                    this.Alu.Verify(x => x.Increment(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Expected, Times.Once);
                    break;
            }
        }

        [Test]
        public void INC_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x3a;
            const byte Expected = unchecked(ValueAtHL + 1);
            this.Alu.Setup(x => x.Increment(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(3, 11, PrimaryOpCode.INC_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Increment(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }


        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void INC_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0x3a;
            const byte Expected = unchecked(ValueAtIndex + 1);
            this.Alu.Setup(x => x.Increment(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.INC_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.Increment(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }

        [TestCase(PrimaryOpCode.DEC_A)]
        [TestCase(PrimaryOpCode.DEC_B)]
        [TestCase(PrimaryOpCode.DEC_C)]
        [TestCase(PrimaryOpCode.DEC_D)]
        [TestCase(PrimaryOpCode.DEC_E)]
        [TestCase(PrimaryOpCode.DEC_H)]
        [TestCase(PrimaryOpCode.DEC_L)]
        public void DEC_r(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Expected = 0xab;
            this.Alu.Setup(x => x.Decrement(It.IsAny<byte>())).Returns(Expected);

            RunWithHalt(1, 4, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.DEC_A:
                    this.Alu.Verify(x => x.Decrement(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
                    break;
                case PrimaryOpCode.DEC_B:
                    this.Alu.Verify(x => x.Decrement(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Expected, Times.Once);
                    break;
                case PrimaryOpCode.DEC_C:
                    this.Alu.Verify(x => x.Decrement(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Expected, Times.Once);
                    break;
                case PrimaryOpCode.DEC_D:
                    this.Alu.Verify(x => x.Decrement(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Expected, Times.Once);
                    break;
                case PrimaryOpCode.DEC_E:
                    this.Alu.Verify(x => x.Decrement(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Expected, Times.Once);
                    break;
                case PrimaryOpCode.DEC_H:
                    this.Alu.Verify(x => x.Decrement(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Expected, Times.Once);
                    break;
                case PrimaryOpCode.DEC_L:
                    this.Alu.Verify(x => x.Decrement(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Expected, Times.Once);
                    break;
            }
        }

        [Test]
        public void DEC_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtHL = 0x3a;
            const byte Expected = unchecked(ValueAtHL - 1);
            this.Alu.Setup(x => x.Decrement(ValueAtHL)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            RunWithHalt(3, 11, PrimaryOpCode.DEC_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Decrement(ValueAtHL), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
        }


        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void DEC_mIXYd(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -15;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));

            const byte ValueAtIndex = 0x3a;
            const byte Expected = unchecked(ValueAtIndex - 1);
            this.Alu.Setup(x => x.Decrement(ValueAtIndex)).Returns(Expected);
            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            RunWithHalt(6, 23, prefix, PrimaryOpCode.DEC_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.Decrement(ValueAtIndex), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);
        }
    }
}
