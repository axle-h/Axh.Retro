using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class GeneralTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public GeneralTests() : base(CpuMode.Z80)
        {
        }

        [Test]
        public void HaltIncrentsProgramCounterAndMemoryRefreshRegistersWithCorrectOverflow()
        {
            ResetMocks();

            Registers.SetupProperty(x => x.R, (byte) 0x7f);
            Registers.SetupProperty(x => x.ProgramCounter, (ushort) 0xffff);

            RunWithHalt(0, 0);

            Cache.Verify(x => x.NextByte(), Times.Once);
            Assert.AreEqual(0x00, Registers.Object.R);
            Assert.AreEqual(0x0000, Registers.Object.ProgramCounter);
        }

        [Test]
        public void NopIncrentsProgramCounterAndMemoryRefreshRegisters()
        {
            ResetMocks();

            Registers.SetupProperty(x => x.R, (byte) 0x5f);
            Registers.SetupProperty(x => x.ProgramCounter, (ushort) 0x1234);

            RunWithHalt(2, 8, PrimaryOpCode.NOP, PrimaryOpCode.NOP);

            Cache.Verify(x => x.NextByte(), Times.Exactly(3));
            Assert.AreEqual(0x62, Registers.Object.R);
            Assert.AreEqual(0x1237, Registers.Object.ProgramCounter);
        }
    }
}