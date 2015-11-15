namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ExchangeTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public ExchangeTests() : base(CpuMode.Z80)
        {
        }

        [Test]
        public void EX_DE_HL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(1, 4, PrimaryOpCode.EX_DE_HL);

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

            RunWithHalt(1, 4, PrimaryOpCode.EX_AF);

            this.Registers.Verify(x => x.SwitchToAlternativeAccumulatorAndFlagsRegisters(), Times.Once);
        }
        
        [Test]
        public void EXX()
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(1, 4, PrimaryOpCode.EXX);

            this.Registers.Verify(x => x.SwitchToAlternativeGeneralPurposeRegisters(), Times.Once);
        }

        [Test]
        public void EX_mSP_HL()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            const ushort Value = 0x1234;

            this.Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            RunWithHalt(5, 19, PrimaryOpCode.EX_mSP_HL);

            this.GpRegisters.VerifySet(x => x.HL = Value, Times.Once);

            Assert.AreEqual(Value, this.GpRegisters.Object.HL);

            // SP unchanged
            this.Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
            Assert.AreEqual(SP, this.Registers.Object.StackPointer);

            this.Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            this.Mmu.Verify(x => x.WriteWord(SP, HL), Times.Once);
        }

        [Test]
        public void EX_mSP_IX()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x1234;

            this.Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            RunWithHalt(6, 23, PrimaryOpCode.Prefix_DD, PrimaryOpCode.EX_mSP_HL);

            this.Registers.VerifySet(x => x.IX = Value, Times.Once);
            Assert.AreEqual(Value, this.Registers.Object.IX);

            // SP unchanged
            this.Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
            Assert.AreEqual(SP, this.Registers.Object.StackPointer);

            this.Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            this.Mmu.Verify(x => x.WriteWord(SP, IX), Times.Once);
        }

        [Test]
        public void EX_mSP_IY()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x1234;

            this.Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            RunWithHalt(6, 23, PrimaryOpCode.Prefix_FD, PrimaryOpCode.EX_mSP_HL);

            this.Registers.VerifySet(x => x.IY = Value, Times.Once);
            Assert.AreEqual(Value, this.Registers.Object.IY);

            // SP unchanged
            this.Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
            Assert.AreEqual(SP, this.Registers.Object.StackPointer);

            this.Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            this.Mmu.Verify(x => x.WriteWord(SP, IY), Times.Once);
        }
    }
}
