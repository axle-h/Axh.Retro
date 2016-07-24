using System;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class Load16BitTests : InstructionBlockDecoderTestsBase
    {
        public Load16BitTests() : base(CpuMode.Z80)
        {
        }

        [TestCase(PrimaryOpCode.LD_BC_nn)]
        [TestCase(PrimaryOpCode.LD_DE_nn)]
        [TestCase(PrimaryOpCode.LD_HL_nn)]
        [TestCase(PrimaryOpCode.LD_SP_nn)]
        public void LD_dd_nn(PrimaryOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x9178;

            RunWithHalt(3, 10, opcode, Value);

            switch (opcode)
            {
                case PrimaryOpCode.LD_BC_nn:
                    Assert.AreEqual(Value, GpRegisters.BC);
                    break;
                case PrimaryOpCode.LD_DE_nn:
                    Assert.AreEqual(Value, GpRegisters.DE);
                    break;
                case PrimaryOpCode.LD_HL_nn:
                    Assert.AreEqual(Value, GpRegisters.HL);
                    break;
                case PrimaryOpCode.LD_SP_nn:
                    Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, Registers.Object.StackPointer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(PrefixEdOpCode.LD_BC_mnn)]
        [TestCase(PrefixEdOpCode.LD_DE_mnn)]
        [TestCase(PrefixEdOpCode.LD_HL_mnn)]
        [TestCase(PrefixEdOpCode.LD_SP_mnn)]
        public void LD_dd_mnn(PrefixEdOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x2dad;
            const ushort Value = 0x53e6;

            Mmu.Setup(x => x.ReadWord(NN)).Returns(Value);

            RunWithHalt(6, 20, PrimaryOpCode.Prefix_ED, opcode, NN);

            Mmu.Verify(x => x.ReadWord(NN), Times.Once);

            switch (opcode)
            {
                case PrefixEdOpCode.LD_BC_mnn:
                    Assert.AreEqual(Value, GpRegisters.BC);
                    break;
                case PrefixEdOpCode.LD_DE_mnn:
                    Assert.AreEqual(Value, GpRegisters.DE);
                    break;
                case PrefixEdOpCode.LD_HL_mnn:
                    Assert.AreEqual(Value, GpRegisters.HL);
                    break;
                case PrefixEdOpCode.LD_SP_mnn:
                    Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, Registers.Object.StackPointer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(PrefixEdOpCode.LD_mnn_BC)]
        [TestCase(PrefixEdOpCode.LD_mnn_DE)]
        [TestCase(PrefixEdOpCode.LD_mnn_HL)]
        [TestCase(PrefixEdOpCode.LD_mnn_SP)]
        public void LD_mnn_dd(PrefixEdOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x3836;

            RunWithHalt(6, 20, PrimaryOpCode.Prefix_ED, opcode, NN);

            switch (opcode)
            {
                case PrefixEdOpCode.LD_mnn_BC:
                    Mmu.Verify(x => x.WriteWord(NN, BC), Times.Once);
                    break;
                case PrefixEdOpCode.LD_mnn_DE:
                    Mmu.Verify(x => x.WriteWord(NN, DE), Times.Once);
                    break;
                case PrefixEdOpCode.LD_mnn_HL:
                    Mmu.Verify(x => x.WriteWord(NN, HL), Times.Once);
                    break;
                case PrefixEdOpCode.LD_mnn_SP:
                    Mmu.Verify(x => x.WriteWord(NN, SP), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(PrimaryOpCode.PUSH_BC)]
        [TestCase(PrimaryOpCode.PUSH_DE)]
        [TestCase(PrimaryOpCode.PUSH_HL)]
        [TestCase(PrimaryOpCode.PUSH_AF)]
        public void PUSH_qq(PrimaryOpCode opCode)
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(3, 11, opCode);

            const ushort SP2 = unchecked((ushort) (SP - 2));

            switch (opCode)
            {
                case PrimaryOpCode.PUSH_BC:
                    Mmu.Verify(x => x.WriteWord(SP2, BC), Times.Once);
                    break;
                case PrimaryOpCode.PUSH_DE:
                    Mmu.Verify(x => x.WriteWord(SP2, DE), Times.Once);
                    break;
                case PrimaryOpCode.PUSH_HL:
                    Mmu.Verify(x => x.WriteWord(SP2, HL), Times.Once);
                    break;
                case PrimaryOpCode.PUSH_AF:
                    Mmu.Verify(x => x.WriteWord(SP2, AF), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            Registers.VerifySet(x => x.StackPointer = SP2, Times.Once);
            Assert.AreEqual(SP2, Registers.Object.StackPointer);
        }

        [TestCase(PrimaryOpCode.POP_BC)]
        [TestCase(PrimaryOpCode.POP_DE)]
        [TestCase(PrimaryOpCode.POP_HL)]
        [TestCase(PrimaryOpCode.POP_AF)]
        public void POP_qq(PrimaryOpCode opCode)
        {
            SetupRegisters();
            ResetMocks();

            const ushort SP2 = unchecked((ushort) (SP + 2));

            const ushort Value = 0x1234;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            RunWithHalt(3, 10, opCode);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);

            switch (opCode)
            {
                case PrimaryOpCode.POP_BC:
                    Assert.AreEqual(Value, GpRegisters.BC);
                    break;
                case PrimaryOpCode.POP_DE:
                    Assert.AreEqual(Value, GpRegisters.DE);
                    break;
                case PrimaryOpCode.POP_HL:
                    Assert.AreEqual(Value, GpRegisters.HL);
                    break;
                case PrimaryOpCode.POP_AF:
                    Assert.AreEqual(Value, AfRegisters.AF);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opCode));
            }

            Registers.VerifySet(x => x.StackPointer = SP2, Times.Once);
            Assert.AreEqual(SP2, Registers.Object.StackPointer);
        }

        [Test]
        public void LD_HL_mnn()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x6b09;
            const ushort Value = 0x1173;

            Mmu.Setup(x => x.ReadWord(NN)).Returns(Value);

            RunWithHalt(5, 16, PrimaryOpCode.LD_HL_mnn, NN);

            Mmu.Verify(x => x.ReadWord(NN), Times.Once);
            Assert.AreEqual(Value, GpRegisters.HL);
        }

        [Test]
        public void LD_IX_mnn()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x2e7a;
            const ushort Value = 0x1f52;

            Mmu.Setup(x => x.ReadWord(NN)).Returns(Value);

            RunWithHalt(6, 20, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_HL_mnn, NN);

            Mmu.Verify(x => x.ReadWord(NN), Times.Once);

            Registers.VerifySet(x => x.IX = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, Registers.Object.IX);
        }

        [Test]
        public void LD_IX_nn()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x3829;

            RunWithHalt(4, 14, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_HL_nn, Value);

            Registers.VerifySet(x => x.IX = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, Registers.Object.IX);
        }

        [Test]
        public void LD_IY_mnn()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x09b7;
            const ushort Value = 0x5cd5;

            Mmu.Setup(x => x.ReadWord(NN)).Returns(Value);

            RunWithHalt(6, 20, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_HL_mnn, NN);

            Mmu.Verify(x => x.ReadWord(NN), Times.Once);

            Registers.VerifySet(x => x.IY = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, Registers.Object.IY);
        }

        [Test]
        public void LD_IY_nn()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x93b8;

            RunWithHalt(4, 14, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_HL_nn, Value);

            Registers.VerifySet(x => x.IY = It.Is<ushort>(y => y == Value), Times.Once);
            Assert.AreEqual(Value, Registers.Object.IY);
        }

        [Test]
        public void LD_mnn_HL()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x0f17;

            RunWithHalt(5, 16, PrimaryOpCode.LD_mnn_HL, NN);

            Mmu.Verify(x => x.WriteWord(NN, HL), Times.Once);
        }

        [Test]
        public void LD_mnn_IX()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x937a;

            RunWithHalt(6, 20, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_mnn_HL, NN);

            Mmu.Verify(x => x.WriteWord(NN, IX), Times.Once);
        }

        [Test]
        public void LD_mnn_IY()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x9240;

            RunWithHalt(6, 20, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_mnn_HL, NN);

            Mmu.Verify(x => x.WriteWord(NN, IY), Times.Once);
        }

        [Test]
        public void LD_SP_HL()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 6, PrimaryOpCode.LD_SP_HL);

            Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == HL), Times.Once);
        }

        [Test]
        public void LD_SP_IX()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 10, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_SP_HL);

            Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == IX), Times.Once);
        }

        [Test]
        public void LD_SP_IY()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 10, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_SP_HL);

            Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == IY), Times.Once);
        }

        [Test]
        public void POP_IX()
        {
            SetupRegisters();
            ResetMocks();

            const ushort SP2 = unchecked((ushort) (SP + 2));
            const ushort Value = 0x1234;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            RunWithHalt(4, 14, PrimaryOpCode.Prefix_DD, PrimaryOpCode.POP_HL);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);

            Registers.VerifySet(x => x.IX = Value, Times.Once);

            Registers.VerifySet(x => x.StackPointer = SP2, Times.Once);
            Assert.AreEqual(SP2, Registers.Object.StackPointer);
        }

        [Test]
        public void POP_IY()
        {
            SetupRegisters();
            ResetMocks();

            const ushort SP2 = unchecked((ushort) (SP + 2));
            const ushort Value = 0x1234;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            RunWithHalt(4, 14, PrimaryOpCode.Prefix_FD, PrimaryOpCode.POP_HL);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);

            Registers.VerifySet(x => x.IY = Value, Times.Once);

            Registers.VerifySet(x => x.StackPointer = SP2, Times.Once);
            Assert.AreEqual(SP2, Registers.Object.StackPointer);
        }

        [Test]
        public void PUSH_IX()
        {
            SetupRegisters();
            ResetMocks();

            const ushort SP2 = unchecked((ushort) (SP - 2));

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_DD, PrimaryOpCode.PUSH_HL);

            Mmu.Verify(x => x.WriteWord(SP2, IX), Times.Once);

            Registers.VerifySet(x => x.StackPointer = SP2, Times.Once);
            Assert.AreEqual(SP2, Registers.Object.StackPointer);
        }

        [Test]
        public void PUSH_IY()
        {
            SetupRegisters();
            ResetMocks();

            const ushort SP2 = unchecked((ushort) (SP - 2));

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_FD, PrimaryOpCode.PUSH_HL);

            Mmu.Verify(x => x.WriteWord(SP2, IY), Times.Once);

            Registers.VerifySet(x => x.StackPointer = SP2, Times.Once);
            Assert.AreEqual(SP2, Registers.Object.StackPointer);
        }
    }
}