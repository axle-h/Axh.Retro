namespace Axh.Retro.CPU.X80.Tests.Core.Z80InstructionDecoder
{
    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ExchangeTests : Z80InstructionDecoderTestsBase
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

        [Test]
        public void EX_AF()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(1, 4, PrimaryOpCode.EX_AF);

            this.Registers.Verify(x => x.SwitchToAlternativeAccumulatorAndFlagsRegisters(), Times.Once);
        }
        
        [Test]
        public void EXX()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(1, 4, PrimaryOpCode.EXX);

            this.Registers.Verify(x => x.SwitchToAlternativeGeneralPurposeRegisters(), Times.Once);
        }

        [Test]
        public void EX_mSP_HL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort SP1 = unchecked((ushort)(SP + 1));
            const byte Value0 = 0x12;
            const byte Value1 = 0x34;

            this.Mmu.Setup(x => x.ReadByte(SP)).Returns(Value0);
            this.Mmu.Setup(x => x.ReadByte(SP1)).Returns(Value1);

            Run(5, 19, PrimaryOpCode.EX_mSP_HL);

            this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == Value1), Times.Once);
            this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == Value0), Times.Once);

            Assert.AreEqual(Value1, this.GpRegisters.Object.H);
            Assert.AreEqual(Value0, this.GpRegisters.Object.L);

            // SP unchanged
            this.Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
            Assert.AreEqual(SP, this.Registers.Object.StackPointer);

            this.Mmu.Verify(x => x.ReadByte(SP), Times.Once);
            this.Mmu.Verify(x => x.ReadByte(SP1), Times.Once);

            this.Mmu.Verify(x => x.WriteByte(SP, L), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(SP1, H), Times.Once);
        }

        [Test]
        public void EX_mSP_IX()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort SP1 = unchecked((ushort)(SP + 1));
            const byte Value0 = 0x12;
            const byte Value1 = 0x34;

            this.Mmu.Setup(x => x.ReadByte(SP)).Returns(Value0);
            this.Mmu.Setup(x => x.ReadByte(SP1)).Returns(Value1);

            Run(6, 23, PrimaryOpCode.Prefix_DD, PrimaryOpCode.EX_mSP_HL);

            this.Registers.VerifySet(x => x.IXh = It.Is<byte>(y => y == Value1), Times.Once);
            this.Registers.VerifySet(x => x.IXl = It.Is<byte>(y => y == Value0), Times.Once);

            Assert.AreEqual(Value1, this.Registers.Object.IXh);
            Assert.AreEqual(Value0, this.Registers.Object.IXl);

            // SP unchanged
            this.Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
            Assert.AreEqual(SP, this.Registers.Object.StackPointer);

            this.Mmu.Verify(x => x.ReadByte(SP), Times.Once);
            this.Mmu.Verify(x => x.ReadByte(SP1), Times.Once);

            this.Mmu.Verify(x => x.WriteByte(SP, IXl), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(SP1, IXh), Times.Once);
        }

        [Test]
        public void EX_mSP_IY()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort SP1 = unchecked((ushort)(SP + 1));
            const byte Value0 = 0x12;
            const byte Value1 = 0x34;

            this.Mmu.Setup(x => x.ReadByte(SP)).Returns(Value0);
            this.Mmu.Setup(x => x.ReadByte(SP1)).Returns(Value1);

            Run(6, 23, PrimaryOpCode.Prefix_FD, PrimaryOpCode.EX_mSP_HL);

            this.Registers.VerifySet(x => x.IYh = It.Is<byte>(y => y == Value1), Times.Once);
            this.Registers.VerifySet(x => x.IYl = It.Is<byte>(y => y == Value0), Times.Once);

            Assert.AreEqual(Value1, this.Registers.Object.IYh);
            Assert.AreEqual(Value0, this.Registers.Object.IYl);

            // SP unchanged
            this.Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
            Assert.AreEqual(SP, this.Registers.Object.StackPointer);

            this.Mmu.Verify(x => x.ReadByte(SP), Times.Once);
            this.Mmu.Verify(x => x.ReadByte(SP1), Times.Once);

            this.Mmu.Verify(x => x.WriteByte(SP, IYl), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(SP1, IYh), Times.Once);
        }
    }
}
