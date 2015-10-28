namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class BitTestSetResetTests : InstructionBlockDecoderTestsBase
    {
        [Test]
        public void BIT_n_r()
        {
            const byte ValueAtHL = 0xf8;
            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            var opCodes = GetOpCodes("BIT");
            foreach (var opCode in opCodes)
            {
                this.SetupRegisters();
                this.ResetMocks();

                Console.WriteLine(opCode.OpCode);

                int expectedMCycles, expectedTCycles;
                if (opCode.TargetRegister == TargetRegister.mHL)
                {
                    expectedMCycles = 3;
                    expectedTCycles = 12;
                }
                else
                {
                    expectedMCycles = 2;
                    expectedTCycles = 8;
                }

                RunWithNOP(expectedMCycles, expectedTCycles, PrimaryOpCode.Prefix_CB, opCode.OpCode);

                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        this.Alu.Verify(x => x.BitTest(A, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.B:
                        this.Alu.Verify(x => x.BitTest(B, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.C:
                        this.Alu.Verify(x => x.BitTest(C, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.D:
                        this.Alu.Verify(x => x.BitTest(D, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.E:
                        this.Alu.Verify(x => x.BitTest(E, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.H:
                        this.Alu.Verify(x => x.BitTest(H, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.L:
                        this.Alu.Verify(x => x.BitTest(L, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.mHL:
                        this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
                        this.Alu.Verify(x => x.BitTest(ValueAtHL, opCode.Bit), Times.Once);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void BIT_n_IXYd(PrimaryOpCode prefix)
        {
            
            const sbyte Displacement = -14;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const byte ValueAtIndex = 0x8b;

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            var opCodes = GetOpCodes("BIT");
            foreach (var opCode in opCodes)
            {
                this.SetupRegisters();
                this.ResetMocks();

                Console.WriteLine(opCode.OpCode);

                RunWithNOP(5, 20, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opCode.OpCode);

                this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
                this.Alu.Verify(x => x.BitTest(ValueAtIndex, opCode.Bit), Times.Once);

                // No autocopy on this isntruction
            }
        }

        [Test]
        public void SET_n_r()
        {
            const byte ValueAtHL = 0xe9;
            const byte Expected = 0xf7;

            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);
            this.Alu.Setup(x => x.BitSet(It.IsAny<byte>(), It.IsAny<int>())).Returns(Expected);

            var opCodes = GetOpCodes("SET");
            foreach (var opCode in opCodes)
            {
                this.SetupRegisters();
                this.ResetMocks();

                Console.WriteLine(opCode.OpCode);

                int expectedMCycles, expectedTCycles;
                if (opCode.TargetRegister == TargetRegister.mHL)
                {
                    expectedMCycles = 4;
                    expectedTCycles = 15;
                }
                else
                {
                    expectedMCycles = 2;
                    expectedTCycles = 8;
                }

                RunWithNOP(expectedMCycles, expectedTCycles, PrimaryOpCode.Prefix_CB, opCode.OpCode);

                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        this.Alu.Verify(x => x.BitSet(A, opCode.Bit), Times.Once);
                        this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
                        break;
                    case TargetRegister.B:
                        this.Alu.Verify(x => x.BitSet(B, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.B = Expected, Times.Once);
                        break;
                    case TargetRegister.C:
                        this.Alu.Verify(x => x.BitSet(C, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.C = Expected, Times.Once);
                        break;
                    case TargetRegister.D:
                        this.Alu.Verify(x => x.BitSet(D, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.D = Expected, Times.Once);
                        break;
                    case TargetRegister.E:
                        this.Alu.Verify(x => x.BitSet(E, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.E = Expected, Times.Once);
                        break;
                    case TargetRegister.H:
                        this.Alu.Verify(x => x.BitSet(H, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.H = Expected, Times.Once);
                        break;
                    case TargetRegister.L:
                        this.Alu.Verify(x => x.BitSet(L, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.L = Expected, Times.Once);
                        break;
                    case TargetRegister.mHL:
                        this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
                        this.Alu.Verify(x => x.BitSet(ValueAtHL, opCode.Bit), Times.Once);
                        this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SET_n_IXYd(PrimaryOpCode prefix)
        {
            const sbyte Displacement = -13;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const byte ValueAtIndex = 0x39;
            const byte Expected = 0x42;

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);
            this.Alu.Setup(x => x.BitSet(ValueAtIndex, It.IsAny<int>())).Returns(Expected);

            var opCodes = GetOpCodes("SET");
            foreach (var opCode in opCodes)
            {
                this.SetupRegisters();
                this.ResetMocks();

                Console.WriteLine(opCode.OpCode);

                var expectedMCycles = 6;
                var expectedTCycles = 23;
                
                if (opCode.TargetRegister != TargetRegister.mHL)
                {
                    // Autocopy has extra cycles to run LD_r_IXYd instruction (5, 19) - Prefix_NOP(1, 4) (which has already been run) = (4, 15)
                    expectedMCycles += 4;
                    expectedTCycles += 15;
                }

                RunWithNOP(expectedMCycles, expectedTCycles, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opCode.OpCode);
                
                this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.AtLeastOnce);
                this.Alu.Verify(x => x.BitSet(ValueAtIndex, opCode.Bit), Times.Once);
                this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

                // These instructions have (undocumented) autocopy
                // Note: checking for writing ValueAtIndex not Expected due to mocked out MMU
                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.B:
                        this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.C:
                        this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.D:
                        this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.E:
                        this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.H:
                        this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.L:
                        this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                        break;
                }
            }
        }
        
        [Test]
        public void RES_n_r()
        {
            const byte ValueAtHL = 0xe9;
            const byte Expected = 0xf7;

            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);
            this.Alu.Setup(x => x.BitReset(It.IsAny<byte>(), It.IsAny<int>())).Returns(Expected);

            var opCodes = GetOpCodes("RES_");
            foreach (var opCode in opCodes)
            {
                this.SetupRegisters();
                this.ResetMocks();

                Console.WriteLine(opCode.OpCode);

                int expectedMCycles, expectedTCycles;
                if (opCode.TargetRegister == TargetRegister.mHL)
                {
                    expectedMCycles = 4;
                    expectedTCycles = 15;
                }
                else
                {
                    expectedMCycles = 2;
                    expectedTCycles = 8;
                }

                RunWithNOP(expectedMCycles, expectedTCycles, PrimaryOpCode.Prefix_CB, opCode.OpCode);

                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        this.Alu.Verify(x => x.BitReset(A, opCode.Bit), Times.Once);
                        this.AfRegisters.VerifySet(x => x.A = Expected, Times.Once);
                        break;
                    case TargetRegister.B:
                        this.Alu.Verify(x => x.BitReset(B, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.B = Expected, Times.Once);
                        break;
                    case TargetRegister.C:
                        this.Alu.Verify(x => x.BitReset(C, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.C = Expected, Times.Once);
                        break;
                    case TargetRegister.D:
                        this.Alu.Verify(x => x.BitReset(D, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.D = Expected, Times.Once);
                        break;
                    case TargetRegister.E:
                        this.Alu.Verify(x => x.BitReset(E, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.E = Expected, Times.Once);
                        break;
                    case TargetRegister.H:
                        this.Alu.Verify(x => x.BitReset(H, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.H = Expected, Times.Once);
                        break;
                    case TargetRegister.L:
                        this.Alu.Verify(x => x.BitReset(L, opCode.Bit), Times.Once);
                        this.GpRegisters.VerifySet(x => x.L = Expected, Times.Once);
                        break;
                    case TargetRegister.mHL:
                        this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
                        this.Alu.Verify(x => x.BitReset(ValueAtHL, opCode.Bit), Times.Once);
                        this.Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RES__n_IXYd(PrimaryOpCode prefix)
        {
            const sbyte Displacement = -13;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort)(indexValue + Displacement));
            const byte ValueAtIndex = 0x39;
            const byte Expected = 0x42;

            this.Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);
            this.Alu.Setup(x => x.BitReset(ValueAtIndex, It.IsAny<int>())).Returns(Expected);

            var opCodes = GetOpCodes("RES_");
            foreach (var opCode in opCodes)
            {
                this.SetupRegisters();
                this.ResetMocks();

                Console.WriteLine(opCode.OpCode);

                var expectedMCycles = 6;
                var expectedTCycles = 23;

                if (opCode.TargetRegister != TargetRegister.mHL)
                {
                    // Autocopy has extra cycles to run LD_r_IXYd instruction (5, 19) - Prefix_NOP(1, 4) (which has already been run) = (4, 15)
                    expectedMCycles += 4;
                    expectedTCycles += 15;
                }

                RunWithNOP(expectedMCycles, expectedTCycles, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte)Displacement), opCode.OpCode);

                this.Mmu.Verify(x => x.ReadByte(displacedIndex), Times.AtLeastOnce);
                this.Alu.Verify(x => x.BitReset(ValueAtIndex, opCode.Bit), Times.Once);
                this.Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

                // These instructions have (undocumented) autocopy
                // Note: checking for writing ValueAtIndex not Expected due to mocked out MMU
                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        this.AfRegisters.VerifySet(x => x.A = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.B:
                        this.GpRegisters.VerifySet(x => x.B = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.C:
                        this.GpRegisters.VerifySet(x => x.C = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.D:
                        this.GpRegisters.VerifySet(x => x.D = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.E:
                        this.GpRegisters.VerifySet(x => x.E = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.H:
                        this.GpRegisters.VerifySet(x => x.H = ValueAtIndex, Times.Once);
                        break;
                    case TargetRegister.L:
                        this.GpRegisters.VerifySet(x => x.L = ValueAtIndex, Times.Once);
                        break;
                }
            }
        }
        
        private struct BitOpCode
        {
            public PrefixCbOpCode OpCode { get; set; }

            public int Bit { get; set; }

            public TargetRegister TargetRegister { get; set; }
        }

        private enum TargetRegister
        {
            A,
            B,
            C,
            D,
            E,
            H,
            L,
            mHL
        }

        private static IEnumerable<BitOpCode> GetOpCodes(string method)
        {
            return Enum.GetValues(typeof(PrefixCbOpCode)).Cast<PrefixCbOpCode>().Select(x => new { Name = x.ToString().Split('_'), Value = x }).Where(x => x.Name[0] == method).Select(x => new BitOpCode { OpCode = x.Value, Bit = int.Parse(x.Name[1]), TargetRegister = (TargetRegister)Enum.Parse(typeof(TargetRegister), x.Name[2]) }).ToArray();
        }
    }
}
