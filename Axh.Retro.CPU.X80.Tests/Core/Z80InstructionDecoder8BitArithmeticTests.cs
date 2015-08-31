﻿namespace Axh.Retro.CPU.X80.Tests.Core
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

            Run(5, 19, opCode, PrefixDdFdOpCode.ADD_A_mIXYd, unchecked((byte)Displacement));

            this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
            this.Alu.Verify(x => x.Add(A, ValueAtIndex), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }
    }
}