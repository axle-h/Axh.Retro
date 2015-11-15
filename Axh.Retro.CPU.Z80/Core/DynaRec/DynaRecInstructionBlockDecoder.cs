namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core.Decode;
    using Axh.Retro.CPU.Z80.Core.Timing;

    public class DynaRecInstructionBlockDecoder<TRegisters> : IInstructionBlockDecoder<TRegisters> where TRegisters : IRegisters
    {
        private readonly IPrefetchQueue prefetchQueue;

        private readonly OpCodeDecoder decoder;

        private readonly DynaRec<TRegisters> dynarec;

        private readonly InstructionTimingsBuilder timer;

        public DynaRecInstructionBlockDecoder(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPrefetchQueue prefetchQueue)
        {
            this.prefetchQueue = prefetchQueue;

            this.timer = new InstructionTimingsBuilder();
            //this.blockBuilder = new DynaRecBlockBuilder<TRegisters>(platformConfig.CpuMode, prefetchQueue, instructionTimingsBuilder);

            this.decoder = new OpCodeDecoder(platformConfig, prefetchQueue, timer);
            this.dynarec = new DynaRec<TRegisters>(platformConfig, prefetchQueue);

            if (runtimeConfig.DebugMode)
            {
                // TODO: Setup debug
            }
        }

        public bool SupportsInstructionBlockCaching => true;

        public IInstructionBlock<TRegisters> DecodeNextBlock(ushort address)
        {
            prefetchQueue.ReBuildCache(address);

            var operations = this.decoder.DecodeNextBlock();
            var lambda = dynarec.BuildExpressionTree(operations);

            return new DynaRecInstructionBlock<TRegisters>(address, (ushort)prefetchQueue.TotalBytesRead, lambda.Compile(), timer.GetInstructionTimings(), dynarec.LastDecodeResult);
        }
        
    }
}
