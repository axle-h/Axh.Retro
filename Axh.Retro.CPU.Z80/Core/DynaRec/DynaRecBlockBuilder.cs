namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core.Timing;
    using Axh.Retro.CPU.Z80.Util;

    /// <summary>
    /// Core opcode decoder & expression builder logic.
    /// This is not thread safe.
    /// </summary>
    internal partial class DynaRecBlockBuilder<TRegisters> where TRegisters : IRegisters
    {
        private readonly CpuMode cpuMode;
        private readonly IInstructionTimingsBuilder timings;
        private readonly IPrefetchQueue prefetchQueue;

        private static readonly DynaRecExpressions<TRegisters> Xpr = new DynaRecExpressions<TRegisters>();
        
        private ConstantExpression NextByte => Expression.Constant(prefetchQueue.NextByte(), typeof(byte));
        private ConstantExpression NextWord => Expression.Constant(prefetchQueue.NextWord(), typeof(ushort));
        private Expression SyncProgramCounter => Expression.Assign(Xpr.PC, Expression.Convert(Expression.Add(Expression.Convert(Xpr.PC, typeof(int)), Expression.Constant(this.prefetchQueue.TotalBytesRead)), typeof(ushort)));

        private IndexRegisterExpressions index;

        private bool usesDynamicTimings;

        public DynaRecBlockBuilder(CpuMode cpuMode, IPrefetchQueue prefetchQueue, IInstructionTimingsBuilder timings)
        {
            this.cpuMode = cpuMode;
            this.timings = timings;
            this.prefetchQueue = prefetchQueue;
        }

        public DecodeResult LastDecodeResult { get; private set; }
        
        public Expression<Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings>> DecodeNextBlock()
        {
            this.timings.Reset();

            // Run this first so we know what iinit & final expressions to add.
            var blockExpressions = GetBlockExpressions().ToArray();
            var initExpressions = GetBlockInitExpressions();
            var finalExpressions = GetBlockFinalExpressions();

            var expressions = initExpressions.Concat(blockExpressions).Concat(finalExpressions).ToArray();

            var expressionBlock = Expression.Block(GeParameterExpressions(), expressions);

            var lambda = Expression.Lambda<Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings>>(expressionBlock, Xpr.Registers, Xpr.Mmu, Xpr.Alu, Xpr.IO);
            return lambda;
        }

        private IEnumerable<ParameterExpression> GeParameterExpressions()
        {
            yield return Xpr.LocalByte;
            yield return Xpr.LocalWord;

            if (usesDynamicTimings)
            {
                yield return Xpr.DynamicTimer;
            }
            
            if (this.CpuSupportsAccummulatorAndResultOperations())
            {
                yield return Xpr.AccumulatorAndResult;
            }
        }

        private IEnumerable<Expression> GetBlockInitExpressions()
        {
            if (usesDynamicTimings)
            {
                // Create a new dynamic timer to record any timings calculated at runtime.
                yield return Expression.Assign(Xpr.DynamicTimer, Expression.New(typeof(InstructionTimingsBuilder)));
            }
        }

        private IEnumerable<Expression> GetBlockFinalExpressions()
        {
            if (LastDecodeResult == DecodeResult.FinalizeAndSync || LastDecodeResult == DecodeResult.Halt || LastDecodeResult == DecodeResult.Stop)
            {
                // Increment the program counter by how many bytes were read.
                yield return SyncProgramCounter;
            }

            if (this.cpuMode == CpuMode.Z80)
            {
                // Add the block length to the 7 lsb of memory refresh register.
                var blockLengthExpression = Expression.Constant(this.prefetchQueue.TotalBytesRead, typeof(int));

                // Update Z80 specific memory refresh register
                yield return Xpr.GetMemoryRefreshDeltaExpression(blockLengthExpression);
            }

            var returnTarget = Expression.Label(typeof(InstructionTimings), "InstructionTimings_Return");

            // Return the dynamic timings.
            if (usesDynamicTimings)
            {
                var getInstructionTimings = ExpressionHelpers.GetMethodInfo<IInstructionTimingsBuilder>(dt => dt.GetInstructionTimings());
                yield return Expression.Return(returnTarget, Expression.Call(Xpr.DynamicTimer, getInstructionTimings), typeof(InstructionTimings));
            }
            
            yield return Expression.Label(returnTarget, Expression.Constant(default(InstructionTimings)));
        }
    }
}
