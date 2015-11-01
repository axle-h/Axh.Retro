namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class InputOutputTests : InstructionBlockDecoderTestsBase
    {
        [Test]
        public void IN_A_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Port = 0xac;
            const byte Value = 0x5e;
            this.Io.Setup(x => x.ReadByte(Port, A)).Returns(Value);

            RunWithHalt(3, 11, PrimaryOpCode.IN_A_n, Port);

            this.Io.Verify(x => x.ReadByte(Port, A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [TestCase(PrefixEdOpCode.IN_A_C)]
        [TestCase(PrefixEdOpCode.IN_B_C)]
        [TestCase(PrefixEdOpCode.IN_C_C)]
        [TestCase(PrefixEdOpCode.IN_D_C)]
        [TestCase(PrefixEdOpCode.IN_E_C)]
        [TestCase(PrefixEdOpCode.IN_F_C)]
        [TestCase(PrefixEdOpCode.IN_H_C)]
        [TestCase(PrefixEdOpCode.IN_L_C)]
        public void IN_r_C(PrefixEdOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            const byte Value = 0x5e;
            this.Io.Setup(x => x.ReadByte(C, B)).Returns(Value);

            RunWithHalt(3, 12, PrimaryOpCode.Prefix_ED, opCode);

            this.Io.Verify(x => x.ReadByte(C, B), Times.Once);

            switch (opCode)
            {
                case PrefixEdOpCode.IN_A_C:
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_B_C:
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_C_C:
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_D_C:
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_E_C:
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_F_C:
                    this.FlagsRegister.VerifySet(x => x.Register = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_H_C:
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_L_C:
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        [Test]
        public void INI()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            const byte Value = 0xb9;
            this.Io.Setup(x => x.ReadByte(C, B)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.INI);

            this.Io.Verify(x => x.ReadByte(C, B), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Value), Times.Once);
            this.GpRegisters.VerifySet(x => x.B = B - 1, Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = HL + 1, Times.Once);
            this.FlagsRegister.Verify(x => x.SetResultFlags(B - 1), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = true, Times.Once);
        }

        [Test]
        public void INIR()
        {
            const ushort Length = B;

            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xb9;
            this.Io.Setup(x => x.ReadByte(C, It.IsAny<byte>())).Returns(Value);

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.INIR);

            for (var i = 1; i < Length; i++)
            {
                var index = i;
                this.GpRegisters.VerifySet(x => x.B = (byte)(B - index), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = (ushort)(HL + index), Times.Once);
                this.FlagsRegister.Verify(x => x.SetResultFlags((byte)(B - index)), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                this.Io.Verify(x => x.ReadByte(C, (byte)(B - index)), Times.Once);
                this.Mmu.Verify(x => x.WriteByte((ushort)(HL + index), Value), Times.Once);
            }

            this.FlagsRegister.VerifySet(x => x.Subtract = true, Times.AtLeastOnce);
        }
        
        [Test]
        public void IND()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xb9;
            this.Io.Setup(x => x.ReadByte(C, B)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.IND);

            this.Io.Verify(x => x.ReadByte(C, B), Times.Once);
            this.Mmu.Verify(x => x.WriteByte(HL, Value), Times.Once);
            this.GpRegisters.VerifySet(x => x.B = B - 1, Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = HL - 1, Times.Once);
            this.FlagsRegister.Verify(x => x.SetResultFlags(B - 1), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = true, Times.Once);
        }

        [Test]
        public void INDR()
        {
            const ushort Length = B;

            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xb9;
            this.Io.Setup(x => x.ReadByte(C, It.IsAny<byte>())).Returns(Value);

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.INDR);

            for (var i = 1; i < Length; i++)
            {
                var index = i;
                this.GpRegisters.VerifySet(x => x.B = (byte)(B - index), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = (ushort)(HL - index), Times.Once);
                this.FlagsRegister.Verify(x => x.SetResultFlags((byte)(B - index)), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                this.Io.Verify(x => x.ReadByte(C, (byte)(B - index)), Times.Once);
                this.Mmu.Verify(x => x.WriteByte((ushort)(HL - index), Value), Times.Once);
            }

            this.FlagsRegister.VerifySet(x => x.Subtract = true, Times.AtLeastOnce);
        }
        
        [Test]
        public void OUT_A_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Port = 0xdb;

            RunWithHalt(3, 11, PrimaryOpCode.OUT_A_n, Port);

            this.Io.Verify(x => x.WriteByte(Port, A, A), Times.Once);
        }

        [TestCase(PrefixEdOpCode.OUT_A_C)]
        [TestCase(PrefixEdOpCode.OUT_B_C)]
        [TestCase(PrefixEdOpCode.OUT_C_C)]
        [TestCase(PrefixEdOpCode.OUT_D_C)]
        [TestCase(PrefixEdOpCode.OUT_E_C)]
        [TestCase(PrefixEdOpCode.OUT_F_C)]
        [TestCase(PrefixEdOpCode.OUT_H_C)]
        [TestCase(PrefixEdOpCode.OUT_L_C)]
        public void OUT_r_C(PrefixEdOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            RunWithHalt(3, 12, PrimaryOpCode.Prefix_ED, opCode);
            
            switch (opCode)
            {
                case PrefixEdOpCode.OUT_A_C:
                    this.Io.Verify(x => x.WriteByte(C, B, A), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_B_C:
                    this.Io.Verify(x => x.WriteByte(C, B, B), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_C_C:
                    this.Io.Verify(x => x.WriteByte(C, B, C), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_D_C:
                    this.Io.Verify(x => x.WriteByte(C, B, D), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_E_C:
                    this.Io.Verify(x => x.WriteByte(C, B, E), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_F_C:
                    this.Io.Verify(x => x.WriteByte(C, B, F), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_H_C:
                    this.Io.Verify(x => x.WriteByte(C, B, H), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_L_C:
                    this.Io.Verify(x => x.WriteByte(C, B, L), Times.Once);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        [Test]
        public void OUTI()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xb1;
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.OUTI);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Io.Verify(x => x.WriteByte(C, B, Value), Times.Once);
            this.GpRegisters.VerifySet(x => x.B = B - 1, Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = HL + 1, Times.Once);
            this.FlagsRegister.Verify(x => x.SetResultFlags(B - 1), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = true, Times.Once);
        }

        [Test]
        public void OUTIR()
        {
            const ushort Length = B;

            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xb9;
            this.Mmu.Setup(x => x.ReadByte(It.IsAny<ushort>())).Returns(Value);

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.OUTIR);

            for (var i = 1; i < Length; i++)
            {
                var index = i;
                this.GpRegisters.VerifySet(x => x.B = (byte)(B - index), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = (ushort)(HL + index), Times.Once);
                this.FlagsRegister.Verify(x => x.SetResultFlags((byte)(B - index)), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                this.Mmu.Verify(x => x.ReadByte((ushort)(HL + index)), Times.Once);
                this.Io.Verify(x => x.WriteByte(C, (byte)(B - index), Value), Times.Once);
            }

            this.FlagsRegister.VerifySet(x => x.Subtract = true, Times.AtLeastOnce);
        }

        [Test]
        public void OUTD()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xb1;
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.OUTD);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Io.Verify(x => x.WriteByte(C, B, Value), Times.Once);
            this.GpRegisters.VerifySet(x => x.B = B - 1, Times.Once);
            this.GpRegisters.VerifySet(x => x.HL = HL - 1, Times.Once);
            this.FlagsRegister.Verify(x => x.SetResultFlags(B - 1), Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = true, Times.Once);
        }

        [Test]
        public void OUTDR()
        {
            const ushort Length = B;

            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xb9;
            this.Mmu.Setup(x => x.ReadByte(It.IsAny<ushort>())).Returns(Value);

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.OUTDR);

            for (var i = 1; i < Length; i++)
            {
                var index = i;
                this.GpRegisters.VerifySet(x => x.B = (byte)(B - index), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = (ushort)(HL - index), Times.Once);
                this.FlagsRegister.Verify(x => x.SetResultFlags((byte)(B - index)), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                this.Mmu.Verify(x => x.ReadByte((ushort)(HL - index)), Times.Once);
                this.Io.Verify(x => x.WriteByte(C, (byte)(B - index), Value), Times.Once);
            }

            this.FlagsRegister.VerifySet(x => x.Subtract = true, Times.AtLeastOnce);
        }
    }
}
