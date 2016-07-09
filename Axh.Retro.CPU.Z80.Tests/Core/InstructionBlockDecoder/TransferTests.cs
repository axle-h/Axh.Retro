using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class TransferTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public TransferTests() : base(CpuMode.Z80)
        {
        }

        [Test]
        public void LDD()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDD);

            Assert.AreEqual(DE - 1, GpRegisters.DE);
            Assert.AreEqual(HL - 1, GpRegisters.HL);
            Assert.AreEqual(BC - 1, GpRegisters.BC);

            Mmu.Verify(x => x.TransferByte(HL, DE), Times.Once);

            FlagsRegister.VerifySet(x => x.HalfCarry = false, Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = true, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = false, Times.Once);
        }

        [Test]
        public void LDD_BC0()
        {
            const ushort BC1 = 0x0001;

            SetupRegisters(BC1);
            ResetMocks();

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDD);

            Assert.AreEqual(DE - 1, GpRegisters.DE);
            Assert.AreEqual(HL - 1, GpRegisters.HL);
            Assert.AreEqual(0, GpRegisters.BC);

            Mmu.Verify(x => x.TransferByte(HL, DE), Times.Once);

            FlagsRegister.VerifySet(x => x.HalfCarry = false, Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = false, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = false, Times.Once);
        }

        [Test]
        public void LDDR()
        {
            const ushort Length = 0x0005;

            SetupRegisters(Length);
            ResetMocks();

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDDR);

            Assert.AreEqual((ushort)(DE - Length), GpRegisters.DE);
            Assert.AreEqual((ushort)(HL - Length), GpRegisters.HL);
            Assert.AreEqual(0, GpRegisters.BC);

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                Mmu.Verify(x => x.TransferByte((ushort) (HL - index), (ushort) (DE - index)), Times.Once);
            }

            FlagsRegister.VerifySet(x => x.HalfCarry = false, Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = false, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = false, Times.Once);
        }

        [Test]
        public void LDI()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDI);

            Assert.AreEqual(DE + 1, GpRegisters.DE);
            Assert.AreEqual(HL + 1, GpRegisters.HL);
            Assert.AreEqual(BC - 1, GpRegisters.BC);

            Mmu.Verify(x => x.TransferByte(HL, DE), Times.Once);

            FlagsRegister.VerifySet(x => x.HalfCarry = false, Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = true, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = false, Times.Once);
        }

        [Test]
        public void LDI_BC0()
        {
            const ushort BC1 = 0x0001;

            SetupRegisters(BC1);
            ResetMocks();

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDI);

            Assert.AreEqual(DE + 1, GpRegisters.DE);
            Assert.AreEqual(HL + 1, GpRegisters.HL);
            Assert.AreEqual(0, GpRegisters.BC);

            Mmu.Verify(x => x.TransferByte(HL, DE), Times.Once);

            FlagsRegister.VerifySet(x => x.HalfCarry = false, Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = false, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = false, Times.Once);
        }

        [Test]
        public void LDIR()
        {
            const ushort Length = 0x0005;

            SetupRegisters(Length);
            ResetMocks();

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDIR);

            Assert.AreEqual(DE + Length, GpRegisters.DE);
            Assert.AreEqual(HL + Length, GpRegisters.HL);
            Assert.AreEqual(0, GpRegisters.BC);

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                Mmu.Verify(x => x.TransferByte((ushort) (HL + index), (ushort) (DE + index)), Times.Once);
            }

            FlagsRegister.VerifySet(x => x.HalfCarry = false, Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = false, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = false, Times.Once);
        }
    }
}