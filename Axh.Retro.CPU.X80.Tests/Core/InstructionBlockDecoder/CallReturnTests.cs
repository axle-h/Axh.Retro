namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using System;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CallReturnTests: InstructionBlockDecoderTestsBase
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
    }
}
