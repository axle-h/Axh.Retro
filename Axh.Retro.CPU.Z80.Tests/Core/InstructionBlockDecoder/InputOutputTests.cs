using System;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class InputOutputTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public InputOutputTests() : base(CpuMode.Z80)
        {
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
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x5e;
            Io.Setup(x => x.ReadByteFromPort(C, B)).Returns(Value);

            RunWithHalt(3, 12, PrimaryOpCode.Prefix_ED, opCode);

            Io.Verify(x => x.ReadByteFromPort(C, B), Times.Once);

            switch (opCode)
            {
                case PrefixEdOpCode.IN_A_C:
                    AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_B_C:
                    GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_C_C:
                    GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_D_C:
                    GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_E_C:
                    GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_F_C:
                    FlagsRegister.VerifySet(x => x.Register = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_H_C:
                    GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_L_C:
                    GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new NotSupportedException();
            }
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
            SetupRegisters();
            ResetMocks();

            RunWithHalt(3, 12, PrimaryOpCode.Prefix_ED, opCode);

            switch (opCode)
            {
                case PrefixEdOpCode.OUT_A_C:
                    Io.Verify(x => x.WriteByteToPort(C, B, A), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_B_C:
                    Io.Verify(x => x.WriteByteToPort(C, B, B), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_C_C:
                    Io.Verify(x => x.WriteByteToPort(C, B, C), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_D_C:
                    Io.Verify(x => x.WriteByteToPort(C, B, D), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_E_C:
                    Io.Verify(x => x.WriteByteToPort(C, B, E), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_F_C:
                    Io.Verify(x => x.WriteByteToPort(C, B, F), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_H_C:
                    Io.Verify(x => x.WriteByteToPort(C, B, H), Times.Once);
                    break;
                case PrefixEdOpCode.OUT_L_C:
                    Io.Verify(x => x.WriteByteToPort(C, B, L), Times.Once);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        [Test]
        public void IN_A_n()
        {
            SetupRegisters();
            ResetMocks();

            const byte Port = 0xac;
            const byte Value = 0x5e;
            Io.Setup(x => x.ReadByteFromPort(Port, A)).Returns(Value);

            RunWithHalt(3, 11, PrimaryOpCode.IN_A_n, Port);

            Io.Verify(x => x.ReadByteFromPort(Port, A), Times.Once);
            AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [Test]
        public void IND()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb9;
            Io.Setup(x => x.ReadByteFromPort(C, B)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.IND);

            Io.Verify(x => x.ReadByteFromPort(C, B), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Value), Times.Once);
            GpRegisters.VerifySet(x => x.B = B - 1, Times.Once);
            GpRegisters.VerifySet(x => x.HL = HL - 1, Times.Once);
            FlagsRegister.Verify(x => x.SetResultFlags(B - 1), Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = true, Times.Once);
        }

        [Test]
        public void INDR()
        {
            const ushort Length = B;

            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb9;
            Io.Setup(x => x.ReadByteFromPort(C, It.IsAny<byte>())).Returns(Value);

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.INDR);

            for (var i = 1; i < Length; i++)
            {
                var index = i;
                GpRegisters.VerifySet(x => x.B = (byte) (B - index), Times.Once);
                GpRegisters.VerifySet(x => x.HL = (ushort) (HL - index), Times.Once);
                FlagsRegister.Verify(x => x.SetResultFlags((byte) (B - index)), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                Io.Verify(x => x.ReadByteFromPort(C, (byte) (B - index)), Times.Once);
                Mmu.Verify(x => x.WriteByte((ushort) (HL - index), Value), Times.Once);
            }

            FlagsRegister.VerifySet(x => x.Subtract = true, Times.AtLeastOnce);
        }

        [Test]
        public void INI()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb9;
            Io.Setup(x => x.ReadByteFromPort(C, B)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.INI);

            Io.Verify(x => x.ReadByteFromPort(C, B), Times.Once);
            Mmu.Verify(x => x.WriteByte(HL, Value), Times.Once);
            GpRegisters.VerifySet(x => x.B = B - 1, Times.Once);
            GpRegisters.VerifySet(x => x.HL = HL + 1, Times.Once);
            FlagsRegister.Verify(x => x.SetResultFlags(B - 1), Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = true, Times.Once);
        }

        [Test]
        public void INIR()
        {
            const ushort Length = B;

            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb9;
            Io.Setup(x => x.ReadByteFromPort(C, It.IsAny<byte>())).Returns(Value);

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.INIR);

            for (var i = 1; i < Length; i++)
            {
                var index = i;
                GpRegisters.VerifySet(x => x.B = (byte) (B - index), Times.Once);
                GpRegisters.VerifySet(x => x.HL = (ushort) (HL + index), Times.Once);
                FlagsRegister.Verify(x => x.SetResultFlags((byte) (B - index)), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                Io.Verify(x => x.ReadByteFromPort(C, (byte) (B - index)), Times.Once);
                Mmu.Verify(x => x.WriteByte((ushort) (HL + index), Value), Times.Once);
            }

            FlagsRegister.VerifySet(x => x.Subtract = true, Times.AtLeastOnce);
        }

        [Test]
        public void OUT_A_n()
        {
            SetupRegisters();
            ResetMocks();

            const byte Port = 0xdb;

            RunWithHalt(3, 11, PrimaryOpCode.OUT_A_n, Port);

            Io.Verify(x => x.WriteByteToPort(Port, A, A), Times.Once);
        }

        [Test]
        public void OUTD()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb1;
            Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.OUTD);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Io.Verify(x => x.WriteByteToPort(C, B, Value), Times.Once);
            GpRegisters.VerifySet(x => x.B = B - 1, Times.Once);
            GpRegisters.VerifySet(x => x.HL = HL - 1, Times.Once);
            FlagsRegister.Verify(x => x.SetResultFlags(B - 1), Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = true, Times.Once);
        }

        [Test]
        public void OUTDR()
        {
            const ushort Length = B;

            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb9;
            Mmu.Setup(x => x.ReadByte(It.IsAny<ushort>())).Returns(Value);

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.OUTDR);

            for (var i = 1; i < Length; i++)
            {
                var index = i;
                GpRegisters.VerifySet(x => x.B = (byte) (B - index), Times.Once);
                GpRegisters.VerifySet(x => x.HL = (ushort) (HL - index), Times.Once);
                FlagsRegister.Verify(x => x.SetResultFlags((byte) (B - index)), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                Mmu.Verify(x => x.ReadByte((ushort) (HL - index)), Times.Once);
                Io.Verify(x => x.WriteByteToPort(C, (byte) (B - index), Value), Times.Once);
            }

            FlagsRegister.VerifySet(x => x.Subtract = true, Times.AtLeastOnce);
        }

        [Test]
        public void OUTI()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb1;
            Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.OUTI);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Io.Verify(x => x.WriteByteToPort(C, B, Value), Times.Once);
            GpRegisters.VerifySet(x => x.B = B - 1, Times.Once);
            GpRegisters.VerifySet(x => x.HL = HL + 1, Times.Once);
            FlagsRegister.Verify(x => x.SetResultFlags(B - 1), Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = true, Times.Once);
        }

        [Test]
        public void OUTIR()
        {
            const ushort Length = B;

            SetupRegisters();
            ResetMocks();

            const byte Value = 0xb9;
            Mmu.Setup(x => x.ReadByte(It.IsAny<ushort>())).Returns(Value);

            RunWithHalt((Length - 1) * 5 + 4, (Length - 1) * 21 + 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.OUTIR);

            for (var i = 1; i < Length; i++)
            {
                var index = i;
                GpRegisters.VerifySet(x => x.B = (byte) (B - index), Times.Once);
                GpRegisters.VerifySet(x => x.HL = (ushort) (HL + index), Times.Once);
                FlagsRegister.Verify(x => x.SetResultFlags((byte) (B - index)), Times.Once);
            }

            for (var i = 0; i < Length; i++)
            {
                var index = i;
                Mmu.Verify(x => x.ReadByte((ushort) (HL + index)), Times.Once);
                Io.Verify(x => x.WriteByteToPort(C, (byte) (B - index), Value), Times.Once);
            }

            FlagsRegister.VerifySet(x => x.Subtract = true, Times.AtLeastOnce);
        }
    }
}