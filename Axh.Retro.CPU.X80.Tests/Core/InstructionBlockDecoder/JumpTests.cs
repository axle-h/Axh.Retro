namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class JumpTests : InstructionBlockDecoderTestsBase
    {
        [Test]
        public void JP()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x1be7;

            Run(3, 10, PrimaryOpCode.JP, Value);

            this.Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
        }
    }
}
