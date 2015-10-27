﻿namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Util;

    using Xpr = Z80Expressions;

    /// <summary>
    /// Core opcode decoder & expression builder logic.
    /// This is not thread safe.
    /// </summary>
    internal partial class Z80BlockBuilder
    {
        private readonly CpuMode cpuMode;
        private readonly IInstructionTimer timer;
        private readonly IMmuCache mmuCache;

        private ConstantExpression NextByte => Expression.Constant(mmuCache.NextByte(), typeof(byte));
        private ConstantExpression NextWord => Expression.Constant(mmuCache.NextWord(), typeof(ushort));

        private DecodeResult lastDecodeResult;
        private IndexRegisterExpressions index;

        public Z80BlockBuilder(CpuMode cpuMode, IMmuCache mmuCache, IInstructionTimer timer)
        {
            this.cpuMode = cpuMode;
            this.timer = timer;
            this.mmuCache = mmuCache;
        }

        public Expression<Func<IZ80Registers, IMmu, IArithmeticLogicUnit, InstructionTimings>> DecodeNextBlock()
        {
            var initExpressions = GetBlockInitExpressions();
            var blockExpressions = GetBlockExpressions();
            var finalExpressions = GetBlockFinalExpressions();

            var expressions = initExpressions.Concat(blockExpressions).Concat(finalExpressions).ToArray();

            var expressionBlock = Expression.Block(GeParameterExpressions(), expressions);
            var lambda = Expression.Lambda<Func<IZ80Registers, IMmu, IArithmeticLogicUnit, InstructionTimings>>(expressionBlock, Xpr.Registers, Xpr.Mmu, Xpr.Alu);

            return lambda;
        }

        private IEnumerable<ParameterExpression> GeParameterExpressions()
        {
            yield return Xpr.LocalByte;
            yield return Xpr.LocalWord;

            if (this.CpuSupportsAccummulatorAndResultOperations())
            {
                yield return Xpr.AccumulatorAndResult;
            }

            if (this.CpuSupportsDynamicTimings())
            {
                yield return Xpr.DynamicTimer;
            }
        }

        private IEnumerable<Expression> GetBlockInitExpressions()
        {
            if (this.CpuSupportsDynamicTimings())
            {
                // Create a new dynamic timer to record any timings calculated at runtime.
                yield return Expression.Assign(Xpr.DynamicTimer, Expression.New(typeof(InstructionTimer)));
            }     
        }

        private IEnumerable<Expression> GetBlockFinalExpressions()
        {
            if (lastDecodeResult == DecodeResult.FinalizeAndSync)
            {
                // Increment the program counter by how many bytes were read.
                yield return Expression.Assign(Xpr.PC, Expression.Convert(Expression.Add(Expression.Convert(Xpr.PC, typeof(int)), Expression.Constant(this.mmuCache.TotalBytesRead)), typeof(ushort)));
            }

            // Add the block length to the 7 lsb of memory refresh register.
            var blockLengthExpression = Expression.Constant(this.mmuCache.TotalBytesRead, typeof(int));
            yield return Xpr.GetMemoryRefreshDeltaExpression(blockLengthExpression);

            // Return the dynamic timings.
            var returnTarget = Expression.Label(typeof(InstructionTimings), "InstructionTimings_Return");

            if (this.CpuSupportsDynamicTimings())
            {
                // Only Z80 actually calculates the timings.
                var getInstructionTimings = ExpressionHelpers.GetMethodInfo<IInstructionTimer>(dt => dt.GetInstructionTimings());
                yield return Expression.Return(returnTarget, Expression.Call(Xpr.DynamicTimer, getInstructionTimings), typeof(InstructionTimings));
            }

            yield return Expression.Label(returnTarget, Expression.Constant(default(InstructionTimings)));
        }
    }
}