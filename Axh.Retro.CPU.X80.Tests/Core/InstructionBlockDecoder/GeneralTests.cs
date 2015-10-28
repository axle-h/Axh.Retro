namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class GeneralTests : InstructionBlockDecoderTestsBase
    {
        [Test]
        public void NopIncrentsProgramCounterAndMemoryRefreshRegisters()
        {
            this.ResetMocks();

            this.Registers.SetupProperty(x => x.R, (byte)0x5f);
            this.Registers.SetupProperty(x => x.ProgramCounter, (ushort)0x1234);

            this.RunWithNOP(2, 8, PrimaryOpCode.NOP, PrimaryOpCode.NOP);

            this.Cache.Verify(x => x.NextByte(), Times.Exactly(3));
            Assert.AreEqual(0x62, this.Registers.Object.R);
            Assert.AreEqual(0x1237, this.Registers.Object.ProgramCounter);
        }

        [Test]
        public void HaltIncrentsProgramCounterAndMemoryRefreshRegistersWithCorrectOverflow()
        {
            this.ResetMocks();

            this.Registers.SetupProperty(x => x.R, (byte)0x7f);
            this.Registers.SetupProperty(x => x.ProgramCounter, (ushort)0xffff);

            this.RunWithNOP(0, 0);
            
            this.Cache.Verify(x => x.NextByte(), Times.Once);
            Assert.AreEqual(0x00, this.Registers.Object.R);
            Assert.AreEqual(0x0000, this.Registers.Object.ProgramCounter);
        }
    }
}
