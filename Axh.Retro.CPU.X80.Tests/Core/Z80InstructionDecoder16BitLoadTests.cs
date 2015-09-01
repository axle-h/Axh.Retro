namespace Axh.Retro.CPU.X80.Tests.Core
{
    using System;

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [Test]
        public void LD_IX_nn()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x3829;

            Run(4, 14, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_HL_nn, Value);

            this.Registers.VerifySet(x => x.IX = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, this.Registers.Object.IX);
        }

        [Test]
        public void LD_IY_nn()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x93b8;

            Run(4, 14, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_HL_nn, Value);

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
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

            Run(6, 20, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_HL_mnn, NN);

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

            Run(6, 20, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_HL_mnn, NN);

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

            Run(5, 16, PrimaryOpCode.LD_mnn_HL, NN);

            this.Mmu.Verify(x => x.WriteWord(NN, HL), Times.Once);
        }

        [TestCase(PrefixEdOpCode.LD_mnn_BC)]
        [TestCase(PrefixEdOpCode.LD_mnn_DE)]
        [TestCase(PrefixEdOpCode.LD_mnn_HL)]
        [TestCase(PrefixEdOpCode.LD_mnn_SP)]
        public void LD_mnn_dd(PrefixEdOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x3836;

            Run(6, 20, PrimaryOpCode.Prefix_ED, opcode, NN);

            switch (opcode)
            {
                case PrefixEdOpCode.LD_mnn_BC:
                    this.Mmu.Verify(x => x.WriteWord(NN, BC), Times.Once);
                    break;
                case PrefixEdOpCode.LD_mnn_DE:
                    this.Mmu.Verify(x => x.WriteWord(NN, DE), Times.Once);
                    break;
                case PrefixEdOpCode.LD_mnn_HL:
                    this.Mmu.Verify(x => x.WriteWord(NN, HL), Times.Once);
                    break;
                case PrefixEdOpCode.LD_mnn_SP:
                    this.Mmu.Verify(x => x.WriteWord(NN, SP), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [Test]
        public void LD_mnn_IX()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x937a;

            Run(6, 20, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_mnn_HL, NN);

            this.Mmu.Verify(x => x.WriteWord(NN, IX), Times.Once);
        }

        [Test]
        public void LD_mnn_IY()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x9240;

            Run(6, 20, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_mnn_HL, NN);

            this.Mmu.Verify(x => x.WriteWord(NN, IY), Times.Once);
        }

        [Test]
        public void LD_SP_HL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(1, 6, PrimaryOpCode.LD_SP_HL);

            this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == HL), Times.Once);
        }

        [Test]
        public void LD_SP_IX()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(2, 10, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_SP_HL);

            this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == IX), Times.Once);
        }

        [Test]
        public void LD_SP_IY()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(2, 10, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_SP_HL);

            this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == IY), Times.Once);
        }

        [TestCase(PrimaryOpCode.PUSH_BC)]
        [TestCase(PrimaryOpCode.PUSH_DE)]
        [TestCase(PrimaryOpCode.PUSH_HL)]
        [TestCase(PrimaryOpCode.PUSH_AF)]
        public void PUSH_qq(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(3, 11, opCode);

            const ushort SP1 = unchecked((ushort)(SP - 1));
            const ushort SP2 = unchecked((ushort)(SP1 - 1));
            
            switch (opCode)
            {
                case PrimaryOpCode.PUSH_BC:
                    this.Mmu.Verify(x => x.WriteByte(SP1, B), Times.Once);
                    this.Mmu.Verify(x => x.WriteByte(SP2, C), Times.Once);
                    break;
                case PrimaryOpCode.PUSH_DE:
                    this.Mmu.Verify(x => x.WriteByte(SP1, D), Times.Once);
                    this.Mmu.Verify(x => x.WriteByte(SP2, E), Times.Once);
                    break;
                case PrimaryOpCode.PUSH_HL:
                    this.Mmu.Verify(x => x.WriteByte(SP1, H), Times.Once);
                    this.Mmu.Verify(x => x.WriteByte(SP2, L), Times.Once);
                    break;
                case PrimaryOpCode.PUSH_AF:
                    this.Mmu.Verify(x => x.WriteByte(SP1, A), Times.Once);
                    this.Mmu.Verify(x => x.WriteByte(SP2, F), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }
            
            this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == SP2), Times.Once);
            Assert.AreEqual(SP2, this.Registers.Object.StackPointer);
        }

        [Test]
        public void PUSH_IX()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort SP1 = unchecked((ushort)(SP - 1));
            const ushort SP2 = unchecked((ushort)(SP1 - 1));

            Run(4, 15, PrimaryOpCode.Prefix_DD, PrimaryOpCode.PUSH_HL);

            this.Mmu.Verify(x => x.WriteByte(SP1, IXh), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(SP2, IXl), Times.Once);
            
            this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == SP2), Times.Once);
            Assert.AreEqual(SP2, this.Registers.Object.StackPointer);
        }

        [Test]
        public void PUSH_IY()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort SP1 = unchecked((ushort)(SP - 1));
            const ushort SP2 = unchecked((ushort)(SP1 - 1));

            Run(4, 15, PrimaryOpCode.Prefix_FD, PrimaryOpCode.PUSH_HL);

            this.Mmu.Verify(x => x.WriteByte(SP1, IYh), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(SP2, IYl), Times.Once);

            this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == SP2), Times.Once);
            Assert.AreEqual(SP2, this.Registers.Object.StackPointer);
        }

        [TestCase(PrimaryOpCode.POP_BC)]
        [TestCase(PrimaryOpCode.POP_DE)]
        [TestCase(PrimaryOpCode.POP_HL)]
        [TestCase(PrimaryOpCode.POP_AF)]
        public void POP_qq(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort SP1 = unchecked((ushort)(SP + 1));
            const ushort SP2 = unchecked((ushort)(SP1 + 1));

            const byte Value1 = 0x12;
            const byte Value2 = 0x34;

            this.Mmu.Setup(x => x.ReadByte(SP)).Returns(Value1);
            this.Mmu.Setup(x => x.ReadByte(SP1)).Returns(Value2);

            Run(3, 10, opCode);

            this.Mmu.Verify(x => x.ReadByte(SP), Times.Once);
            this.Mmu.Verify(x => x.ReadByte(SP1), Times.Once);

            switch (opCode)
            {
                case PrimaryOpCode.POP_BC:
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == Value2), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == Value1), Times.Once);
                    break;
                case PrimaryOpCode.POP_DE:
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == Value2), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == Value1), Times.Once);
                    break;
                case PrimaryOpCode.POP_HL:
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == Value2), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == Value1), Times.Once);
                    break;
                case PrimaryOpCode.POP_AF:
                    this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == Value2), Times.Once);
                    this.FlagsRegister.VerifySet(x => x.Register = It.Is<byte>(y => y == Value1), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == SP2), Times.Once);
            Assert.AreEqual(SP2, this.Registers.Object.StackPointer);
        }

        [Test]
        public void POP_IX()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort SP1 = unchecked((ushort)(SP + 1));
            const ushort SP2 = unchecked((ushort)(SP1 + 1));

            const byte Value1 = 0x12;
            const byte Value2 = 0x34;

            this.Mmu.Setup(x => x.ReadByte(SP)).Returns(Value1);
            this.Mmu.Setup(x => x.ReadByte(SP1)).Returns(Value2);

            this.Mmu.Setup(x => x.ReadByte(SP)).Returns(Value1);
            this.Mmu.Setup(x => x.ReadByte(SP1)).Returns(Value2);

            Run(4, 14, PrimaryOpCode.Prefix_DD, PrimaryOpCode.POP_HL);

            this.Mmu.Verify(x => x.ReadByte(SP), Times.Once);
            this.Mmu.Verify(x => x.ReadByte(SP1), Times.Once);

            this.Registers.VerifySet(x => x.IXh = It.Is<byte>(y => y == Value2), Times.Once);
            this.Registers.VerifySet(x => x.IXl = It.Is<byte>(y => y == Value1), Times.Once);
            
            this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == SP2), Times.Once);
            Assert.AreEqual(SP2, this.Registers.Object.StackPointer);
        }


        [Test]
        public void POP_IY()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort SP1 = unchecked((ushort)(SP + 1));
            const ushort SP2 = unchecked((ushort)(SP1 + 1));

            const byte Value1 = 0x12;
            const byte Value2 = 0x34;

            this.Mmu.Setup(x => x.ReadByte(SP)).Returns(Value1);
            this.Mmu.Setup(x => x.ReadByte(SP1)).Returns(Value2);

            this.Mmu.Setup(x => x.ReadByte(SP)).Returns(Value1);
            this.Mmu.Setup(x => x.ReadByte(SP1)).Returns(Value2);

            Run(4, 14, PrimaryOpCode.Prefix_FD, PrimaryOpCode.POP_HL);

            this.Mmu.Verify(x => x.ReadByte(SP), Times.Once);
            this.Mmu.Verify(x => x.ReadByte(SP1), Times.Once);

            this.Registers.VerifySet(x => x.IYh = It.Is<byte>(y => y == Value2), Times.Once);
            this.Registers.VerifySet(x => x.IYl = It.Is<byte>(y => y == Value1), Times.Once);

            this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == SP2), Times.Once);
            Assert.AreEqual(SP2, this.Registers.Object.StackPointer);
        }
    }
}
