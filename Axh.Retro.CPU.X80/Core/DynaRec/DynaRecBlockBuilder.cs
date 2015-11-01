namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Core.Timing;
    using Axh.Retro.CPU.X80.Contracts.IO;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Core.Timing;
    using Axh.Retro.CPU.X80.Util;

    using Xpr = DynaRecExpressions;

    /// <summary>
    /// Core opcode decoder & expression builder logic.
    /// This is not thread safe.
    /// </summary>
    internal partial class DynaRecBlockBuilder<TRegisters> where TRegisters : IRegisters
    {
        private readonly CpuMode cpuMode;
        private readonly IInstructionTimingsBuilder timingsBuilder;
        private readonly IMmuCache mmuCache;

        private ConstantExpression NextByte => Expression.Constant(mmuCache.NextByte(), typeof(byte));
        private ConstantExpression NextWord => Expression.Constant(mmuCache.NextWord(), typeof(ushort));
        private Expression SyncProgramCounter => Expression.Assign(Xpr.PC, Expression.Convert(Expression.Add(Expression.Convert(Xpr.PC, typeof(int)), Expression.Constant(this.mmuCache.TotalBytesRead)), typeof(ushort)));

        private DecodeResult lastDecodeResult;
        private IndexRegisterExpressions index;

        public DynaRecBlockBuilder(CpuMode cpuMode, IMmuCache mmuCache, IInstructionTimingsBuilder timingsBuilder)
        {
            this.cpuMode = cpuMode;
            this.timingsBuilder = timingsBuilder;
            this.mmuCache = mmuCache;
        }

        public Expression<Func<TRegisters, IMmu, IArithmeticLogicUnit, IInputOutputManager, InstructionTimings>> DecodeNextBlock()
        {
            var initExpressions = GetBlockInitExpressions();
            var blockExpressions = GetBlockExpressions();
            var finalExpressions = GetBlockFinalExpressions();

            var expressions = initExpressions.Concat(blockExpressions).Concat(finalExpressions).ToArray();

            var expressionBlock = Expression.Block(GeParameterExpressions(), expressions);
            var lambda = Expression.Lambda<Func<TRegisters, IMmu, IArithmeticLogicUnit, IInputOutputManager, InstructionTimings>>(expressionBlock, Xpr.Registers, Xpr.Mmu, Xpr.Alu, Xpr.IO);

            return lambda;
        }

        private IEnumerable<ParameterExpression> GeParameterExpressions()
        {
            yield return Xpr.LocalByte;
            yield return Xpr.LocalWord;
            yield return Xpr.DynamicTimer;

            if (this.CpuSupportsAccummulatorAndResultOperations())
            {
                yield return Xpr.AccumulatorAndResult;
            }
            
        }

        private static IEnumerable<Expression> GetBlockInitExpressions()
        {
            // Create a new dynamic timer to record any timings calculated at runtime.
            yield return Expression.Assign(Xpr.DynamicTimer, Expression.New(typeof(InstructionTimingsBuilder)));
        }

        private IEnumerable<Expression> GetBlockFinalExpressions()
        {
            if (lastDecodeResult == DecodeResult.FinalizeAndSync)
            {
                // Increment the program counter by how many bytes were read.
                yield return SyncProgramCounter;
            }

            // Add the block length to the 7 lsb of memory refresh register.
            var blockLengthExpression = Expression.Constant(this.mmuCache.TotalBytesRead, typeof(int));
            yield return Xpr.GetMemoryRefreshDeltaExpression(blockLengthExpression);

            // Return the dynamic timings.
            var returnTarget = Expression.Label(typeof(InstructionTimings), "InstructionTimings_Return");
            var getInstructionTimings = ExpressionHelpers.GetMethodInfo<IInstructionTimingsBuilder>(dt => dt.GetInstructionTimings());
            yield return Expression.Return(returnTarget, Expression.Call(Xpr.DynamicTimer, getInstructionTimings), typeof(InstructionTimings));
            yield return Expression.Label(returnTarget, Expression.Constant(default(InstructionTimings)));
        }
    }
}
