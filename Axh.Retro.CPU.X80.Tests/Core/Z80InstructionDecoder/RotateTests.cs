namespace Axh.Retro.CPU.X80.Tests.Core.Z80InstructionDecoder
{
    using System;

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

            Run(1, 4, PrimaryOpCode.RLCA);

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

            Run(1, 4, PrimaryOpCode.RLA);

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

            Run(1, 4, PrimaryOpCode.RRCA);

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

            Run(1, 4, PrimaryOpCode.RRA);

            this.Alu.Verify(x => x.RotateRight(A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }
    }
}
