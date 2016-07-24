using System;
using System.Collections;
using System.Linq;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Core.Decode;
using Axh.Retro.CPU.Z80.Core.DynaRec;
using Axh.Retro.CPU.Z80.Core.Interpreted;
using Axh.Retro.CPU.Z80.Tests.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    public abstract class InstructionBlockDecoderTestsBase
    {
        protected const ushort Address = 0x0000;

        protected const byte A = 0xaa;
        protected const byte B = 0xbb;
        protected const byte C = 0xcc;
        protected const byte D = 0xdd;
        protected const byte E = 0xee;
        protected const byte H = 0x44;
        protected const byte L = 0x77;
        protected const byte F = 0xff;

        protected const ushort BC = 0xbbcc;
        protected const ushort DE = 0xddee;
        protected const ushort HL = 0x4477;
        protected const ushort SP = 0xf000;
        protected const ushort AF = 0xaaff;

        protected const ushort PC = 0x0480;

        protected const byte I = 0xe7;
        protected const byte R = 0xed;

        protected const ushort IX = 0x3366;
        protected const byte IXl = 0x66;
        protected const byte IXh = 0x33;

        protected const ushort IY = 0x2255;
        protected const byte IYl = 0x55;
        protected const byte IYh = 0x22;

        protected const bool IFF1 = false;
        protected const bool IFF2 = true;
        private readonly CpuMode cpuMode;

        protected AccumulatorAndFlagsRegisterSet AfRegisters;

        protected Mock<IAlu> Alu;

        protected Mock<IPrefetchQueue> Cache;

        protected IInstructionBlockDecoder DynaRecBlockDecoder;

        protected Mock<IFlagsRegister> FlagsRegister;

        protected GeneralPurposeRegisterSet GpRegisters;

        protected Mock<IPeripheralManager> Io;

        protected Mock<IMmu> Mmu;

        protected Mock<IRegisters> Registers;

        protected InstructionBlockDecoderTestsBase(CpuMode cpuMode)
        {
            this.cpuMode = cpuMode;
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Registers = new Mock<IRegisters>();
            GpRegisters = new GeneralPurposeRegisterSet();
            FlagsRegister = new Mock<IFlagsRegister>();
            AfRegisters = new AccumulatorAndFlagsRegisterSet(FlagsRegister.Object);
            
            Registers.Setup(x => x.GeneralPurposeRegisters).Returns(GpRegisters);
            Registers.Setup(x => x.AccumulatorAndFlagsRegisters).Returns(AfRegisters);

            Mmu = new Mock<IMmu>();

            Cache = new Mock<IPrefetchQueue>();

            Alu = new Mock<IAlu>();

            Io = new Mock<IPeripheralManager>();

            var platformConfig = new Mock<IPlatformConfig>();
            platformConfig.Setup(x => x.CpuMode).Returns(cpuMode);

            var runtimeConfig = new Mock<IRuntimeConfig>();
            runtimeConfig.Setup(x => x.DebugMode).Returns(true);

            // TODO: we should be testing the opcode decoder separate from the core.
            DynaRecBlockDecoder = new DynaRec(platformConfig.Object,
                                              runtimeConfig.Object,
                                              Cache.Object,
                                              new OpCodeDecoder(platformConfig.Object, Cache.Object));
        }

        protected void SetupRegisters(ushort? bc = null)
        {
            Registers.SetupProperty(x => x.InterruptFlipFlop1, IFF1);
            Registers.SetupProperty(x => x.InterruptFlipFlop2, IFF2);

            Registers.SetupProperty(x => x.ProgramCounter, PC);
            Registers.SetupProperty(x => x.StackPointer, SP);
            
            FlagsRegister.SetupProperty(x => x.Register, F);
            FlagsRegister.SetupProperty(x => x.Sign, false);
            FlagsRegister.SetupProperty(x => x.Zero, false);
            FlagsRegister.SetupProperty(x => x.Flag5, false);
            FlagsRegister.SetupProperty(x => x.HalfCarry, false);
            FlagsRegister.SetupProperty(x => x.ParityOverflow, false);
            FlagsRegister.SetupProperty(x => x.Flag3, false);
            FlagsRegister.SetupProperty(x => x.Subtract, false);
            FlagsRegister.SetupProperty(x => x.Carry, false);

            // Z80 Specific
            Registers.SetupProperty(x => x.I, I);
            Registers.SetupProperty(x => x.R, R);
            Registers.SetupProperty(x => x.IX, IX);
            Registers.SetupProperty(x => x.IY, IY);
            Registers.SetupProperty(x => x.IXh, IXh);
            Registers.SetupProperty(x => x.IXl, IXl);
            Registers.SetupProperty(x => x.IYh, IYh);
            Registers.SetupProperty(x => x.IYl, IYl);

            GpRegisters.BC = bc ?? BC;
            GpRegisters.DE = DE;
            GpRegisters.HL = HL;
            AfRegisters.AF = AF;
        }

        protected void ResetMocks()
        {
            Registers.ResetCalls();
            FlagsRegister.ResetCalls();
            Cache.ResetCalls();
            Mmu.ResetCalls();
            Alu.ResetCalls();
            Io.ResetCalls();
        }

        protected void Run(int expectedMachineCycles, int? expectedThrottlingStates, params object[] bytes)
        {
            var length = 0;
            var queue = new Queue(bytes);
            Cache.Setup(x => x.NextByte()).Returns(() =>
                                                   {
                                                       length++;
                                                       return (byte) queue.Dequeue();
                                                   });

            Cache.Setup(x => x.NextWord()).Returns(() =>
                                                   {
                                                       length += 2;
                                                       return (ushort) queue.Dequeue();
                                                   });

            Cache.Setup(x => x.TotalBytesRead).Returns(() => length);

            var block = DynaRecBlockDecoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            var timings = block.ExecuteInstructionBlock(Registers.Object, Mmu.Object, Alu.Object, Io.Object);

            Assert.AreEqual(expectedMachineCycles, timings.MachineCycles);

            if (expectedThrottlingStates.HasValue)
            {
                Assert.AreEqual(expectedThrottlingStates.Value, timings.ThrottlingStates);
            }

            // Make sure all bytes were read
            Cache.Verify(x => x.NextByte(),
                         Times.Exactly(
                                       bytes.Count(
                                                   x =>
                                                   x is byte || x is PrimaryOpCode || x is PrefixEdOpCode || x is PrefixCbOpCode ||
                                                   x is GameBoyPrimaryOpCode || x is GameBoyPrefixCbOpCode)));
            Cache.Verify(x => x.NextWord(), Times.Exactly(bytes.Count(x => x is ushort)));
        }

        protected void RunWithHalt(int expectedMachineCycles, int? expectedThrottlingStates, params object[] bytes)
        {
            // Add halt
            var opcodes = new object[bytes.Length + 1];
            Array.Copy(bytes, opcodes, bytes.Length);

            opcodes[bytes.Length] = PrimaryOpCode.HALT;
            expectedMachineCycles += 1;
            expectedThrottlingStates += 4;

            Run(expectedMachineCycles, expectedThrottlingStates, opcodes);
        }

        protected byte Get8BitRegisterInitialValue(CpuRegister cpuRegister)
        {
            switch (cpuRegister)
            {
                case CpuRegister.A:
                    return A;
                case CpuRegister.B:
                    return B;
                case CpuRegister.C:
                    return C;
                case CpuRegister.D:
                    return D;
                case CpuRegister.E:
                    return E;
                case CpuRegister.H:
                    return H;
                case CpuRegister.L:
                    return L;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cpuRegister), cpuRegister, null);
            }
        }

        protected byte Get8BitRegisterValue(CpuRegister cpuRegister)
        {
            switch (cpuRegister)
            {
                case CpuRegister.A:
                    return AfRegisters.A;
                case CpuRegister.B:
                    return GpRegisters.B;
                case CpuRegister.C:
                    return GpRegisters.C;
                case CpuRegister.D:
                    return GpRegisters.D;
                case CpuRegister.E:
                    return GpRegisters.E;
                case CpuRegister.H:
                    return GpRegisters.H;
                case CpuRegister.L:
                    return GpRegisters.L;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cpuRegister), cpuRegister, null);
            }
        }
    }
}