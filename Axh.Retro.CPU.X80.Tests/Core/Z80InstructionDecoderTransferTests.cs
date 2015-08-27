namespace Axh.Retro.CPU.X80.Tests.Core
{
    using System.ComponentModel;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoderTransferTests : Z80InstructionDecoderBase
    {

        [Test]
        public void LDI()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            Run(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDI);

            this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == DE + 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL + 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == BC - 1), Times.Once);
            
            this.Mmu.Verify(x => x.TransferByte(HL, DE), Times.Once);

            this.FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.ParityOverflow = It.Is<bool>(y => y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }

        [Test]
        public void LDI_BC0()
        {
            const ushort BC1 = 0x0001;

            this.SetupRegisters(bc: BC1);
            this.ResetMocks();
            
            Run(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDI);

            this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == DE + 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL + 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == 0), Times.Once);

            this.Mmu.Verify(x => x.TransferByte(HL, DE), Times.Once);

            this.FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.ParityOverflow = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }

        [Test]
        public void LDIR()
        {
            const ushort Length = 0x0005;

            this.SetupRegisters(bc: Length);
            this.ResetMocks();
            
            Run((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDIR);

            for(var i = 1; i < Length; i++)
            {
                this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == DE + i), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL + i), Times.Once);
                this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == Length - i), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                this.Mmu.Verify(x => x.TransferByte((ushort)(HL + i), (ushort)(DE + i)), Times.Once);
            }

            this.FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.ParityOverflow = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }

        [Test]
        public void LDD()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDD);

            this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == DE - 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL - 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == BC - 1), Times.Once);

            this.Mmu.Verify(x => x.TransferByte(HL, DE), Times.Once);

            this.FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.ParityOverflow = It.Is<bool>(y => y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }


        [Test]
        public void LDD_BC0()
        {
            const ushort BC1 = 0x0001;

            this.SetupRegisters(bc: BC1);
            this.ResetMocks();

            Run(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDD);

            this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == DE - 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL - 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == 0), Times.Once);

            this.Mmu.Verify(x => x.TransferByte(HL, DE), Times.Once);

            this.FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.ParityOverflow = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }

        [Test]
        public void LDDR()
        {
            const ushort Length = 0x0005;

            this.SetupRegisters(bc: Length);
            this.ResetMocks();

            Run((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LDDR);

            for (var i = 1; i < Length; i++)
            {
                this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == DE - i), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL - i), Times.Once);
                this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == Length - i), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                this.Mmu.Verify(x => x.TransferByte((ushort)(HL - i), (ushort)(DE - i)), Times.Once);
            }

            this.FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.ParityOverflow = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }
    }
}
