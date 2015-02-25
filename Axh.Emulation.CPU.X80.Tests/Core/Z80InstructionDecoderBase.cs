namespace Axh.Emulation.CPU.X80.Tests.Core
{
    using Axh.Emulation.CPU.X80.Contracts.Core;
    using Axh.Emulation.CPU.X80.Contracts.Factories;
    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Contracts.Registers;
    using Axh.Emulation.CPU.X80.Core;

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

        protected IZ80InstructionDecoder decoder;

        protected Mock<IZ80Registers> registers;

        protected Mock<IMmu> mmu;

        protected Mock<IGeneralPurposeRegisterSet> gpRegisters;

        protected Mock<IMmuCache> cache;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.registers = new Mock<IZ80Registers>();
            this.gpRegisters = new Mock<IGeneralPurposeRegisterSet>();
            this.registers.Setup(x => x.GeneralPurposeRegisters).Returns(this.gpRegisters.Object);

            this.mmu = new Mock<IMmu>();

            this.cache = new Mock<IMmuCache>();

            var mmuFactory = new Mock<IMmuFactory>();
            mmuFactory.Setup(x => x.GetMmuCache(this.mmu.Object, Address)).Returns(this.cache.Object);

            this.decoder = new Z80InstructionDecoder(mmuFactory.Object, this.mmu.Object);
        }

        protected void SetupRegisters()
        {
            this.gpRegisters.SetupProperty(x => x.A, A);
            this.gpRegisters.SetupProperty(x => x.B, B);
            this.gpRegisters.SetupProperty(x => x.C, C);
            this.gpRegisters.SetupProperty(x => x.D, D);
            this.gpRegisters.SetupProperty(x => x.E, E);
            this.gpRegisters.SetupProperty(x => x.H, H);
            this.gpRegisters.SetupProperty(x => x.L, L);

            this.gpRegisters.SetupProperty(x => x.HL, HL);
        }

        protected void ResetMocks()
        {
            this.registers.ResetCalls();
            this.cache.ResetCalls();
            this.mmu.ResetCalls();
            this.gpRegisters.ResetCalls();
        }
    }
}
