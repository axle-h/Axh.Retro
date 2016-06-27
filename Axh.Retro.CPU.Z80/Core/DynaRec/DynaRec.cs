using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Timing;

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
    using Axh.Retro.CPU.Z80.Core.Decode;
    using Axh.Retro.CPU.Z80.Util;

    public partial class DynaRec<TRegisters> : IInstructionBlockDecoder<TRegisters> where TRegisters : IRegisters
    {
        private readonly IPrefetchQueue prefetch;

        private readonly CpuMode cpuMode;

        private readonly bool debug;

        private readonly OpCodeDecoder decoder;

        private bool usesDynamicTimings;

        private bool usesLocalWord;

        private bool usesAccumulatorAndResult;

        private DecodeResult lastDecodeResult;

        private Expression SyncProgramCounter => Expression.Assign(PC, Expression.Convert(Expression.Add(Expression.Convert(PC, typeof(int)), Expression.Constant(this.prefetch.TotalBytesRead)), typeof(ushort)));

        public DynaRec(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPrefetchQueue prefetch) : this()
        {
            this.prefetch = prefetch;
            this.cpuMode = platformConfig.CpuMode;
            this.debug = runtimeConfig.DebugMode;
            
            this.decoder = new OpCodeDecoder(platformConfig, prefetch);
        }

        public bool SupportsInstructionBlockCaching => true;

        public IInstructionBlock<TRegisters> DecodeNextBlock(ushort address)
        {
            var decodedBlock = this.decoder.GetNextBlock(address);
            var lambda = this.BuildExpressionTree(decodedBlock.Operations);

            var block = new DynaRecInstructionBlock<TRegisters>(address, (ushort)prefetch.TotalBytesRead, lambda.Compile(), decodedBlock.Timings, lastDecodeResult);
            if (this.debug)
            {
                block.DebugInfo = $"{string.Join("\n", decodedBlock.Operations.Select(x => x.ToString()))}\n\n{lambda.DebugView()}";
            }

            return block;
        }

        private Expression<Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings>> BuildExpressionTree(IEnumerable<Operation> operations)
        {
            // Reset
            this.usesDynamicTimings = false;
            this.usesLocalWord = false;
            this.usesAccumulatorAndResult = false;
            this.lastDecodeResult = DecodeResult.Continue;

            // Run this first so we know what init & final expressions to add.
            var blockExpressions = operations.SelectMany(Recompile).ToArray();
            var initExpressions = GetBlockInitExpressions();
            var finalExpressions = GetBlockFinalExpressions();

            var expressions = initExpressions.Concat(blockExpressions).Concat(finalExpressions).ToArray();

            var expressionBlock = Expression.Block(GeParameterExpressions(), expressions);

            var lambda = Expression.Lambda<Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings>>(expressionBlock, Registers, Mmu, Alu, IO);
            return lambda;
        }
        
        private IEnumerable<ParameterExpression> GeParameterExpressions()
        {
            if (this.usesLocalWord)
            {
                yield return LocalWord;
            }

            if (usesDynamicTimings)
            {
                yield return DynamicTimer;
            }

            if (this.usesAccumulatorAndResult)
            {
                // Z80 supports some opcodes that manipulate the accumulator and a result in memory at the same time.
                yield return AccumulatorAndResult;
            }
        }

        private IEnumerable<Expression> GetBlockInitExpressions()
        {
            if (usesDynamicTimings)
            {
                // Create a new dynamic timer to record any timings calculated at runtime.
                yield return Expression.Assign(DynamicTimer, Expression.New(typeof(InstructionTimingsBuilder)));
            }
        }

        private IEnumerable<Expression> GetBlockFinalExpressions()
        {
            if (this.debug)
            {
                yield return GetDebugExpression("Block Finalize");
            }

            if (lastDecodeResult == DecodeResult.FinalizeAndSync || lastDecodeResult == DecodeResult.Halt || lastDecodeResult == DecodeResult.Stop)
            {
                // Increment the program counter by how many bytes were read.
                yield return SyncProgramCounter;
            }

            if (this.cpuMode == CpuMode.Z80)
            {
                // Add the block length to the 7 lsb of memory refresh register.
                var blockLengthExpression = Expression.Constant(prefetch.TotalBytesRead, typeof(int));

                // Update Z80 specific memory refresh register
                yield return GetMemoryRefreshDeltaExpression(blockLengthExpression);
            }

            var returnTarget = Expression.Label(typeof(InstructionTimings), "InstructionTimings_Return");

            // Return the dynamic timings.
            if (usesDynamicTimings)
            {
                var getInstructionTimings = ExpressionHelpers.GetMethodInfo<IInstructionTimingsBuilder>(dt => dt.GetInstructionTimings());
                yield return Expression.Return(returnTarget, Expression.Call(DynamicTimer, getInstructionTimings), typeof(InstructionTimings));
            }

            yield return Expression.Label(returnTarget, Expression.Constant(default(InstructionTimings)));
        }
    }
}
