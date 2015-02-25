namespace Axh.Emulation.CPU.X80.Tests.Core
{
    using Axh.Emulation.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoderGeneralTests : Z80InstructionDecoderBase
    {
        [Test]
        public void HaltIncrentsProgramCounterAndMemoryRefreshRegisters()
        {
            this.ResetMocks();

            this.cache.SetupSequence(x => x.NextByte()).Returns((byte)PrimaryOpCode.NOP).Returns((byte)PrimaryOpCode.NOP).Returns((byte)PrimaryOpCode.HALT);
            var block = this.decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);
            Assert.AreEqual(3, block.MachineCycles);
            Assert.AreEqual(12, block.ThrottlingStates);

            this.registers.SetupProperty(x => x.R, (byte)0x5f);
            this.registers.SetupProperty(x => x.ProgramCounter, (ushort)0x1234);

            block.Action(this.registers.Object, this.mmu.Object);

            this.cache.Verify(x => x.NextByte(), Times.Exactly(3));
            Assert.AreEqual(0x62, this.registers.Object.R);
            Assert.AreEqual(0x1237, this.registers.Object.ProgramCounter);
        }

        [Test]
        public void NopIncrentsProgramCounterAndMemoryRefreshRegistersWithCorrectOverflow()
        {
            this.ResetMocks();

            this.cache.Setup(x => x.NextByte()).Returns((byte)PrimaryOpCode.HALT);
            var block = this.decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);
            Assert.AreEqual(1, block.MachineCycles);
            Assert.AreEqual(4, block.ThrottlingStates);

            this.registers.SetupProperty(x => x.R, (byte)0x7f);
            this.registers.SetupProperty(x => x.ProgramCounter, (ushort)0xffff);

            block.Action(this.registers.Object, this.mmu.Object);

            this.cache.Verify(x => x.NextByte(), Times.Once);
            Assert.AreEqual(0x00, this.registers.Object.R);
            Assert.AreEqual(0x0000, this.registers.Object.ProgramCounter);
        }
    }
}
