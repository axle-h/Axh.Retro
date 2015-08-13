namespace Axh.Retro.CPU.X80.Core
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Core;

    public class Z80InstructionDecoder : IZ80InstructionDecoder
    {
        private readonly IMmuFactory mmuFactory;

        private readonly IMmu mmu;

        public Z80InstructionDecoder(IMmuFactory mmuFactory, IMmu mmu)
        {
            this.mmuFactory = mmuFactory;
            this.mmu = mmu;
        }
        
        public Z80DynamicallyRecompiledBlock DecodeNextBlock(ushort address)
        {
            var mmuCache = mmuFactory.GetMmuCache(mmu, address);
            var expressions = new List<Expression>();
            var timer = new InstructionTimer();

            while (true)
            {
                // Break the loop if the instructionndecoder returns false
                // Also, don't let a block get bigger than the total memory size.
                if (!Z80ExpressionBuilder.TryDecodeNextOperation(mmuCache, expressions, timer) || mmuCache.TotalBytesRead == ushort.MaxValue)
                {
                    break;
                }
            }
            
            var lambda = Z80ExpressionBuilder.FinalizeBlock(mmuCache, expressions);
            return new Z80DynamicallyRecompiledBlock
                   {
                       Address = address,
                       Action = lambda.Compile(),
                       Length = (ushort)mmuCache.TotalBytesRead,
                       MachineCycles = timer.MachineCycles,
                       ThrottlingStates = timer.ThrottlingStates
                   };
        }
        
    }
}
