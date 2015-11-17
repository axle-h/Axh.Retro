namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System.Linq;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core.Decode;
    using Axh.Retro.CPU.Z80.Core.Timing;
    using Axh.Retro.CPU.Z80.Util;

    public class DynaRecInstructionBlockDecoder<TRegisters> : IInstructionBlockDecoder<TRegisters> where TRegisters : IRegisters
    {
        private readonly IPrefetchQueue prefetchQueue;

        private readonly OpCodeDecoder decoder;

        private readonly DynaRec<TRegisters> dynarec;

        private readonly InstructionTimingsBuilder timer;

        private readonly bool debugInfo;

        public DynaRecInstructionBlockDecoder(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPrefetchQueue prefetchQueue)
        {
            this.prefetchQueue = prefetchQueue;

            this.timer = new InstructionTimingsBuilder();

            this.decoder = new OpCodeDecoder(platformConfig, prefetchQueue, timer);
            this.dynarec = new DynaRec<TRegisters>(platformConfig, prefetchQueue);

            this.debugInfo = runtimeConfig.DebugMode;
        }

        public bool SupportsInstructionBlockCaching => true;

        public IInstructionBlock<TRegisters> DecodeNextBlock(ushort address)
        {
            var operations = this.decoder.DecodeNextBlock(address).ToArray();
            var lambda = dynarec.BuildExpressionTree(operations);
            
            var block = new DynaRecInstructionBlock<TRegisters>(address, (ushort)prefetchQueue.TotalBytesRead, lambda.Compile(), timer.GetInstructionTimings(), dynarec.LastDecodeResult);
            if (this.debugInfo)
            {
                block.DebugInfo = $"{string.Join("\n", operations.Select(x => x.ToString()))}\n\n{lambda.DebugView()}";
            }

            return block;
        }
        
    }
}
