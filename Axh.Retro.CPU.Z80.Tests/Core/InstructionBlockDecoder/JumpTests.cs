using System;
using System.Linq.Expressions;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class JumpTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public JumpTests() : base(CpuMode.Z80)
        {
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void JP_IXY(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            Run(2, 8, prefix, PrimaryOpCode.JP_mHL);

            var index = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            Registers.VerifySet(x => x.ProgramCounter = index, Times.Once);
        }

        private void TestAbsoluteJump(PrimaryOpCode opCode,
                                      Expression<Func<IFlagsRegister, bool>> propertyLambda,
                                      bool flagValue,
                                      bool expectJump)
        {
            const ushort Value = 0x1be7;

            SetupRegisters();
            ResetMocks();
            FlagsRegister.SetupProperty(propertyLambda, flagValue);

            Run(3, 10, opCode, Value);

            if (expectJump)
            {
                // Jump
                Registers.VerifySet(x => x.ProgramCounter = PC + 3, Times.Never);
                Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            }
            else
            {
                // No Jump
                Registers.VerifySet(x => x.ProgramCounter = PC + 3, Times.Once);
                Registers.VerifySet(x => x.ProgramCounter = Value, Times.Never);
            }
        }

        private void TestRelativeJump(PrimaryOpCode opCode,
                                      Expression<Func<IFlagsRegister, bool>> propertyLambda,
                                      bool flagValue,
                                      bool expectJump)
        {
            const sbyte Displacement = -45;

            SetupRegisters();
            ResetMocks();

            FlagsRegister.SetupProperty(propertyLambda, flagValue);

            var m = 2;
            var t = 7;
            if (expectJump)
            {
                m += 1;
                t += 5;
            }

            Run(m, t, opCode, unchecked((byte) Displacement));

            if (expectJump)
            {
                // Jump
                Registers.VerifySet(x => x.ProgramCounter = unchecked(PC + 2), Times.Never);
                Registers.VerifySet(x => x.ProgramCounter = unchecked(PC + Displacement + 2), Times.Once);
            }
            else
            {
                // No Jump
                Registers.VerifySet(x => x.ProgramCounter = unchecked(PC + 2), Times.Once);
                Registers.VerifySet(x => x.ProgramCounter = unchecked(PC + Displacement + 2), Times.Never);
            }
        }

        [Test]
        public void DJNZ_Jump()
        {
            SetupRegisters();
            ResetMocks();

            GpRegisters.SetupProperty(x => x.B, (byte) 2);

            const sbyte Displacement = -30;
            const ushort DisplacedIndex = unchecked(PC + Displacement + 2);

            Run(3, 13, PrimaryOpCode.DJNZ, unchecked((byte) Displacement));

            GpRegisters.VerifySet(x => x.B = 1, Times.Once);
            Registers.VerifySet(x => x.ProgramCounter = PC + 2, Times.Never);
            Registers.VerifySet(x => x.ProgramCounter = DisplacedIndex, Times.Once);
        }

        [Test]
        public void DJNZ_NoJump()
        {
            SetupRegisters();
            ResetMocks();

            GpRegisters.SetupProperty(x => x.B, (byte) 1);

            const sbyte Displacement = -30;
            const ushort DisplacedIndex = unchecked(PC + Displacement + 2);

            Run(2, 8, PrimaryOpCode.DJNZ, unchecked((byte) Displacement));

            GpRegisters.VerifySet(x => x.B = 0, Times.Once);
            Registers.VerifySet(x => x.ProgramCounter = PC + 2, Times.Once);
            Registers.VerifySet(x => x.ProgramCounter = DisplacedIndex, Times.Never);
        }

        [Test]
        public void JP()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x1be7;

            Run(3, 10, PrimaryOpCode.JP, Value);

            Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
        }

        [Test]
        public void JP_C()
        {
            TestAbsoluteJump(PrimaryOpCode.JP_C, x => x.Carry, true, true);
            TestAbsoluteJump(PrimaryOpCode.JP_C, x => x.Carry, false, false);
        }

        [Test]
        public void JP_M()
        {
            TestAbsoluteJump(PrimaryOpCode.JP_M, x => x.Sign, true, true);
            TestAbsoluteJump(PrimaryOpCode.JP_M, x => x.Sign, false, false);
        }

        [Test]
        public void JP_mHL()
        {
            SetupRegisters();
            ResetMocks();

            Run(1, 4, PrimaryOpCode.JP_mHL);

            Registers.VerifySet(x => x.ProgramCounter = HL, Times.Once);
        }

        [Test]
        public void JP_NC()
        {
            TestAbsoluteJump(PrimaryOpCode.JP_NC, x => x.Carry, true, false);
            TestAbsoluteJump(PrimaryOpCode.JP_NC, x => x.Carry, false, true);
        }

        [Test]
        public void JP_NZ()
        {
            TestAbsoluteJump(PrimaryOpCode.JP_NZ, x => x.Zero, true, false);
            TestAbsoluteJump(PrimaryOpCode.JP_NZ, x => x.Zero, false, true);
        }

        [Test]
        public void JP_P()
        {
            TestAbsoluteJump(PrimaryOpCode.JP_P, x => x.Sign, true, false);
            TestAbsoluteJump(PrimaryOpCode.JP_P, x => x.Sign, false, true);
        }

        [Test]
        public void JP_PE()
        {
            TestAbsoluteJump(PrimaryOpCode.JP_PE, x => x.ParityOverflow, true, true);
            TestAbsoluteJump(PrimaryOpCode.JP_PE, x => x.ParityOverflow, false, false);
        }

        [Test]
        public void JP_PO()
        {
            TestAbsoluteJump(PrimaryOpCode.JP_PO, x => x.ParityOverflow, true, false);
            TestAbsoluteJump(PrimaryOpCode.JP_PO, x => x.ParityOverflow, false, true);
        }

        [Test]
        public void JP_Z()
        {
            TestAbsoluteJump(PrimaryOpCode.JP_Z, x => x.Zero, true, true);
            TestAbsoluteJump(PrimaryOpCode.JP_Z, x => x.Zero, false, false);
        }

        [Test]
        public void JR()
        {
            SetupRegisters();
            ResetMocks();

            const sbyte Displacement = 30;
            const ushort DisplacedIndex = unchecked(PC + Displacement + 2);

            Run(3, 12, PrimaryOpCode.JR, unchecked((byte) Displacement));

            Registers.VerifySet(x => x.ProgramCounter = DisplacedIndex, Times.Once);
        }

        [Test]
        public void JR_C()
        {
            TestRelativeJump(PrimaryOpCode.JR_C, x => x.Carry, true, true);
            TestRelativeJump(PrimaryOpCode.JR_C, x => x.Carry, false, false);
        }


        [Test]
        public void JR_NC()
        {
            TestRelativeJump(PrimaryOpCode.JR_NC, x => x.Carry, true, false);
            TestRelativeJump(PrimaryOpCode.JR_NC, x => x.Carry, false, true);
        }


        [Test]
        public void JR_NZ()
        {
            TestRelativeJump(PrimaryOpCode.JR_NZ, x => x.Zero, true, false);
            TestRelativeJump(PrimaryOpCode.JR_NZ, x => x.Zero, false, true);
        }


        [Test]
        public void JR_Z()
        {
            TestRelativeJump(PrimaryOpCode.JR_Z, x => x.Zero, true, true);
            TestRelativeJump(PrimaryOpCode.JR_Z, x => x.Zero, false, false);
        }
    }
}