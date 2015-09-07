namespace Axh.Retro.CPU.X80.Tests.Core.Z80InstructionDecoder
{
    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ArithmeticGeneralPurposeTests : Z80InstructionDecoderTestsBase
    {
        [Test]
        public void DAA()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            const byte Expected = 0xac;
            this.Alu.Setup(x => x.DecimalAdjust(A)).Returns(Expected);

            Run(1, 4, PrimaryOpCode.DAA);

            this.Alu.Verify(x => x.DecimalAdjust(A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void CPL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Expected = unchecked ((byte)~A);

            Run(1, 4, PrimaryOpCode.CPL);

            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
            this.FlagsRegister.VerifySet(x => x.HalfCarry = true);
            this.FlagsRegister.VerifySet(x => x.Subtract = true);
            this.FlagsRegister.Verify(x => x.SetUndocumentedFlags(Expected), Times.Once);
        }


        [Test]
        public void NEG()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Expected = unchecked((byte)(0 - A));

            this.Alu.Setup(x => x.Subtract(0, A)).Returns(Expected);

            Run(2, 8, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.NEG);

            this.Alu.Verify(x => x.Subtract(0, A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
        }

        [Test]
        public void CCF()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(1, 4, PrimaryOpCode.CCF);

            this.FlagsRegister.VerifySet(x => x.HalfCarry = false);
            this.FlagsRegister.VerifySet(x => x.Subtract = false);
            this.FlagsRegister.VerifySet(x => x.Carry = true);
        }


        [Test]
        public void SCF()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(1, 4, PrimaryOpCode.SCF);

            this.FlagsRegister.VerifySet(x => x.HalfCarry = false);
            this.FlagsRegister.VerifySet(x => x.Subtract = false);
            this.FlagsRegister.VerifySet(x => x.Carry = true);
        }

        [Test]
        public void DI()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(1, 4, PrimaryOpCode.DI);

            this.Registers.VerifySet(x => x.InterruptFlipFlop1 = false, Times.Once);
            this.Registers.VerifySet(x => x.InterruptFlipFlop2 = false, Times.Once);
        }

        [Test]
        public void EI()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(1, 4, PrimaryOpCode.EI);

            this.Registers.VerifySet(x => x.InterruptFlipFlop1 = true, Times.Once);
            this.Registers.VerifySet(x => x.InterruptFlipFlop2 = true, Times.Once);
        }

        [Test]
        public void IM0()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            Run(2, 8, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.IM0);

            this.Registers.VerifySet(x => x.InterruptMode = InterruptMode.InterruptMode0, Times.Once);
        }

        [Test]
        public void IM1()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(2, 8, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.IM1);

            this.Registers.VerifySet(x => x.InterruptMode = InterruptMode.InterruptMode1, Times.Once);
        }


        [Test]
        public void IM2()
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run(2, 8, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.IM2);

            this.Registers.VerifySet(x => x.InterruptMode = InterruptMode.InterruptMode2, Times.Once);
        }
    }
}
