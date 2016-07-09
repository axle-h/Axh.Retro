using System;
using System.Collections.Generic;
using System.Linq;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class BitTestSetResetTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public BitTestSetResetTests() : base(CpuMode.Z80)
        {
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void BIT_n_IXYd(PrimaryOpCode prefix)
        {
            const sbyte Displacement = -14;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const byte ValueAtIndex = 0x8b;

            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);

            var opCodes = GetOpCodes("BIT");
            foreach (var opCode in opCodes)
            {
                SetupRegisters();
                ResetMocks();

                Console.WriteLine(opCode.OpCode);

                RunWithHalt(5, 20, prefix, PrimaryOpCode.Prefix_CB, unchecked((byte) Displacement), opCode.OpCode);

                Mmu.Verify(x => x.ReadByte(displacedIndex), Times.Once);
                Alu.Verify(x => x.BitTest(ValueAtIndex, opCode.Bit), Times.Once);

                // No autocopy on this isntruction
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void SET_n_IXYd(PrimaryOpCode prefix)
        {
            const sbyte Displacement = -13;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const byte ValueAtIndex = 0x39;
            const byte Expected = 0x42;

            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);
            Alu.Setup(x => x.BitSet(ValueAtIndex, It.IsAny<int>())).Returns(Expected);

            var opCodes = GetOpCodes("SET");
            foreach (var opCode in opCodes)
            {
                SetupRegisters();
                ResetMocks();

                Console.WriteLine(opCode.OpCode);

                var expectedMCycles = 6;
                var expectedTCycles = 23;

                if (opCode.TargetRegister != TargetRegister.mHL)
                {
                    // Autocopy has extra cycles to run LD_r_IXYd instruction (5, 19) - Prefix_NOP(1, 4) (which has already been run) = (4, 15)
                    expectedMCycles += 4;
                    expectedTCycles += 15;
                }

                RunWithHalt(expectedMCycles,
                            expectedTCycles,
                            prefix,
                            PrimaryOpCode.Prefix_CB,
                            unchecked((byte) Displacement),
                            opCode.OpCode);

                Mmu.Verify(x => x.ReadByte(displacedIndex), Times.AtLeastOnce);
                Alu.Verify(x => x.BitSet(ValueAtIndex, opCode.Bit), Times.Once);
                Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

                // These instructions have (undocumented) autocopy
                // Note: checking for writing ValueAtIndex not Expected due to mocked out MMU
                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        Assert.AreEqual(AfRegisters.A, ValueAtIndex);
                        break;
                    case TargetRegister.B:
                        Assert.AreEqual(GpRegisters.B, ValueAtIndex);
                        break;
                    case TargetRegister.C:
                        Assert.AreEqual(GpRegisters.C, ValueAtIndex);
                        break;
                    case TargetRegister.D:
                        Assert.AreEqual(GpRegisters.D, ValueAtIndex);
                        break;
                    case TargetRegister.E:
                        Assert.AreEqual(GpRegisters.E, ValueAtIndex);
                        break;
                    case TargetRegister.H:
                        Assert.AreEqual(GpRegisters.H, ValueAtIndex);
                        break;
                    case TargetRegister.L:
                        Assert.AreEqual(GpRegisters.L, ValueAtIndex);
                        break;
                }
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void RES_n_IXYd(PrimaryOpCode prefix)
        {
            const sbyte Displacement = -13;
            var indexValue = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            var displacedIndex = unchecked((ushort) (indexValue + Displacement));
            const byte ValueAtIndex = 0x39;
            const byte Expected = 0x42;

            Mmu.Setup(x => x.ReadByte(displacedIndex)).Returns(ValueAtIndex);
            Alu.Setup(x => x.BitReset(ValueAtIndex, It.IsAny<int>())).Returns(Expected);

            var opCodes = GetOpCodes("RES");
            foreach (var opCode in opCodes)
            {
                SetupRegisters();
                ResetMocks();

                Console.WriteLine(opCode.OpCode);

                var expectedMCycles = 6;
                var expectedTCycles = 23;

                if (opCode.TargetRegister != TargetRegister.mHL)
                {
                    // Autocopy has extra cycles to run LD_r_IXYd instruction (5, 19) - Prefix_NOP(1, 4) (which has already been run) = (4, 15)
                    expectedMCycles += 4;
                    expectedTCycles += 15;
                }

                RunWithHalt(expectedMCycles,
                            expectedTCycles,
                            prefix,
                            PrimaryOpCode.Prefix_CB,
                            unchecked((byte) Displacement),
                            opCode.OpCode);

                Mmu.Verify(x => x.ReadByte(displacedIndex), Times.AtLeastOnce);
                Alu.Verify(x => x.BitReset(ValueAtIndex, opCode.Bit), Times.Once);
                Mmu.Verify(x => x.WriteByte(displacedIndex, Expected), Times.Once);

                // These instructions have (undocumented) autocopy
                // Note: checking for writing ValueAtIndex not Expected due to mocked out MMU
                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        Assert.AreEqual(AfRegisters.A, ValueAtIndex);
                        break;
                    case TargetRegister.B:
                        Assert.AreEqual(GpRegisters.B, ValueAtIndex);
                        break;
                    case TargetRegister.C:
                        Assert.AreEqual(GpRegisters.C, ValueAtIndex);
                        break;
                    case TargetRegister.D:
                        Assert.AreEqual(GpRegisters.D, ValueAtIndex);
                        break;
                    case TargetRegister.E:
                        Assert.AreEqual(GpRegisters.E, ValueAtIndex);
                        break;
                    case TargetRegister.H:
                        Assert.AreEqual(GpRegisters.H, ValueAtIndex);
                        break;
                    case TargetRegister.L:
                        Assert.AreEqual(GpRegisters.L, ValueAtIndex);
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
            return
                Enum.GetValues(typeof (PrefixCbOpCode))
                    .Cast<PrefixCbOpCode>()
                    .Select(x => new { Name = x.ToString().Split('_'), Value = x })
                    .Where(x => x.Name[0] == method)
                    .Select(
                            x =>
                            new BitOpCode
                            {
                                OpCode = x.Value,
                                Bit = int.Parse(x.Name[1]),
                                TargetRegister = (TargetRegister) Enum.Parse(typeof (TargetRegister), x.Name[2])
                            })
                    .ToArray();
        }

        [Test]
        public void BIT_n_r()
        {
            const byte ValueAtHL = 0xf8;
            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);

            var opCodes = GetOpCodes("BIT");
            foreach (var opCode in opCodes)
            {
                SetupRegisters();
                ResetMocks();

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

                RunWithHalt(expectedMCycles, expectedTCycles, PrimaryOpCode.Prefix_CB, opCode.OpCode);

                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        Alu.Verify(x => x.BitTest(A, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.B:
                        Alu.Verify(x => x.BitTest(B, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.C:
                        Alu.Verify(x => x.BitTest(C, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.D:
                        Alu.Verify(x => x.BitTest(D, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.E:
                        Alu.Verify(x => x.BitTest(E, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.H:
                        Alu.Verify(x => x.BitTest(H, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.L:
                        Alu.Verify(x => x.BitTest(L, opCode.Bit), Times.Once);
                        break;
                    case TargetRegister.mHL:
                        Mmu.Verify(x => x.ReadByte(HL), Times.Once);
                        Alu.Verify(x => x.BitTest(ValueAtHL, opCode.Bit), Times.Once);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Test]
        public void RES_n_r()
        {
            const byte ValueAtHL = 0xe9;
            const byte Expected = 0xf7;

            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);
            Alu.Setup(x => x.BitReset(It.IsAny<byte>(), It.IsAny<int>())).Returns(Expected);

            var opCodes = GetOpCodes("RES_");
            foreach (var opCode in opCodes)
            {
                SetupRegisters();
                ResetMocks();

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

                RunWithHalt(expectedMCycles, expectedTCycles, PrimaryOpCode.Prefix_CB, opCode.OpCode);

                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        Alu.Verify(x => x.BitReset(A, opCode.Bit), Times.Once);
                        Assert.AreEqual(AfRegisters.A, Expected);
                        break;
                    case TargetRegister.B:
                        Alu.Verify(x => x.BitReset(B, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.B, Expected);
                        break;
                    case TargetRegister.C:
                        Alu.Verify(x => x.BitReset(C, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.C, Expected);
                        break;
                    case TargetRegister.D:
                        Alu.Verify(x => x.BitReset(D, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.D, Expected);
                        break;
                    case TargetRegister.E:
                        Alu.Verify(x => x.BitReset(E, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.E, Expected);
                        break;
                    case TargetRegister.H:
                        Alu.Verify(x => x.BitReset(H, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.H, Expected);
                        break;
                    case TargetRegister.L:
                        Alu.Verify(x => x.BitReset(L, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.L, Expected);
                        break;
                    case TargetRegister.mHL:
                        Mmu.Verify(x => x.ReadByte(HL), Times.Once);
                        Alu.Verify(x => x.BitReset(ValueAtHL, opCode.Bit), Times.Once);
                        Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Test]
        public void SET_n_r()
        {
            const byte ValueAtHL = 0xe9;
            const byte Expected = 0xf7;

            Mmu.Setup(x => x.ReadByte(HL)).Returns(ValueAtHL);
            Alu.Setup(x => x.BitSet(It.IsAny<byte>(), It.IsAny<int>())).Returns(Expected);

            var opCodes = GetOpCodes("SET");
            foreach (var opCode in opCodes)
            {
                SetupRegisters();
                ResetMocks();

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

                RunWithHalt(expectedMCycles, expectedTCycles, PrimaryOpCode.Prefix_CB, opCode.OpCode);

                switch (opCode.TargetRegister)
                {
                    case TargetRegister.A:
                        Alu.Verify(x => x.BitSet(A, opCode.Bit), Times.Once);
                        Assert.AreEqual(AfRegisters.A, Expected);
                        break;
                    case TargetRegister.B:
                        Alu.Verify(x => x.BitSet(B, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.B, Expected);
                        break;
                    case TargetRegister.C:
                        Alu.Verify(x => x.BitSet(C, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.C, Expected);
                        break;
                    case TargetRegister.D:
                        Alu.Verify(x => x.BitSet(D, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.D, Expected);
                        break;
                    case TargetRegister.E:
                        Alu.Verify(x => x.BitSet(E, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.E, Expected);
                        break;
                    case TargetRegister.H:
                        Alu.Verify(x => x.BitSet(H, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.H, Expected);
                        break;
                    case TargetRegister.L:
                        Alu.Verify(x => x.BitSet(L, opCode.Bit), Times.Once);
                        Assert.AreEqual(GpRegisters.L, Expected);
                        break;
                    case TargetRegister.mHL:
                        Mmu.Verify(x => x.ReadByte(HL), Times.Once);
                        Alu.Verify(x => x.BitSet(ValueAtHL, opCode.Bit), Times.Once);
                        Mmu.Verify(x => x.WriteByte(HL, Expected), Times.Once);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}