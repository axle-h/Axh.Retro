namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using System;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CallReturnResetTests: InstructionBlockDecoderTestsBase
    {
        [Test]
        public void CALL()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x6238;

            Run(5, 17, PrimaryOpCode.CALL, Value);
            
            this.Mmu.Verify(x => x.WriteWord(SP - 2, PC + 3), Times.Once);
            this.Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
        }

        [Test]
        public void CALL_NZ()
        {
            TestCall(PrimaryOpCode.CALL_NZ, x => x.Zero, true, false);
            TestCall(PrimaryOpCode.CALL_NZ, x => x.Zero, false, true);
        }
        
        [Test]
        public void CALL_Z()
        {
            TestCall(PrimaryOpCode.CALL_Z, x => x.Zero, true, true);
            TestCall(PrimaryOpCode.CALL_Z, x => x.Zero, false, false);
        }

        [Test]
        public void CALL_NC()
        {
            TestCall(PrimaryOpCode.CALL_NC, x => x.Carry, true, false);
            TestCall(PrimaryOpCode.CALL_NC, x => x.Carry, false, true);
        }

        [Test]
        public void CALL_C()
        {
            TestCall(PrimaryOpCode.CALL_C, x => x.Carry, true, true);
            TestCall(PrimaryOpCode.CALL_C, x => x.Carry, false, false);
        }

        [Test]
        public void CALL_PO()
        {
            TestCall(PrimaryOpCode.CALL_PO, x => x.ParityOverflow, true, false);
            TestCall(PrimaryOpCode.CALL_PO, x => x.ParityOverflow, false, true);
        }

        [Test]
        public void CALL_PE()
        {
            TestCall(PrimaryOpCode.CALL_PE, x => x.ParityOverflow, true, true);
            TestCall(PrimaryOpCode.CALL_PE, x => x.ParityOverflow, false, false);
        }

        [Test]
        public void CALL_P()
        {
            TestCall(PrimaryOpCode.CALL_P, x => x.Sign, true, false);
            TestCall(PrimaryOpCode.CALL_P, x => x.Sign, false, true);
        }

        [Test]
        public void CALL_M()
        {
            TestCall(PrimaryOpCode.CALL_M, x => x.Sign, true, true);
            TestCall(PrimaryOpCode.CALL_M, x => x.Sign, false, false);
        }

        [Test]
        public void RET()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0xd466;

            this.Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            Run(3, 10, PrimaryOpCode.RET);

            this.Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            this.Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            this.Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);
        }


        [Test]
        public void RET_NZ()
        {
            TestReturn(PrimaryOpCode.RET_NZ, x => x.Zero, true, false);
            TestReturn(PrimaryOpCode.RET_NZ, x => x.Zero, false, true);
        }

        [Test]
        public void RET_Z()
        {
            TestReturn(PrimaryOpCode.RET_Z, x => x.Zero, true, true);
            TestReturn(PrimaryOpCode.RET_Z, x => x.Zero, false, false);
        }

        [Test]
        public void RET_NC()
        {
            TestReturn(PrimaryOpCode.RET_NC, x => x.Carry, true, false);
            TestReturn(PrimaryOpCode.RET_NC, x => x.Carry, false, true);
        }

        [Test]
        public void RET_C()
        {
            TestReturn(PrimaryOpCode.RET_C, x => x.Carry, true, true);
            TestReturn(PrimaryOpCode.RET_C, x => x.Carry, false, false);
        }

        [Test]
        public void RET_PO()
        {
            TestReturn(PrimaryOpCode.RET_PO, x => x.ParityOverflow, true, false);
            TestReturn(PrimaryOpCode.RET_PO, x => x.ParityOverflow, false, true);
        }

        [Test]
        public void RET_PE()
        {
            TestReturn(PrimaryOpCode.RET_PE, x => x.ParityOverflow, true, true);
            TestReturn(PrimaryOpCode.RET_PE, x => x.ParityOverflow, false, false);
        }

        [Test]
        public void RET_P()
        {
            TestReturn(PrimaryOpCode.RET_P, x => x.Sign, true, false);
            TestReturn(PrimaryOpCode.RET_P, x => x.Sign, false, true);
        }

        [Test]
        public void RET_M()
        {
            TestReturn(PrimaryOpCode.RET_M, x => x.Sign, true, true);
            TestReturn(PrimaryOpCode.RET_M, x => x.Sign, false, false);
        }
        
        [Test]
        public void RETI()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0xd466;

            this.Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            Run(4, 14, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.RETI);

            this.Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            this.Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            this.Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);
        }

        [Test]
        public void RETN()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0xd466;

            this.Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            Run(4, 14, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.RETN);

            this.Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            this.Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            this.Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);

            // RETN The state of IFF2 is copied back to IFF1
            this.Registers.VerifySet(x => x.InterruptFlipFlop1 = IFF2, Times.Once);
        }

        [TestCase(PrimaryOpCode.RST_00, (ushort)0x0000)]
        [TestCase(PrimaryOpCode.RST_08, (ushort)0x0008)]
        [TestCase(PrimaryOpCode.RST_10, (ushort)0x0010)]
        [TestCase(PrimaryOpCode.RST_18, (ushort)0x0018)]
        [TestCase(PrimaryOpCode.RST_20, (ushort)0x0020)]
        [TestCase(PrimaryOpCode.RST_28, (ushort)0x0028)]
        [TestCase(PrimaryOpCode.RST_30, (ushort)0x0030)]
        [TestCase(PrimaryOpCode.RST_38, (ushort)0x0038)]
        public void TestReset(PrimaryOpCode opCode, ushort address)
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            Run(3, 11, opCode);

            this.Mmu.Verify(x => x.WriteWord(SP - 2, PC + 1), Times.Once);
            this.Registers.VerifySet(x => x.ProgramCounter = address, Times.Once);
        }

        private void TestCall(PrimaryOpCode opCode, Expression<Func<IFlagsRegister, bool>> propertyLambda, bool flagValue, bool expectJump)
        {
            const ushort Value = 0x6dc4;

            this.SetupRegisters();
            this.ResetMocks();
            this.FlagsRegister.SetupProperty(propertyLambda, flagValue);

            var m = 3;
            var t = 10;
            if (expectJump)
            {
                m += 2;
                t += 7;
            }

            Run(m, t, opCode, Value);

            if (expectJump)
            {
                // Call
                this.Mmu.Verify(x => x.WriteWord(SP - 2, PC + 3), Times.Once);
                this.Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            }
            else
            {
                // No Call
                this.Registers.VerifySet(x => x.ProgramCounter = PC + 3, Times.Once);
                this.Registers.VerifySet(x => x.ProgramCounter = Value, Times.Never);
            }
        }

        private void TestReturn(PrimaryOpCode opCode, Expression<Func<IFlagsRegister, bool>> propertyLambda, bool flagValue, bool expectReturn)
        {
            const ushort Value = 0x6dc4;

            this.SetupRegisters();
            this.ResetMocks();
            this.FlagsRegister.SetupProperty(propertyLambda, flagValue);
            this.Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            var m = 1;
            var t = 5;
            if (expectReturn)
            {
                m += 2;
                t += 6;
            }

            Run(m, t, opCode);

            if (expectReturn)
            {
                this.Mmu.Verify(x => x.ReadWord(SP), Times.Once);
                this.Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
                this.Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);
            }
            else
            {
                // No Call
                this.Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
                this.Mmu.Verify(x => x.ReadWord(It.IsAny<ushort>()), Times.Never);
                this.Registers.VerifySet(x => x.ProgramCounter = PC + 1, Times.Once);
            }
        }
    }
}
