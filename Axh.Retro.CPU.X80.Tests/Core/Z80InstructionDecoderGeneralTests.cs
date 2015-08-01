using Axh.Retro.CPU.X80.Contracts.OpCodes;

namespace Axh.Retro.CPU.X80.Tests.Core
{
    using Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoderGeneralTests : Z80InstructionDecoderBase
    {
        [Test]
        public void HaltIncrentsProgramCounterAndMemoryRefreshRegisters()
        {
            this.ResetMocks();

            this.SetCacheForSingleBytes(PrimaryOpCode.NOP, PrimaryOpCode.NOP, PrimaryOpCode.HALT);

            var block = this.Decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);
            Assert.AreEqual(3, block.MachineCycles);
            Assert.AreEqual(12, block.ThrottlingStates);

            this.Registers.SetupProperty(x => x.R, (byte)0x5f);
            this.Registers.SetupProperty(x => x.ProgramCounter, (ushort)0x1234);

            block.Action(this.Registers.Object, this.Mmu.Object);

            this.Cache.Verify(x => x.NextByte(), Times.Exactly(3));
            Assert.AreEqual(0x62, this.Registers.Object.R);
            Assert.AreEqual(0x1237, this.Registers.Object.ProgramCounter);
        }

        [Test]
        public void NopIncrentsProgramCounterAndMemoryRefreshRegistersWithCorrectOverflow()
        {
            this.ResetMocks();
            this.SetCacheForSingleBytes(PrimaryOpCode.HALT);

            var block = this.Decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);
            Assert.AreEqual(1, block.MachineCycles);
            Assert.AreEqual(4, block.ThrottlingStates);

            this.Registers.SetupProperty(x => x.R, (byte)0x7f);
            this.Registers.SetupProperty(x => x.ProgramCounter, (ushort)0xffff);

            block.Action(this.Registers.Object, this.Mmu.Object);

            this.Cache.Verify(x => x.NextByte(), Times.Once);
            Assert.AreEqual(0x00, this.Registers.Object.R);
            Assert.AreEqual(0x0000, this.Registers.Object.ProgramCounter);
        }
    }
}
