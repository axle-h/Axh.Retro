using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class ArithmeticGeneralPurposeTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public ArithmeticGeneralPurposeTests() : base(CpuMode.Z80)
        {
        }

        [Test]
        public void CCF()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 4, PrimaryOpCode.CCF);

            FlagsRegister.VerifySet(x => x.HalfCarry = false);
            FlagsRegister.VerifySet(x => x.Subtract = false);
            FlagsRegister.VerifySet(x => x.Carry = true);
        }

        [Test]
        public void CPL()
        {
            SetupRegisters();
            ResetMocks();

            const byte Expected = unchecked ((byte) ~A);

            RunWithHalt(1, 4, PrimaryOpCode.CPL);

            AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
            FlagsRegister.VerifySet(x => x.HalfCarry = true);
            FlagsRegister.VerifySet(x => x.Subtract = true);
            FlagsRegister.Verify(x => x.SetUndocumentedFlags(Expected), Times.Once);
        }

        [Test]
        public void DAA()
        {
            SetupRegisters();
            ResetMocks();

            const byte Expected = 0xac;
            Alu.Setup(x => x.DecimalAdjust(A, true)).Returns(Expected);

            RunWithHalt(1, 4, PrimaryOpCode.DAA);

            Alu.Verify(x => x.DecimalAdjust(A, true), Times.Once);
            AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void DI()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 4, PrimaryOpCode.DI);

            Registers.VerifySet(x => x.InterruptFlipFlop1 = false, Times.Once);
            Registers.VerifySet(x => x.InterruptFlipFlop2 = false, Times.Once);
        }

        [Test]
        public void EI()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 4, PrimaryOpCode.EI);

            Registers.VerifySet(x => x.InterruptFlipFlop1 = true, Times.Once);
            Registers.VerifySet(x => x.InterruptFlipFlop2 = true, Times.Once);
        }

        [Test]
        public void IM0()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.IM0);

            Registers.VerifySet(x => x.InterruptMode = InterruptMode.InterruptMode0, Times.Once);
        }

        [Test]
        public void IM1()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.IM1);

            Registers.VerifySet(x => x.InterruptMode = InterruptMode.InterruptMode1, Times.Once);
        }


        [Test]
        public void IM2()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.IM2);

            Registers.VerifySet(x => x.InterruptMode = InterruptMode.InterruptMode2, Times.Once);
        }


        [Test]
        public void NEG()
        {
            SetupRegisters();
            ResetMocks();

            const byte Expected = unchecked((byte) (0 - A));

            Alu.Setup(x => x.Subtract(0, A)).Returns(Expected);

            RunWithHalt(2, 8, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.NEG);

            Alu.Verify(x => x.Subtract(0, A), Times.Once);
            AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }


        [Test]
        public void SCF()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 4, PrimaryOpCode.SCF);

            FlagsRegister.VerifySet(x => x.HalfCarry = false);
            FlagsRegister.VerifySet(x => x.Subtract = false);
            FlagsRegister.VerifySet(x => x.Carry = true);
        }
    }
}