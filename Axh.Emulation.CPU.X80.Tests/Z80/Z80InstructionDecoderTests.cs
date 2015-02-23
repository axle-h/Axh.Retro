namespace Axh.Emulation.CPU.X80.Tests.Z80
{
    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Contracts.OpCodes;
    using Axh.Emulation.CPU.X80.Contracts.Registers;
    using Axh.Emulation.CPU.X80.Contracts.Z80;
    using Axh.Emulation.CPU.X80.Z80;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoderTests
    {
        private IZ80InstructionDecoder decoder;

        private Mock<IZ80Registers> registers;

        private Mock<IMmu> mmu;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            decoder = new Z80InstructionDecoder();

            registers = new Mock<IZ80Registers>();
            mmu = new Mock<IMmu>();
        }

        [Test]
        public void NopIncrentsProgramCounterAndMemoryRefreshRegisters()
        {
            var expression = decoder.DecodeOperation(PrimaryOpCode.NOP);
            Assert.IsNotNull(expression);

            registers.ResetCalls();
            mmu.ResetCalls();

            registers.Setup(x => x.R).Returns(0x5f);
            registers.Setup(x => x.ProgramCounter).Returns(0x1234);

            var action = expression.Compile();
            action(registers.Object, mmu.Object);
            
            registers.VerifySet(x => x.R = It.Is<byte>(y => y == 0x60), Times.Once);
            registers.VerifySet(x => x.ProgramCounter = It.Is<ushort>(y => y == 0x1235), Times.Once);
        }
    }
}
