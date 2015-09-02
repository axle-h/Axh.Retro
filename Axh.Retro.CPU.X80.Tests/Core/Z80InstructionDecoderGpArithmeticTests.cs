namespace Axh.Retro.CPU.X80.Tests.Core
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoderGpArithmeticTests : Z80InstructionDecoderBase
    {
        [Test]
        public void DAA()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            const byte Expected = 0xac;
            this.Alu.Setup(x => x.DecimalAdjust(A)).Returns(Expected);

            Run(1, 4, PrimaryOpCode.DAA);

            this.Alu.Verify(x => x.DecimalAdjust(A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }
    }
}
