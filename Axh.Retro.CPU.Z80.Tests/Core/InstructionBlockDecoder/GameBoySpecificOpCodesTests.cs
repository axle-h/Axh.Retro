namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class GameBoySpecificOpCodesTests : InstructionBlockDecoderTestsBase<IIntel8080Registers>
    {
        public GameBoySpecificOpCodesTests() : base(CpuMode.GameBoy)
        {
        }

        [Test]
        public void LD_mnn_SP()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x3836;

            RunWithHalt(6, 20, GameBoyPrimaryOpCode.LD_mnn_SP, NN);

            this.Mmu.Verify(x => x.WriteWord(NN, SP), Times.Once);
        }

        [Test]
        public void STOP()
        {
            this.ResetMocks();
            
            this.Run(1, 4, GameBoyPrimaryOpCode.STOP);

            this.Cache.Verify(x => x.NextByte(), Times.Once);
        }

        [Test]
        public void LDI_mHL_A()
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(2, 8, GameBoyPrimaryOpCode.LDI_mHL_A);

            this.Mmu.Verify(x => x.WriteByte(HL, A), Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = HL + 1, Times.Once);
        }

        [Test]
        public void LDI_A_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xb9;

            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(2, 8, GameBoyPrimaryOpCode.LDI_A_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = HL + 1, Times.Once);
        }


        [Test]
        public void LDD_mHL_A()
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(2, 8, GameBoyPrimaryOpCode.LDD_mHL_A);

            this.Mmu.Verify(x => x.WriteByte(HL, A), Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = HL - 1, Times.Once);
        }

        [Test]
        public void LDD_A_mHL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xb9;

            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(2, 8, GameBoyPrimaryOpCode.LDD_A_mHL);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = HL - 1, Times.Once);
        }

        [Test]
        public void RETI()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0xd466;

            this.Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            Run(4, 16, GameBoyPrimaryOpCode.RETI);

            this.Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            this.Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            this.Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);
        }

        [Test]
        public void LD_mFF00n_A()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x1e;

            RunWithHalt(3, 12, GameBoyPrimaryOpCode.LD_mFF00n_A, Value);

            this.Mmu.Verify(x => x.WriteByte(0xff00 + Value, A), Times.Once);
        }


        [Test]
        public void LD_mFF00C_A()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            RunWithHalt(2, 8, GameBoyPrimaryOpCode.LD_mFF00C_A);

            this.Mmu.Verify(x => x.WriteByte(0xff00 + C, A), Times.Once);
        }

        [Test]
        public void LD_A_mFF00n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xfe;
            const ushort Address = 0xff00 + Value;

            this.Mmu.Setup(x => x.ReadByte(Address)).Returns(Value);

            RunWithHalt(3, 12, GameBoyPrimaryOpCode.LD_A_mFF00n, Value);

            this.Mmu.Verify(x => x.ReadByte(Address), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void LD_A_mFF00C()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xfe;
            const ushort Address = 0xff00 + C;

            this.Mmu.Setup(x => x.ReadByte(Address)).Returns(Value);

            RunWithHalt(2, 8, GameBoyPrimaryOpCode.LD_A_mFF00C);

            this.Mmu.Verify(x => x.ReadByte(Address), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }


        [Test]
        public void LD_mnn_A()
        {
            this.SetupRegisters();
            this.ResetMocks();
            const ushort N = 0x613b;

            RunWithHalt(2, 8, GameBoyPrimaryOpCode.LD_mnn_A, N);

            this.Mmu.Verify(x => x.WriteByte(N, A), Times.Once);
        }
        
        [Test]
        public void LD_A_mnn()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x827d;
            const byte ValueAtNN = 0xcf;

            this.Mmu.Setup(x => x.ReadByte(NN)).Returns(ValueAtNN);

            RunWithHalt(4, 16, GameBoyPrimaryOpCode.LD_A_mnn, NN);

            this.Mmu.Verify(x => x.ReadByte(NN), Times.Once);
            Assert.AreEqual(ValueAtNN, this.AfRegisters.Object.A);
        }

        [Test]
        public void ADD_SP_d()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = -127;
            const ushort Value = SP + Displacement;

            this.Alu.Setup(x => x.AddDisplacement(SP, Displacement)).Returns(Value);

            RunWithHalt(4, 16, GameBoyPrimaryOpCode.ADD_SP_d, unchecked((byte)Displacement));

            this.Alu.Verify(x => x.AddDisplacement(SP, Displacement), Times.Once);
            this.Registers.VerifySet(x => x.StackPointer = Value, Times.Once);
        }

        [Test]
        public void LD_HL_SPd()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte Displacement = 64;
            const ushort Value = SP + Displacement;

            this.Alu.Setup(x => x.AddDisplacement(SP, Displacement)).Returns(Value);

            RunWithHalt(3, 12, GameBoyPrimaryOpCode.LD_HL_SPd, unchecked((byte)Displacement));

            this.Alu.Verify(x => x.AddDisplacement(SP, Displacement), Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = Value, Times.Once);
        }

        [TestCase(GameBoyPrefixCbOpCode.SWAP_A)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_B)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_C)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_E)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_H)]
        [TestCase(GameBoyPrefixCbOpCode.SWAP_L)]
        public void SWAP_r(GameBoyPrefixCbOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xcd;
            this.Alu.Setup(x => x.Swap(It.IsAny<byte>())).Returns(Value);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_CB, opcode);

            switch (opcode)
            {
                case GameBoyPrefixCbOpCode.SWAP_A:
                    this.Alu.Verify(x => x.Swap(A), Times.Once);
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_B:
                    this.Alu.Verify(x => x.Swap(B), Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_C:
                    this.Alu.Verify(x => x.Swap(C), Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_D:
                    this.Alu.Verify(x => x.Swap(D), Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_E:
                    this.Alu.Verify(x => x.Swap(E), Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_H:
                    this.Alu.Verify(x => x.Swap(H), Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case GameBoyPrefixCbOpCode.SWAP_L:
                    this.Alu.Verify(x => x.Swap(L), Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
