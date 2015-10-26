namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public class DynaRecInstructionBlockDecoder : IInstructionBlockDecoder<IZ80Registers>
    {
        private readonly IMmuFactory mmuFactory;

        private readonly IMmu mmu;

        private readonly CpuMode cpuMode;

        public DynaRecInstructionBlockDecoder(IPlatformConfig platformConfig, IMmuFactory mmuFactory, IMmu mmu)
        {
            this.cpuMode = platformConfig.CpuMode;
            this.mmuFactory = mmuFactory;
            this.mmu = mmu;
        }
        
        public InstructionBlock<IZ80Registers> DecodeNextBlock(ushort address)
        {
            var mmuCache = this.mmuFactory.GetMmuCache(this.mmu, address);
            var timer = new InstructionTimer();
            var expressionBuilder = new Z80BlockBuilder(cpuMode, mmuCache, timer);
            var lambda = expressionBuilder.DecodeNextBlock();

            return new InstructionBlock<IZ80Registers>(address, (ushort)mmuCache.TotalBytesRead, lambda.Compile(), timer.GetInstructionTimings());
        }
        
    }
}
