namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Util;

    /// <summary>
    /// Core opcode decoder & expression builder logic.
    /// This is not thread safe.
    /// </summary>
    internal partial class DynaRecBlockBuilder<TRegisters> where TRegisters : IRegisters
    {
        private readonly CpuMode cpuMode;
        private readonly IInstructionTimer timer;
        private readonly IMmuCache mmuCache;

        private ConstantExpression NextByte => Expression.Constant(mmuCache.NextByte(), typeof(byte));
        private ConstantExpression NextWord => Expression.Constant(mmuCache.NextWord(), typeof(ushort));

        private DecodeResult lastDecodeResult;
        private IndexRegisterExpressions index;

        public DynaRecBlockBuilder(CpuMode cpuMode, IMmuCache mmuCache, IInstructionTimer timer)
        {
            this.cpuMode = cpuMode;
            this.timer = timer;
            this.mmuCache = mmuCache;
        }

        public Expression<Func<TRegisters, IMmu, IArithmeticLogicUnit, InstructionTimings>> DecodeNextBlock()
        {
            var initExpressions = GetBlockInitExpressions();
            var blockExpressions = GetBlockExpressions();
            var finalExpressions = GetBlockFinalExpressions();

            var expressions = initExpressions.Concat(blockExpressions).Concat(finalExpressions).ToArray();

            var expressionBlock = Expression.Block(GeParameterExpressions(), expressions);
            var lambda = Expression.Lambda<Func<TRegisters, IMmu, IArithmeticLogicUnit, InstructionTimings>>(expressionBlock, DynaRecExpressions.Registers, DynaRecExpressions.Mmu, DynaRecExpressions.Alu);

            return lambda;
        }

        private IEnumerable<ParameterExpression> GeParameterExpressions()
        {
            yield return DynaRecExpressions.LocalByte;
            yield return DynaRecExpressions.LocalWord;

            if (this.CpuSupportsAccummulatorAndResultOperations())
            {
                yield return DynaRecExpressions.AccumulatorAndResult;
            }

            if (this.CpuSupportsDynamicTimings())
            {
                yield return DynaRecExpressions.DynamicTimer;
            }
        }

        private IEnumerable<Expression> GetBlockInitExpressions()
        {
            if (this.CpuSupportsDynamicTimings())
            {
                // Create a new dynamic timer to record any timings calculated at runtime.
                yield return Expression.Assign(DynaRecExpressions.DynamicTimer, Expression.New(typeof(InstructionTimer)));
            }     
        }

        private IEnumerable<Expression> GetBlockFinalExpressions()
        {
            if (lastDecodeResult == DecodeResult.FinalizeAndSync)
            {
                // Increment the program counter by how many bytes were read.
                yield return Expression.Assign(DynaRecExpressions.PC, Expression.Convert(Expression.Add(Expression.Convert(DynaRecExpressions.PC, typeof(int)), Expression.Constant(this.mmuCache.TotalBytesRead)), typeof(ushort)));
            }

            // Add the block length to the 7 lsb of memory refresh register.
            var blockLengthExpression = Expression.Constant(this.mmuCache.TotalBytesRead, typeof(int));
            yield return DynaRecExpressions.GetMemoryRefreshDeltaExpression(blockLengthExpression);

            // Return the dynamic timings.
            var returnTarget = Expression.Label(typeof(InstructionTimings), "InstructionTimings_Return");

            if (this.CpuSupportsDynamicTimings())
            {
                // Only Z80 actually calculates the timings.
                var getInstructionTimings = ExpressionHelpers.GetMethodInfo<IInstructionTimer>(dt => dt.GetInstructionTimings());
                yield return Expression.Return(returnTarget, Expression.Call(DynaRecExpressions.DynamicTimer, getInstructionTimings), typeof(InstructionTimings));
            }

            yield return Expression.Label(returnTarget, Expression.Constant(default(InstructionTimings)));
        }
    }
}
