namespace Axh.Retro.CPU.X80.Tests.Core
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoder8BitArithmeticTests : Z80InstructionDecoderBase
    {
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

            Run(1, 4, opCode);

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

            Run(2, 7, PrimaryOpCode.ADD_A_n, N);
            
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

            Run(2, 7, PrimaryOpCode.ADD_A_mHL);

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

            Run(5, 19, opCode, PrimaryOpCode.ADD_A_mHL, unchecked((byte)Displacement));

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

            Run(1, 4, opCode);

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

            Run(2, 7, PrimaryOpCode.ADC_A_n, N);

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

            Run(2, 7, PrimaryOpCode.ADC_A_mHL);

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

            Run(5, 19, opCode, PrimaryOpCode.ADC_A_mHL, unchecked((byte)Displacement));

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

            Run(1, 4, opCode);

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

            Run(2, 7, PrimaryOpCode.SUB_A_n, N);

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

            Run(2, 7, PrimaryOpCode.SUB_A_mHL);

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

            Run(5, 19, opCode, PrimaryOpCode.SUB_A_mHL, unchecked((byte)Displacement));

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

            Run(1, 4, opCode);

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

            Run(2, 7, PrimaryOpCode.SBC_A_n, N);

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

            Run(2, 7, PrimaryOpCode.SBC_A_mHL);

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

            Run(5, 19, opCode, PrimaryOpCode.SBC_A_mHL, unchecked((byte)Displacement));

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

            Run(1, 4, opCode);

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

            Run(2, 7, PrimaryOpCode.AND_n, N);

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

            Run(2, 7, PrimaryOpCode.AND_mHL);

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

            Run(5, 19, opCode, PrimaryOpCode.AND_mHL, unchecked((byte)Displacement));

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

            Run(1, 4, opCode);

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

            Run(2, 7, PrimaryOpCode.OR_n, N);

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

            Run(2, 7, PrimaryOpCode.OR_mHL);

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

            Run(5, 19, opCode, PrimaryOpCode.OR_mHL, unchecked((byte)Displacement));

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

            Run(1, 4, opCode);

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

            Run(2, 7, PrimaryOpCode.XOR_n, N);

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

            Run(2, 7, PrimaryOpCode.XOR_mHL);

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

            Run(5, 19, opCode, PrimaryOpCode.XOR_mHL, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.Xor(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

    }
}
