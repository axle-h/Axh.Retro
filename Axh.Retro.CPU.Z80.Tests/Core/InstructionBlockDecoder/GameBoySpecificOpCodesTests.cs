using System;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class GameBoySpecificOpCodesTests : InstructionBlockDecoderTestsBase<IRegisters>
    {
        public GameBoySpecificOpCodesTests() : base(CpuMode.GameBoy)
        {
        }

        [TestCase(GameBoyPrefixCbOpCode.SWAP_A)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_B)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_C)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_E)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_H)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_L)]
        public void SWAP_r(GameBoyPrefixCbOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xcd;
            Alu.Setup(x => x.Swap(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, null, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case GameBoyPrefixCbOpCode.SWAP_A:
                    Alu.Verify(x => x.Swap(A), Times.Once);
                    Assert.AreEqual(Value, AfRegisters.A);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_B:
                    Alu.Verify(x => x.Swap(B), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.B);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_C:
                    Alu.Verify(x => x.Swap(C), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.C);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_D:
                    Alu.Verify(x => x.Swap(D), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.D);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_E:
                    Alu.Verify(x => x.Swap(E), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.E);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_H:
                    Alu.Verify(x => x.Swap(H), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.H);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_L:
                    Alu.Verify(x => x.Swap(L), Times.Once);
                    Assert.AreEqual(Value, GpRegisters.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void LD_A_mFF00C()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xfe;
            const ushort Address = 0xff00 + C;

            Mmu.Setup(x => x.ReadByte(Address)).Returns(Value);

            RunWithHalt(2, null, GameBoyPrimaryOpCode.LD_A_mFF00C);

            Mmu.Verify(x => x.ReadByte(Address), Times.Once);
            Assert.AreEqual(Value, AfRegisters.A);
        }

        [Test]
        public void LD_A_mFF00n()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xfe;
            const ushort Address = 0xff00 + Value;

            Mmu.Setup(x => x.ReadByte(Address)).Returns(Value);

            RunWithHalt(3, null, GameBoyPrimaryOpCode.LD_A_mFF00n, Value);

            Mmu.Verify(x => x.ReadByte(Address), Times.Once);
            Assert.AreEqual(Value, AfRegisters.A);
        }

        [Test]
        public void LD_A_mnn()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x827d;
            const byte ValueAtNN = 0xcf;

            Mmu.Setup(x => x.ReadByte(NN)).Returns(ValueAtNN);

            RunWithHalt(4, null, GameBoyPrimaryOpCode.LD_A_mnn, NN);

            Mmu.Verify(x => x.ReadByte(NN), Times.Once);
            Assert.AreEqual(ValueAtNN, AfRegisters.A);
        }

        [Test]
        public void LD_HL_SPd()
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = 64;
            const ushort Value = SP + Displacement;

            Alu.Setup(x => x.AddDisplacement(SP, Displacement)).Returns(Value);

            RunWithHalt(3, null, GameBoyPrimaryOpCode.LD_HL_SPd, unchecked((byte) Displacement));

            Alu.Verify(x => x.AddDisplacement(SP, Displacement), Times.Once);
            Assert.AreEqual(Value, GpRegisters.HL);
        }

        [Test]
        public void LD_mFF00C_A()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, null, GameBoyPrimaryOpCode.LD_mFF00C_A);

            Mmu.Verify(x => x.WriteByte(0xff00 + C, A), Times.Once);
        }

        [Test]
        public void LD_mFF00n_A()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x1e;

            RunWithHalt(3, null, GameBoyPrimaryOpCode.LD_mFF00n_A, Value);

            Mmu.Verify(x => x.WriteByte(0xff00 + Value, A), Times.Once);
        }

        [Test]
        public void LD_mnn_A()
        {
            SetupRegisters();
            ResetMocks();
            const ushort N = 0x613b;

            RunWithHalt(4, null, GameBoyPrimaryOpCode.LD_mnn_A, N);

            Mmu.Verify(x => x.WriteByte(N, A), Times.Once);
        }

        [Test]
        public void LD_mnn_SP()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x3836;

            RunWithHalt(5, null, GameBoyPrimaryOpCode.LD_mnn_SP, NN);

            Mmu.Verify(x => x.WriteWord(NN, SP), Times.Once);
        }

        [Test]
        public void LD_SP_SPd()
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = -127;
            const ushort Value = SP + Displacement;

            Alu.Setup(x => x.AddDisplacement(SP, Displacement)).Returns(Value);

            RunWithHalt(4, null, GameBoyPrimaryOpCode.ADD_SP_d, unchecked((byte) Displacement));

            Alu.Verify(x => x.AddDisplacement(SP, Displacement), Times.Once);
            Registers.VerifySet(x => x.StackPointer = Value, Times.Once);
        }

        [Test]
        public void LDD_A_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb9;

            Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(2, null, GameBoyPrimaryOpCode.LDD_A_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Assert.AreEqual(Value, AfRegisters.A);
            Assert.AreEqual(HL - 1, GpRegisters.HL);
        }

        [Test]
        public void LDD_mHL_A()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, null, GameBoyPrimaryOpCode.LDD_mHL_A);

            Mmu.Verify(x => x.WriteByte(HL, A), Times.Once);
            Assert.AreEqual(HL - 1, GpRegisters.HL);
        }

        [Test]
        public void LDI_A_mHL()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb9;

            Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(2, null, GameBoyPrimaryOpCode.LDI_A_mHL);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Assert.AreEqual(Value, AfRegisters.A);
            Assert.AreEqual(HL + 1, GpRegisters.HL);
        }

        [Test]
        public void LDI_mHL_A()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, null, GameBoyPrimaryOpCode.LDI_mHL_A);

            Mmu.Verify(x => x.WriteByte(HL, A), Times.Once);
            Assert.AreEqual(HL + 1, GpRegisters.HL);
        }

        [Test]
        public void RETI()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0xd466;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            Run(4, null, GameBoyPrimaryOpCode.RETI);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);
        }

        [Test]
        public void STOP()
        {
            ResetMocks();

            Run(1, null, GameBoyPrimaryOpCode.STOP);

            Cache.Verify(x => x.NextByte(), Times.Once);
        }
    }
}