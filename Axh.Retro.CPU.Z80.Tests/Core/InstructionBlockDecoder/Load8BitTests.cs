using System;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Tests.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class Load8BitTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public Load8BitTests() : base(CpuMode.Z80)
        {
        }
        
        [TestCaseSource(nameof(SimpleLoadOpCodes))]
        public void LD_r_r(PrimaryOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 4, opcode);

            AssertLoad(ParseSourceRegister(opcode), ParseTargetRegister(opcode));
        }

        private static readonly PrimaryOpCode[] SimpleLoadOpCodes =
        {
            PrimaryOpCode.LD_A_A, PrimaryOpCode.LD_A_B, PrimaryOpCode.LD_A_C,
            PrimaryOpCode.LD_A_D, PrimaryOpCode.LD_A_E, PrimaryOpCode.LD_A_H,
            PrimaryOpCode.LD_A_L, PrimaryOpCode.LD_B_A, PrimaryOpCode.LD_B_B,
            PrimaryOpCode.LD_B_C, PrimaryOpCode.LD_B_D, PrimaryOpCode.LD_B_E,
            PrimaryOpCode.LD_B_H, PrimaryOpCode.LD_B_L, PrimaryOpCode.LD_C_A,
            PrimaryOpCode.LD_C_B, PrimaryOpCode.LD_C_C, PrimaryOpCode.LD_C_D,
            PrimaryOpCode.LD_C_E, PrimaryOpCode.LD_C_H, PrimaryOpCode.LD_C_L,
            PrimaryOpCode.LD_D_A, PrimaryOpCode.LD_D_B, PrimaryOpCode.LD_D_C,
            PrimaryOpCode.LD_D_D, PrimaryOpCode.LD_D_E, PrimaryOpCode.LD_D_H,
            PrimaryOpCode.LD_D_L, PrimaryOpCode.LD_E_A, PrimaryOpCode.LD_E_B,
            PrimaryOpCode.LD_E_C, PrimaryOpCode.LD_E_D, PrimaryOpCode.LD_E_E,
            PrimaryOpCode.LD_E_H, PrimaryOpCode.LD_E_L, PrimaryOpCode.LD_H_A,
            PrimaryOpCode.LD_H_B, PrimaryOpCode.LD_H_C, PrimaryOpCode.LD_H_D,
            PrimaryOpCode.LD_H_E, PrimaryOpCode.LD_H_H, PrimaryOpCode.LD_H_L,
            PrimaryOpCode.LD_L_A, PrimaryOpCode.LD_L_B, PrimaryOpCode.LD_L_C,
            PrimaryOpCode.LD_L_D, PrimaryOpCode.LD_L_E, PrimaryOpCode.LD_L_H,
            PrimaryOpCode.LD_L_L
        };

        [TestCaseSource(nameof(LiteralLoadOpCodes))]
        public void LD_r_n(PrimaryOpCode opcode)
        {
            const byte Value = 0x56;

            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 7, opcode, Value);

            AssertLoad(Value, ParseTargetRegister(opcode));
        }

        private static readonly PrimaryOpCode[] LiteralLoadOpCodes =
        {
            PrimaryOpCode.LD_A_n, PrimaryOpCode.LD_B_n,
            PrimaryOpCode.LD_C_n, PrimaryOpCode.LD_D_n,
            PrimaryOpCode.LD_E_n, PrimaryOpCode.LD_H_n,
            PrimaryOpCode.LD_L_n
        };

        [TestCase(PrimaryOpCode.LD_A_mHL)]
        [TestCase(PrimaryOpCode.LD_B_mHL)]
        [TestCase(PrimaryOpCode.LD_C_mHL)]
        [TestCase(PrimaryOpCode.LD_D_mHL)]
        [TestCase(PrimaryOpCode.LD_E_mHL)]
        [TestCase(PrimaryOpCode.LD_H_mHL)]
        [TestCase(PrimaryOpCode.LD_L_mHL)]
        public void LD_r_mHL(PrimaryOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x8f;

            Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(2, 7, opcode);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);

            AssertLoad(Value, ParseTargetRegister(opcode));
        }


        [TestCase(PrimaryOpCode.LD_A_mHL)]
        [TestCase(PrimaryOpCode.LD_B_mHL)]
        [TestCase(PrimaryOpCode.LD_C_mHL)]
        [TestCase(PrimaryOpCode.LD_D_mHL)]
        [TestCase(PrimaryOpCode.LD_E_mHL)]
        [TestCase(PrimaryOpCode.LD_H_mHL)]
        [TestCase(PrimaryOpCode.LD_L_mHL)]
        public void LD_r_mIXd(PrimaryOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = -127;
            const byte UnsignedD = unchecked((byte) SignedD);
            const byte ValueAtIXd = 0x8f;

            Mmu.Setup(x => x.ReadByte(IX + SignedD)).Returns(ValueAtIXd);
            RunWithHalt(5, 19, PrimaryOpCode.Prefix_DD, opcode, UnsignedD);

            Mmu.Verify(x => x.ReadByte(IX + SignedD), Times.Once);
            Registers.VerifyGet(x => x.IX, Times.Once);

            AssertLoad(ValueAtIXd, ParseTargetRegister(opcode));
        }

        [TestCase(PrimaryOpCode.LD_A_mHL)]
        [TestCase(PrimaryOpCode.LD_B_mHL)]
        [TestCase(PrimaryOpCode.LD_C_mHL)]
        [TestCase(PrimaryOpCode.LD_D_mHL)]
        [TestCase(PrimaryOpCode.LD_E_mHL)]
        [TestCase(PrimaryOpCode.LD_H_mHL)]
        [TestCase(PrimaryOpCode.LD_L_mHL)]
        public void LD_r_mIYd(PrimaryOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = 55;
            const byte UnsignedD = unchecked((byte) SignedD);
            const byte ValueAtIYd = 0x42;

            Mmu.Setup(x => x.ReadByte(IY + SignedD)).Returns(ValueAtIYd);

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_FD, opcode, UnsignedD);

            Mmu.Verify(x => x.ReadByte(IY + SignedD), Times.Once);
            Registers.VerifyGet(x => x.IY, Times.Once);

            AssertLoad(ValueAtIYd, ParseTargetRegister(opcode));
        }

        [TestCase(PrimaryOpCode.LD_mHL_A)]
        [TestCase(PrimaryOpCode.LD_mHL_B)]
        [TestCase(PrimaryOpCode.LD_mHL_C)]
        [TestCase(PrimaryOpCode.LD_mHL_D)]
        [TestCase(PrimaryOpCode.LD_mHL_E)]
        [TestCase(PrimaryOpCode.LD_mHL_H)]
        [TestCase(PrimaryOpCode.LD_mHL_L)]
        public void LD_mHL_r(PrimaryOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 7, opcode);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    Mmu.Verify(x => x.WriteByte(HL, A), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    Mmu.Verify(x => x.WriteByte(HL, B), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    Mmu.Verify(x => x.WriteByte(HL, C), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    Mmu.Verify(x => x.WriteByte(HL, D), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    Mmu.Verify(x => x.WriteByte(HL, E), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    Mmu.Verify(x => x.WriteByte(HL, H), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    Mmu.Verify(x => x.WriteByte(HL, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(0x12)]
        [TestCase(0x34)]
        public void LD_mHL_n(byte value)
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(3, 10, PrimaryOpCode.LD_mHL_n, value);

            Mmu.Verify(x => x.WriteByte(HL, value), Times.Once);
        }

        [TestCase(PrimaryOpCode.LD_mHL_A)]
        [TestCase(PrimaryOpCode.LD_mHL_B)]
        [TestCase(PrimaryOpCode.LD_mHL_C)]
        [TestCase(PrimaryOpCode.LD_mHL_D)]
        [TestCase(PrimaryOpCode.LD_mHL_E)]
        [TestCase(PrimaryOpCode.LD_mHL_H)]
        [TestCase(PrimaryOpCode.LD_mHL_L)]
        public void LD_mIXd_r(PrimaryOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = -66;
            const byte UnsignedD = unchecked((byte) SignedD);

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_DD, opcode, UnsignedD);

            Registers.VerifyGet(x => x.IX, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, A), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, B), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, C), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, D), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, E), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, H), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(PrimaryOpCode.LD_mHL_A)]
        [TestCase(PrimaryOpCode.LD_mHL_B)]
        [TestCase(PrimaryOpCode.LD_mHL_C)]
        [TestCase(PrimaryOpCode.LD_mHL_D)]
        [TestCase(PrimaryOpCode.LD_mHL_E)]
        [TestCase(PrimaryOpCode.LD_mHL_H)]
        [TestCase(PrimaryOpCode.LD_mHL_L)]
        public void LD_mIYd_r(PrimaryOpCode opcode)
        {
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = 88;
            const byte UnsignedD = unchecked((byte) SignedD);

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_FD, opcode, UnsignedD);

            Registers.VerifyGet(x => x.IY, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, A), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, B), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, C), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, D), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, E), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, H), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [Test]
        public void LD_A_I()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_A_I);
            
            Assert.AreEqual(I, AfRegisters.A);

            // Check flags
            FlagsRegister.Verify(x => x.SetResultFlags(It.Is<byte>(y => y == I)), Times.Once);
            FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = IFF2, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }

        [Test]
        public void LD_A_mBC()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtBC = 0x61;

            Mmu.Setup(x => x.ReadByte(BC)).Returns(ValueAtBC);

            RunWithHalt(2, 7, PrimaryOpCode.LD_A_mBC);

            Mmu.Verify(x => x.ReadByte(BC), Times.Once);
            Assert.AreEqual(ValueAtBC, AfRegisters.A);
        }

        [Test]
        public void LD_A_mDE()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtDE = 0x0c;

            Mmu.Setup(x => x.ReadByte(DE)).Returns(ValueAtDE);

            RunWithHalt(2, 7, PrimaryOpCode.LD_A_mDE);

            Mmu.Verify(x => x.ReadByte(DE), Times.Once);
            Assert.AreEqual(ValueAtDE, AfRegisters.A);
        }

        [Test]
        public void LD_A_mnn()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x827d;
            const byte ValueAtNN = 0xcf;

            Mmu.Setup(x => x.ReadByte(NN)).Returns(ValueAtNN);

            RunWithHalt(4, 13, PrimaryOpCode.LD_A_mnn, NN);

            Mmu.Verify(x => x.ReadByte(NN), Times.Once);
            Assert.AreEqual(ValueAtNN, AfRegisters.A);
        }

        [Test]
        public void LD_A_R()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_A_R);
            
            Assert.AreEqual(R, AfRegisters.A);

            // Check flags
            FlagsRegister.Verify(x => x.SetResultFlags(It.Is<byte>(y => y == R)), Times.Once);
            FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = IFF2, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }

        [Test]
        public void LD_I_A()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_I_A);

            Registers.VerifySet(x => x.I = It.Is<byte>(y => y == A), Times.Once);
            Assert.AreEqual(A, Registers.Object.I);
        }

        [Test]
        public void LD_mBC_A()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 7, PrimaryOpCode.LD_mBC_A);

            Mmu.Verify(x => x.WriteByte(BC, A), Times.Once);
        }

        [Test]
        public void LD_mDE_A()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 7, PrimaryOpCode.LD_mDE_A);

            Mmu.Verify(x => x.WriteByte(DE, A), Times.Once);
        }

        [Test]
        public void LD_mIXd_n()
        {
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = 77;
            const byte UnsignedD = unchecked((byte) SignedD);
            const byte N = 0x73;

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_mHL_n, UnsignedD, N);

            Registers.VerifyGet(x => x.IX, Times.Once);
            Mmu.Verify(x => x.WriteByte(IX + SignedD, N), Times.Once);
        }

        [Test]
        public void LD_mIYd_n()
        {
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = -77;
            const byte UnsignedD = unchecked((byte) SignedD);
            const byte N = 0xF6;

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_mHL_n, UnsignedD, N);

            Registers.VerifyGet(x => x.IY, Times.Once);
            Mmu.Verify(x => x.WriteByte(IY + SignedD, N), Times.Once);
        }

        [Test]
        public void LD_mnn_A()
        {
            SetupRegisters();
            ResetMocks();
            const ushort N = 0x613b;

            RunWithHalt(4, 13, PrimaryOpCode.LD_mnn_A, N);

            Mmu.Verify(x => x.WriteByte(N, A), Times.Once);
        }

        [Test]
        public void LD_R_A()
        {
            SetupRegisters();
            ResetMocks();

            // This is not all it seems.
            // 1. A is loaded to R
            // 2. LS 7-bytes of R are incremented 3 times for the 3 opcodes executed.
            // Note: this is not cycle accurate. R should only be incremented twice as Prefix_ED has already been run. However, it really doesn't matter. 
            const int ExprectedR = ((A + 3) & 0x7f) | ((A & 0x80) >> 8);

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_R_A);

            Registers.VerifySet(x => x.R = It.Is<byte>(y => y == A), Times.Once);

            Assert.AreEqual(ExprectedR, Registers.Object.R);
        }


        private static CpuRegister ParseTargetRegister(PrimaryOpCode opcode)
        {
            var tokens = opcode.ToString().Split(new[] { '_' }, 3, StringSplitOptions.RemoveEmptyEntries);
            return (CpuRegister)Enum.Parse(typeof(CpuRegister), tokens[1]);
        }

        private static CpuRegister ParseSourceRegister(PrimaryOpCode opcode)
        {
            var tokens = opcode.ToString().Split(new[] { '_' }, 3, StringSplitOptions.RemoveEmptyEntries);
            return (CpuRegister)Enum.Parse(typeof(CpuRegister), tokens[2]);
        }

        private void AssertLoad(CpuRegister source, CpuRegister target)
        {
            var sourceValue = Get8BitRegisterInitialValue(source);
            var targetValue = Get8BitRegisterValue(target);

            Assert.AreEqual(sourceValue, targetValue);

            // Make sure all other registers have not changed.
            foreach (var register in target.Other8BitRegisters())
            {
                Assert.AreEqual(Get8BitRegisterInitialValue(register), Get8BitRegisterValue(register));
            }
        }

        private void AssertLoad(byte sourceValue, CpuRegister target)
        {
            var targetValue = Get8BitRegisterValue(target);

            Assert.AreEqual(sourceValue, targetValue);

            // Make sure all other registers have not changed.
            foreach (var register in target.Other8BitRegisters())
            {
                Assert.AreEqual(Get8BitRegisterInitialValue(register), Get8BitRegisterValue(register));
            }
        }
    }
}