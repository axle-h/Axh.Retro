namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using System;
    using System.Collections;
    using System.Linq;

    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Core;
    using Axh.Retro.CPU.X80.Core.DynaRec;

    using Moq;

    using NUnit.Framework;

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
        protected const byte F = 0x00;

        protected const ushort BC = 0xbbcc;
        protected const ushort DE = 0xddee;
        protected const ushort HL = 0x4477;
        protected const ushort SP = 0xf000;

        protected const byte I = 0xe7;
        protected const byte R = 0xed;

        protected const ushort IX = 0x3366;
        protected const byte IXl = 0x66;
        protected const byte IXh = 0x33;

        protected const ushort IY = 0x2255;
        protected const byte IYl = 0x55;
        protected const byte IYh = 0x22;

        protected IInstructionBlockDecoder<IZ80Registers> DynaRecBlockDecoder;

        protected Mock<IZ80Registers> Registers;

        protected Mock<IMmu> Mmu;

        protected Mock<IArithmeticLogicUnit> Alu;

        protected Mock<IGeneralPurposeRegisterSet> GpRegisters;

        protected Mock<IAccumulatorAndFlagsRegisterSet> AfRegisters;

        protected Mock<IFlagsRegister> FlagsRegister;

        protected Mock<IMmuCache> Cache;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.Registers = new Mock<IZ80Registers>();
            this.GpRegisters = new Mock<IGeneralPurposeRegisterSet>();
            this.AfRegisters = new Mock<IAccumulatorAndFlagsRegisterSet>();
            this.FlagsRegister = new Mock<IFlagsRegister>();

            this.AfRegisters.Setup(x => x.Flags).Returns(this.FlagsRegister.Object);
            this.Registers.Setup(x => x.GeneralPurposeRegisters).Returns(this.GpRegisters.Object);
            this.Registers.Setup(x => x.AccumulatorAndFlagsRegisters).Returns(this.AfRegisters.Object);

            this.Mmu = new Mock<IMmu>();

            this.Cache = new Mock<IMmuCache>();
            
            var mmuFactory = new Mock<IMmuFactory>();
            mmuFactory.Setup(x => x.GetMmuCache(this.Mmu.Object, Address)).Returns(this.Cache.Object);

            this.Alu = new Mock<IArithmeticLogicUnit>();

            var platformConfig = new Mock<IPlatformConfig>();
            platformConfig.Setup(x => x.CpuMode).Returns(CpuMode.Z80);

            this.DynaRecBlockDecoder = new DynaRecInstructionBlockDecoder<IZ80Registers>(platformConfig.Object, mmuFactory.Object);
        }

        protected void SetupRegisters(ushort? bc = null)
        {
            this.GpRegisters.SetupProperty(x => x.B, B);
            this.GpRegisters.SetupProperty(x => x.C, C);
            this.GpRegisters.SetupProperty(x => x.D, D);
            this.GpRegisters.SetupProperty(x => x.E, E);
            this.GpRegisters.SetupProperty(x => x.H, H);
            this.GpRegisters.SetupProperty(x => x.L, L);

            this.GpRegisters.SetupProperty(x => x.BC, bc ?? BC);
            this.GpRegisters.SetupProperty(x => x.DE, DE);

            this.GpRegisters.SetupProperty(x => x.HL, HL);

            this.Registers.SetupProperty(x => x.I, I);
            this.Registers.SetupProperty(x => x.R, R);

            this.Registers.SetupProperty(x => x.IX, IX);
            this.Registers.SetupProperty(x => x.IY, IY);

            this.Registers.SetupProperty(x => x.IXh, IXh);
            this.Registers.SetupProperty(x => x.IXl, IXl);
            this.Registers.SetupProperty(x => x.IYh, IYh);
            this.Registers.SetupProperty(x => x.IYl, IYl);
            
            this.Registers.SetupProperty(x => x.StackPointer, SP);

            this.AfRegisters.SetupProperty(x => x.A, A);
            
            this.FlagsRegister.SetupProperty(x => x.Register, F);
            this.FlagsRegister.SetupProperty(x => x.Sign, false);
            this.FlagsRegister.SetupProperty(x => x.Zero, false);
            this.FlagsRegister.SetupProperty(x => x.Flag5, false);
            this.FlagsRegister.SetupProperty(x => x.HalfCarry, false);
            this.FlagsRegister.SetupProperty(x => x.ParityOverflow, false);
            this.FlagsRegister.SetupProperty(x => x.Flag3, false);
            this.FlagsRegister.SetupProperty(x => x.Subtract, false);
            this.FlagsRegister.SetupProperty(x => x.Carry, false);
        }

        protected void ResetMocks()
        {
            this.Registers.ResetCalls();
            this.GpRegisters.ResetCalls();
            this.AfRegisters.ResetCalls();
            this.FlagsRegister.ResetCalls();
            this.Cache.ResetCalls();
            this.Mmu.ResetCalls();
            this.Alu.ResetCalls();
        }

        protected void Run(int expectedMachineCycles, int expectedThrottlingStates, params object[] bytes)
        {
            var length = 0;
            var queue = new Queue(bytes);
            this.Cache.Setup(x => x.NextByte()).Returns(
                () =>
                {
                    length++;
                    return (byte)queue.Dequeue();
                });

            this.Cache.Setup(x => x.NextWord()).Returns(
                () =>
                {
                    length += 2;
                    return (ushort)queue.Dequeue();
                });

            this.Cache.Setup(x => x.TotalBytesRead).Returns(() => length);

            var block = this.DynaRecBlockDecoder.DecodeNextBlock(this.Mmu.Object, Address);
            Assert.IsNotNull(block);

            var timings = block.ExecuteInstructionBlock(this.Registers.Object, this.Mmu.Object, this.Alu.Object);

            Assert.AreEqual(expectedMachineCycles, timings.MachineCycles);
            Assert.AreEqual(expectedThrottlingStates, timings.ThrottlingStates);

            // Make sure all bytes were read
            this.Cache.Verify(x => x.NextByte(), Times.Exactly(bytes.Count(x => x is byte || x is PrimaryOpCode || x is PrefixEdOpCode || x is PrefixCbOpCode)));
            this.Cache.Verify(x => x.NextWord(), Times.Exactly(bytes.Count(x => x is ushort)));
        }
        
        protected void RunWithNOP(int expectedMachineCycles, int expectedThrottlingStates, params object[] bytes)
        {
            // Add halt
            var opcodes = new object[bytes.Length + 1];
            Array.Copy(bytes, opcodes, bytes.Length);

            opcodes[bytes.Length] = PrimaryOpCode.HALT;
            expectedMachineCycles += 1;
            expectedThrottlingStates += 4;

            Run(expectedMachineCycles, expectedThrottlingStates, opcodes);
        }
    }
}
