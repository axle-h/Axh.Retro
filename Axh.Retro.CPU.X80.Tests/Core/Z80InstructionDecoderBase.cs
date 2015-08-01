using Axh.Retro.CPU.X80.Contracts.Core;
using Axh.Retro.CPU.X80.Contracts.Factories;
using Axh.Retro.CPU.X80.Contracts.Memory;
using Axh.Retro.CPU.X80.Contracts.Registers;
using Axh.Retro.CPU.X80.Core;

namespace Axh.Retro.CPU.X80.Tests.Core
{
    using System.Collections;

    using Retro.CPU.X80.Contracts.Core;
    using Retro.CPU.X80.Contracts.Factories;
    using Retro.CPU.X80.Contracts.Memory;
    using Retro.CPU.X80.Contracts.Registers;
    using Retro.CPU.X80.Core;

    using Moq;

    using NUnit.Framework;

    public abstract class Z80InstructionDecoderBase
    {
        protected const ushort Address = 0x0000;

        protected const byte A = 0xaa;
        protected const byte B = 0xbb;
        protected const byte C = 0xcc;
        protected const byte D = 0xdd;
        protected const byte E = 0xee;
        protected const byte H = 0x44;
        protected const byte L = 0x77;

        protected const ushort HL = 0x4477;

        protected const ushort IX = 0x3366;
        protected const ushort IY = 0x2255;

        protected IZ80InstructionDecoder Decoder;

        protected Mock<IZ80Registers> Registers;

        protected Mock<IMmu> Mmu;

        protected Mock<IGeneralPurposeRegisterSet> GpRegisters;

        protected Mock<IMmuCache> Cache;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.Registers = new Mock<IZ80Registers>();
            this.GpRegisters = new Mock<IGeneralPurposeRegisterSet>();
            this.Registers.Setup(x => x.GeneralPurposeRegisters).Returns(this.GpRegisters.Object);

            this.Mmu = new Mock<IMmu>();

            this.Cache = new Mock<IMmuCache>();

            var mmuFactory = new Mock<IMmuFactory>();
            mmuFactory.Setup(x => x.GetMmuCache(this.Mmu.Object, Address)).Returns(this.Cache.Object);

            this.Decoder = new Z80InstructionDecoder(mmuFactory.Object, this.Mmu.Object);
        }

        protected void SetupRegisters()
        {
            this.GpRegisters.SetupProperty(x => x.A, A);
            this.GpRegisters.SetupProperty(x => x.B, B);
            this.GpRegisters.SetupProperty(x => x.C, C);
            this.GpRegisters.SetupProperty(x => x.D, D);
            this.GpRegisters.SetupProperty(x => x.E, E);
            this.GpRegisters.SetupProperty(x => x.H, H);
            this.GpRegisters.SetupProperty(x => x.L, L);

            this.GpRegisters.SetupProperty(x => x.HL, HL);

            this.Registers.SetupProperty(x => x.IX, IX);
            this.Registers.SetupProperty(x => x.IY, IY);
        }

        protected void ResetMocks()
        {
            this.Registers.ResetCalls();
            this.Cache.ResetCalls();
            this.Mmu.ResetCalls();
            this.GpRegisters.ResetCalls();
        }

        protected void SetCacheForSingleBytes(params object[] bytes)
        {
            var length = 0;
            var queue = new Queue(bytes);
            this.Cache.Setup(x => x.NextByte()).Returns(
                () =>
                {
                    length++;
                    return (byte)queue.Dequeue();
                });

            this.Cache.Setup(x => x.TotalBytesRead).Returns(() => length);
        }
    }
}
