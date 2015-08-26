namespace Axh.Retro.CPU.X80.Tests.Core
{
    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoderExchangeBlockTransferSearchTests : Z80InstructionDecoderBase
    {
        [Test]
        public void EX_DE_HL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(1, 4, PrimaryOpCode.EX_DE_HL);

            this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == HL), Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == DE), Times.Once);
            Assert.AreEqual(DE, this.GpRegisters.Object.HL);
            Assert.AreEqual(HL, this.GpRegisters.Object.DE);
        }
    }
}
