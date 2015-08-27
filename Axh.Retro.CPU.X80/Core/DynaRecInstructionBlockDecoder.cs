namespace Axh.Retro.CPU.X80.Core
{
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public class DynaRecInstructionBlockDecoder : IInstructionBlockDecoder<IZ80Registers>
    {
        private readonly IMmuFactory mmuFactory;

        private readonly IMmu mmu;

        public DynaRecInstructionBlockDecoder(IMmuFactory mmuFactory, IMmu mmu)
        {
            this.mmuFactory = mmuFactory;
            this.mmu = mmu;
        }
        
        public InstructionBlock<IZ80Registers> DecodeNextBlock(ushort address)
        {
            var mmuCache = this.mmuFactory.GetMmuCache(this.mmu, address);
            var timer = new InstructionTimer();
            var expressionBuilder = new Z80ExpressionBuilder(mmuCache, timer);
            var lastResult = DecodeResult.Continue;

            while (lastResult == DecodeResult.Continue)
            {
                lastResult = expressionBuilder.TryDecodeNextOperation();
                
                if (mmuCache.TotalBytesRead == ushort.MaxValue && lastResult == DecodeResult.Continue)
                {
                    lastResult = DecodeResult.FinalizeAndSync;
                }
            }
            
            var lambda = expressionBuilder.FinalizeBlock(lastResult);
            return new InstructionBlock<IZ80Registers>(address, (ushort)mmuCache.TotalBytesRead, lambda.Compile(), timer.GetInstructionTimings());
        }
        
    }
}
