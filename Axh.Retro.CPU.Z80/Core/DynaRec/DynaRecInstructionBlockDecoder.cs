namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core.Timing;

    public class DynaRecInstructionBlockDecoder<TRegisters> : IInstructionBlockDecoder<TRegisters> where TRegisters : IRegisters
    {
        private readonly CpuMode cpuMode;

        public DynaRecInstructionBlockDecoder(IPlatformConfig platformConfig)
        {
            this.cpuMode = platformConfig.CpuMode;
        }

        public bool SupportsInstructionBlockCaching => true;

        public IInstructionBlock<TRegisters> DecodeNextBlock(ushort address, IPrefetchQueue prefetchQueue)
        {
            prefetchQueue.ReBuildCache(address);
            var timer = new InstructionTimingsBuilder();
            var expressionBuilder = new DynaRecBlockBuilder<TRegisters>(cpuMode, prefetchQueue, timer);
            var lambda = expressionBuilder.DecodeNextBlock();

            return new DynaRecInstructionBlock<TRegisters>(address, (ushort)prefetchQueue.TotalBytesRead, lambda.Compile(), timer.GetInstructionTimings(), expressionBuilder.LastDecodeResult);
        }
        
    }
}
