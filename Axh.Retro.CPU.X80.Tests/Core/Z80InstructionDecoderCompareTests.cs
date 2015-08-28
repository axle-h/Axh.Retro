namespace Axh.Retro.CPU.X80.Tests.Core
{
    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoderCompareTests : Z80InstructionDecoderBase
    {
        [Test]
        public void CPI()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xa7;

            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            Run(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPI);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Compare(A, Value), Times.Once);
        }
    }
}
