namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    using Xpr = Z80Expressions;

    internal partial class Z80BlockBuilder
    {
        private readonly IInstructionTimer timer;
        private readonly IMmuCache mmuCache;

        private ConstantExpression NextByte => Expression.Constant(mmuCache.NextByte(), typeof(byte));
        private ConstantExpression NextWord => Expression.Constant(mmuCache.NextWord(), typeof(ushort));
        
        public Z80BlockBuilder(IMmuCache mmuCache, IInstructionTimer timer)
        {
            this.timer = timer;
            this.mmuCache = mmuCache;
        }

        public Expression<Func<IZ80Registers, IMmu, IArithmeticLogicUnit, InstructionTimings>> DecodeNextBlock()
        {
            var expressions = GetBlockExpressions().ToArray();

            var expressionBlock = Expression.Block(new[] { Xpr.LocalByte, Xpr.LocalWord, Xpr.DynamicTimer }, expressions);
            var lambda = Expression.Lambda<Func<IZ80Registers, IMmu, IArithmeticLogicUnit, InstructionTimings>>(expressionBlock, Xpr.Registers, Xpr.Mmu, Xpr.Alu);

            return lambda;
        }
    }
}
