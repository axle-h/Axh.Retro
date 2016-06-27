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
    public class CallReturnResetTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public CallReturnResetTests() : base(CpuMode.Z80)
        {
        }

        [TestCase(PrimaryOpCode.RST_00, (ushort) 0x0000)]
        [TestCase(PrimaryOpCode.RST_08, (ushort) 0x0008)]
        [TestCase(PrimaryOpCode.RST_10, (ushort) 0x0010)]
        [TestCase(PrimaryOpCode.RST_18, (ushort) 0x0018)]
        [TestCase(PrimaryOpCode.RST_20, (ushort) 0x0020)]
        [TestCase(PrimaryOpCode.RST_28, (ushort) 0x0028)]
        [TestCase(PrimaryOpCode.RST_30, (ushort) 0x0030)]
        [TestCase(PrimaryOpCode.RST_38, (ushort) 0x0038)]
        public void TestReset(PrimaryOpCode opCode, ushort address)
        {
            SetupRegisters();
            ResetMocks();

            Run(3, 11, opCode);

            Mmu.Verify(x => x.WriteWord(SP - 2, PC + 1), Times.Once);
            Registers.VerifySet(x => x.ProgramCounter = address, Times.Once);
        }

        private void TestCall(PrimaryOpCode opCode,
                              Expression<Func<IFlagsRegister, bool>> propertyLambda,
                              bool flagValue,
                              bool expectJump)
        {
            const ushort Value = 0x6dc4;

            SetupRegisters();
            ResetMocks();
            FlagsRegister.SetupProperty(propertyLambda, flagValue);

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
                Mmu.Verify(x => x.WriteWord(SP - 2, PC + 3), Times.Once);
                Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            }
            else
            {
                // No Call
                Registers.VerifySet(x => x.ProgramCounter = PC + 3, Times.Once);
                Registers.VerifySet(x => x.ProgramCounter = Value, Times.Never);
            }
        }

        private void TestReturn(PrimaryOpCode opCode,
                                Expression<Func<IFlagsRegister, bool>> propertyLambda,
                                bool flagValue,
                                bool expectReturn)
        {
            const ushort Value = 0x6dc4;

            SetupRegisters();
            ResetMocks();
            FlagsRegister.SetupProperty(propertyLambda, flagValue);
            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

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
                Mmu.Verify(x => x.ReadWord(SP), Times.Once);
                Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
                Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);
            }
            else
            {
                // No Call
                Registers.VerifySet(x => x.StackPointer = It.IsAny<ushort>(), Times.Never);
                Mmu.Verify(x => x.ReadWord(It.IsAny<ushort>()), Times.Never);
                Registers.VerifySet(x => x.ProgramCounter = PC + 1, Times.Once);
            }
        }

        [Test]
        public void CALL()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x6238;

            Run(5, 17, PrimaryOpCode.CALL, Value);

            Mmu.Verify(x => x.WriteWord(SP - 2, PC + 3), Times.Once);
            Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
        }

        [Test]
        public void CALL_C()
        {
            TestCall(PrimaryOpCode.CALL_C, x => x.Carry, true, true);
            TestCall(PrimaryOpCode.CALL_C, x => x.Carry, false, false);
        }

        [Test]
        public void CALL_M()
        {
            TestCall(PrimaryOpCode.CALL_M, x => x.Sign, true, true);
            TestCall(PrimaryOpCode.CALL_M, x => x.Sign, false, false);
        }

        [Test]
        public void CALL_NC()
        {
            TestCall(PrimaryOpCode.CALL_NC, x => x.Carry, true, false);
            TestCall(PrimaryOpCode.CALL_NC, x => x.Carry, false, true);
        }

        [Test]
        public void CALL_NZ()
        {
            TestCall(PrimaryOpCode.CALL_NZ, x => x.Zero, true, false);
            TestCall(PrimaryOpCode.CALL_NZ, x => x.Zero, false, true);
        }

        [Test]
        public void CALL_P()
        {
            TestCall(PrimaryOpCode.CALL_P, x => x.Sign, true, false);
            TestCall(PrimaryOpCode.CALL_P, x => x.Sign, false, true);
        }

        [Test]
        public void CALL_PE()
        {
            TestCall(PrimaryOpCode.CALL_PE, x => x.ParityOverflow, true, true);
            TestCall(PrimaryOpCode.CALL_PE, x => x.ParityOverflow, false, false);
        }

        [Test]
        public void CALL_PO()
        {
            TestCall(PrimaryOpCode.CALL_PO, x => x.ParityOverflow, true, false);
            TestCall(PrimaryOpCode.CALL_PO, x => x.ParityOverflow, false, true);
        }

        [Test]
        public void CALL_Z()
        {
            TestCall(PrimaryOpCode.CALL_Z, x => x.Zero, true, true);
            TestCall(PrimaryOpCode.CALL_Z, x => x.Zero, false, false);
        }

        [Test]
        public void RET()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0xd466;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            Run(3, 10, PrimaryOpCode.RET);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);
        }

        [Test]
        public void RET_C()
        {
            TestReturn(PrimaryOpCode.RET_C, x => x.Carry, true, true);
            TestReturn(PrimaryOpCode.RET_C, x => x.Carry, false, false);
        }

        [Test]
        public void RET_M()
        {
            TestReturn(PrimaryOpCode.RET_M, x => x.Sign, true, true);
            TestReturn(PrimaryOpCode.RET_M, x => x.Sign, false, false);
        }

        [Test]
        public void RET_NC()
        {
            TestReturn(PrimaryOpCode.RET_NC, x => x.Carry, true, false);
            TestReturn(PrimaryOpCode.RET_NC, x => x.Carry, false, true);
        }


        [Test]
        public void RET_NZ()
        {
            TestReturn(PrimaryOpCode.RET_NZ, x => x.Zero, true, false);
            TestReturn(PrimaryOpCode.RET_NZ, x => x.Zero, false, true);
        }

        [Test]
        public void RET_P()
        {
            TestReturn(PrimaryOpCode.RET_P, x => x.Sign, true, false);
            TestReturn(PrimaryOpCode.RET_P, x => x.Sign, false, true);
        }

        [Test]
        public void RET_PE()
        {
            TestReturn(PrimaryOpCode.RET_PE, x => x.ParityOverflow, true, true);
            TestReturn(PrimaryOpCode.RET_PE, x => x.ParityOverflow, false, false);
        }

        [Test]
        public void RET_PO()
        {
            TestReturn(PrimaryOpCode.RET_PO, x => x.ParityOverflow, true, false);
            TestReturn(PrimaryOpCode.RET_PO, x => x.ParityOverflow, false, true);
        }

        [Test]
        public void RET_Z()
        {
            TestReturn(PrimaryOpCode.RET_Z, x => x.Zero, true, true);
            TestReturn(PrimaryOpCode.RET_Z, x => x.Zero, false, false);
        }

        [Test]
        public void RETI()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0xd466;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            Run(4, 14, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.RETI);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);
        }

        [Test]
        public void RETN()
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0xd466;

            Mmu.Setup(x => x.ReadWord(SP)).Returns(Value);

            Run(4, 14, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.RETN);

            Mmu.Verify(x => x.ReadWord(SP), Times.Once);
            Registers.VerifySet(x => x.ProgramCounter = Value, Times.Once);
            Registers.VerifySet(x => x.StackPointer = SP + 2, Times.Once);

            // RETN The state of IFF2 is copied back to IFF1
            Registers.VerifySet(x => x.InterruptFlipFlop1 = IFF2, Times.Once);
        }
    }
}