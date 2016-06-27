using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class ExchangeTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public ExchangeTests() : base(CpuMode.Z80)
        {
        }

        [Test]
        public void EX_AF()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 4, PrimaryOpCode.EX_AF);

            Registers.Verify(x => x.SwitchToAlternativeAccumulatorAndFlagsRegisters(), Times.Once);
        }

        [Test]
        public void EX_DE_HL()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 4, PrimaryOpCode.EX_DE_HL);

            GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == HL), Times.Once);
            GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == DE), Times.Once);
            Assert.AreEqual(DE, GpRegisters.Object.HL);
            Assert.AreEqual(HL, GpRegisters.Object.DE);
        }

        [Test]
        public void EX_mSP_HL()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x1234;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            RunWithHalt(5, 19, PrimaryOpCode.EX_mSP_HL);

            GpRegisters.VerifySet(x => x.HL = Value, Times.Once);

            Assert.AreEqual(Value, GpRegisters.Object.HL);

            // SP unchanged
            Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
            Assert.AreEqual(SP, Registers.Object.StackPointer);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            Mmu.Verify(x => x.WriteWord(SP, HL), Times.Once);
        }

        [Test]
        public void EX_mSP_IX()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x1234;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            RunWithHalt(6, 23, PrimaryOpCode.Prefix_DD, PrimaryOpCode.EX_mSP_HL);

            Registers.VerifySet(x => x.IX = Value, Times.Once);
            Assert.AreEqual(Value, Registers.Object.IX);

            // SP unchanged
            Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
            Assert.AreEqual(SP, Registers.Object.StackPointer);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            Mmu.Verify(x => x.WriteWord(SP, IX), Times.Once);
        }

        [Test]
        public void EX_mSP_IY()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x1234;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            RunWithHalt(6, 23, PrimaryOpCode.Prefix_FD, PrimaryOpCode.EX_mSP_HL);

            Registers.VerifySet(x => x.IY = Value, Times.Once);
            Assert.AreEqual(Value, Registers.Object.IY);

            // SP unchanged
            Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
            Assert.AreEqual(SP, Registers.Object.StackPointer);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            Mmu.Verify(x => x.WriteWord(SP, IY), Times.Once);
        }

        [Test]
        public void EXX()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 4, PrimaryOpCode.EXX);

            Registers.Verify(x => x.SwitchToAlternativeGeneralPurposeRegisters(), Times.Once);
        }
    }
}