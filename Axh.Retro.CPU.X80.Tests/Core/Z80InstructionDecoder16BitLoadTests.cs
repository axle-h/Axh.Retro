namespace Axh.Retro.CPU.X80.Tests.Core
{
    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoder16BitLoadTests : Z80InstructionDecoderBase
    {
        [TestCase(PrimaryOpCode.LD_BC_nn)]
        [TestCase(PrimaryOpCode.LD_DE_nn)]
        [TestCase(PrimaryOpCode.LD_HL_nn)]
        [TestCase(PrimaryOpCode.LD_SP_nn)]
        public void LD_dd_nn(PrimaryOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x9178;

            Run(2, 10, opcode, Value);

            switch (opcode)
            {
                case PrimaryOpCode.LD_BC_nn:
                    this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.GpRegisters.Object.BC);
                    break;
                case PrimaryOpCode.LD_DE_nn:
                    this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.GpRegisters.Object.DE);
                    break;
                case PrimaryOpCode.LD_HL_nn:
                    this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.GpRegisters.Object.HL);
                    break;
                case PrimaryOpCode.LD_SP_nn:
                    this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.Registers.Object.StackPointer);
                    break;
            }
        }

        [Test]
        public void LD_IX_nn()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x3829;

            Run(4, 14, PrimaryOpCode.Prefix_DD, PrefixDdFdOpCode.LD_IXY_nn, Value);

            this.Registers.VerifySet(x => x.IX = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, this.Registers.Object.IX);
        }

        [Test]
        public void LD_IY_nn()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x93b8;

            Run(4, 14, PrimaryOpCode.Prefix_FD, PrefixDdFdOpCode.LD_IXY_nn, Value);

            this.Registers.VerifySet(x => x.IY = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, this.Registers.Object.IY);
        }

        [Test]
        public void LD_HL_mnn()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x6b09;
            const ushort Value = 0x1173;

            this.Mmu.Setup(x => x.ReadWord(NN)).Returns(Value);

            Run(5, 16, PrimaryOpCode.LD_HL_mnn, NN);

            this.Mmu.Verify(x => x.ReadWord(NN), Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, this.GpRegisters.Object.HL);
        }

        [TestCase(PrefixEdOpCode.LD_BC_mnn)]
        [TestCase(PrefixEdOpCode.LD_DE_mnn)]
        [TestCase(PrefixEdOpCode.LD_HL_mnn)]
        [TestCase(PrefixEdOpCode.LD_SP_mnn)]
        public void LD_dd_mnn(PrefixEdOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x2dad;
            const ushort Value = 0x53e6;

            this.Mmu.Setup(x => x.ReadWord(NN)).Returns(Value);

            Run(6, 20, PrimaryOpCode.Prefix_ED, opcode, NN);

            this.Mmu.Verify(x => x.ReadWord(NN), Times.Once);

            switch (opcode)
            {
                case PrefixEdOpCode.LD_BC_mnn:
                    this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.GpRegisters.Object.BC);
                    break;
                case PrefixEdOpCode.LD_DE_mnn:
                    this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.GpRegisters.Object.DE);
                    break;
                case PrefixEdOpCode.LD_HL_mnn:
                    this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.GpRegisters.Object.HL);
                    break;
                case PrefixEdOpCode.LD_SP_mnn:
                    this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.Registers.Object.StackPointer);
                    break;
            }
        }

        [Test]
        public void LD_IX_mnn()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x2e7a;
            const ushort Value = 0x1f52;

            this.Mmu.Setup(x => x.ReadWord(NN)).Returns(Value);

            Run(6, 20, PrimaryOpCode.Prefix_DD, PrefixDdFdOpCode.LD_IXY_mnn, NN);

            this.Mmu.Verify(x => x.ReadWord(NN), Times.Once);

            this.Registers.VerifySet(x => x.IX = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, this.Registers.Object.IX);
        }

        [Test]
        public void LD_IY_mnn()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x09b7;
            const ushort Value = 0x5cd5;

            this.Mmu.Setup(x => x.ReadWord(NN)).Returns(Value);

            Run(6, 20, PrimaryOpCode.Prefix_FD, PrefixDdFdOpCode.LD_IXY_mnn, NN);

            this.Mmu.Verify(x => x.ReadWord(NN), Times.Once);

            this.Registers.VerifySet(x => x.IY = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, this.Registers.Object.IY);
        }

        [Test]
        public void LD_mnn_HL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x0f17;

            Run(6, 20, PrimaryOpCode.LD_mnn_HL, NN);

            this.Mmu.Verify(x => x.WriteWord(NN, HL), Times.Once);
        }
    }
}
