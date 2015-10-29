namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.IO;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public class DynaRecInstructionBlockDecoder<TRegisters> : IInstructionBlockDecoder<TRegisters> where TRegisters : IRegisters
    {
        private readonly IMmuFactory mmuFactory;
        
        private readonly CpuMode cpuMode;

        public DynaRecInstructionBlockDecoder(IPlatformConfig platformConfig, IMmuFactory mmuFactory)
        {
            this.cpuMode = platformConfig.CpuMode;
            this.mmuFactory = mmuFactory;
        }

        public bool SupportsInstructionBlockCaching => true;

        public IInstructionBlock<TRegisters> DecodeNextBlock(ushort address, IMmu mmu)
        {
            var mmuCache = this.mmuFactory.GetMmuCache(mmu, address);
            var timer = new InstructionTimer();
            var expressionBuilder = new DynaRecBlockBuilder<TRegisters>(cpuMode, mmuCache, timer);
            var lambda = expressionBuilder.DecodeNextBlock();

            return new DynaRecInstructionBlock<TRegisters>(address, (ushort)mmuCache.TotalBytesRead, lambda.Compile(), timer.GetInstructionTimings());
        }
        
    }
}
