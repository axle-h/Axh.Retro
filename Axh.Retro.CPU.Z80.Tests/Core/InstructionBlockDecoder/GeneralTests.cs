namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class GeneralTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public GeneralTests() : base(CpuMode.Z80)
        {
        }

        [Test]
        public void NopIncrentsProgramCounterAndMemoryRefreshRegisters()
        {
            this.ResetMocks();

            this.Registers.SetupProperty(x => x.R, (byte)0x5f);
            this.Registers.SetupProperty(x => x.ProgramCounter, (ushort)0x1234);

            this.RunWithHalt(2, 8, PrimaryOpCode.NOP, PrimaryOpCode.NOP);

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

            this.RunWithHalt(0, 0);
            
            this.Cache.Verify(x => x.NextByte(), Times.Once);
            Assert.AreEqual(0x00, this.Registers.Object.R);
            Assert.AreEqual(0x0000, this.Registers.Object.ProgramCounter);
        }
    }
}
