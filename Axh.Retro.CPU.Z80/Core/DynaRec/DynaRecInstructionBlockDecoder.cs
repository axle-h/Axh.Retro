namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Debug;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core.Debug;
    using Axh.Retro.CPU.Z80.Core.Timing;

    public class DynaRecInstructionBlockDecoder<TRegisters> : IInstructionBlockDecoder<TRegisters> where TRegisters : IRegisters
    {
        private readonly IPrefetchQueue prefetchQueue;

        private readonly DynaRecBlockBuilder<TRegisters> blockBuilder;

        private readonly DebugBuilder<TRegisters> debugBuilder;

        private readonly InstructionTimingsBuilder instructionTimingsBuilder;

        public DynaRecInstructionBlockDecoder(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPrefetchQueue prefetchQueue)
        {
            this.prefetchQueue = prefetchQueue;

            this.instructionTimingsBuilder = new InstructionTimingsBuilder();
            this.blockBuilder = new DynaRecBlockBuilder<TRegisters>(platformConfig.CpuMode, prefetchQueue, instructionTimingsBuilder);

            if (runtimeConfig.DebugMode)
            {
                this.debugBuilder = new DebugBuilder<TRegisters>(platformConfig.CpuMode, prefetchQueue);
            }
        }

        public bool SupportsInstructionBlockCaching => true;

        public IInstructionBlock<TRegisters> DecodeNextBlock(ushort address)
        {
            prefetchQueue.ReBuildCache(address);
            var lambda = blockBuilder.DecodeNextBlock();

            var debugInfo = this.debugBuilder?.GetDebugInfo(address);

            return new DynaRecInstructionBlock<TRegisters>(address, (ushort)prefetchQueue.TotalBytesRead, lambda.Compile(), instructionTimingsBuilder.GetInstructionTimings(), blockBuilder.LastDecodeResult, debugInfo);
        }
        
    }
}
