using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.Timing;
using Axh.Retro.CPU.Z80.Core.Decode;
using Axh.Retro.CPU.Z80.Timing;
using Axh.Retro.CPU.Z80.Util;

namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    /// <summary>
    /// Instruction block decoder using a dynamic translation from Z80 operations to expression trees.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Core.IInstructionBlockDecoder" />
    public partial class DynaRec : IInstructionBlockDecoder
    {
        private readonly CpuMode _cpuMode;
        private readonly bool _debug;
        private readonly IOpCodeDecoder _decoder;
        private readonly IPrefetchQueue _prefetch;

        private bool _usesAccumulatorAndResult;
        private bool _usesDynamicTimings;
        private bool _usesLocalWord;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynaRec"/> class.
        /// </summary>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <param name="runtimeConfig">The runtime configuration.</param>
        /// <param name="prefetch">The prefetch.</param>
        /// <param name="decoder">The decoder.</param>
        public DynaRec(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPrefetchQueue prefetch, IOpCodeDecoder decoder) : this()
        {
            _prefetch = prefetch;
            _cpuMode = platformConfig.CpuMode;
            _debug = runtimeConfig.DebugMode;
            _decoder = decoder;
        }

        /// <summary>
        /// Gets an expression that synchronizes the program counter to the bytes read from teh prefetch queue.
        /// </summary>
        /// <value>
        /// An expression that synchronizes the program counter to the bytes read from teh prefetch queue.
        /// </value>
        private Expression SyncProgramCounter => Expression.Assign(PC, Expression.Convert(Expression.Add(Expression.Convert(PC, typeof (int)),
                                                                                          Expression.Constant(_prefetch.TotalBytesRead)),
                                                                                          typeof (ushort)));
        /// <summary>
        /// Gets a value indicating whether this instruction block decoder [supports instruction block caching].
        /// </summary>
        /// <value>
        /// <c>true</c> if this instruction block decoder [supports instruction block caching]; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsInstructionBlockCaching => true;

        /// <summary>
        /// Decodes the next block.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public IInstructionBlock DecodeNextBlock(ushort address)
        {
            var decodedBlock = _decoder.DecodeNextBlock(address);
            var lambda = BuildExpressionTree(decodedBlock);
            var debugInfo = _debug
                                ? $"{string.Join("\n", decodedBlock.Operations.Select(x => x.ToString()))}\n\n{lambda.DebugView()}"
                                : null;

            return new InstructionBlock(address,
                                        (ushort) _prefetch.TotalBytesRead,
                                        lambda.Compile(),
                                        decodedBlock.Timings,
                                        decodedBlock.Halt,
                                        decodedBlock.Stop,
                                        debugInfo);
        }

        /// <summary>
        /// Builds the expression tree.
        /// </summary>
        /// <param name="block">The decoded block.</param>
        /// <returns></returns>
        private Expression<Func<IRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings>> BuildExpressionTree(DecodedBlock block)
        {
            // Reset
            _usesDynamicTimings = _usesLocalWord = _usesAccumulatorAndResult = false;

            // Run this first so we know what init & final expressions to add.
            var blockExpressions = block.Operations.SelectMany(Recompile).ToArray();
            var initExpressions = GetBlockInitExpressions();
            var finalExpressions = GetBlockFinalExpressions(block);

            var expressions = initExpressions.Concat(blockExpressions).Concat(finalExpressions).ToArray();

            var expressionBlock = Expression.Block(GetParameterExpressions(), expressions);

            var lambda = Expression.Lambda<Func<IRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings>>(expressionBlock,
                                                                                                                 Registers,
                                                                                                                 Mmu,
                                                                                                                 Alu,
                                                                                                                 IO);
            return lambda;
        }

        /// <summary>
        /// Gets the parameter expressions of the expression block.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ParameterExpression> GetParameterExpressions()
        {
            if (_usesLocalWord)
            {
                yield return LocalWord;
            }

            if (_usesDynamicTimings)
            {
                yield return DynamicTimer;
            }

            if (_usesAccumulatorAndResult)
            {
                // Z80 supports some opcodes that manipulate the accumulator and a result in memory at the same time.
                yield return AccumulatorAndResult;
            }
        }

        /// <summary>
        /// Gets the expression block initialize expressions.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Expression> GetBlockInitExpressions()
        {
            if (_usesDynamicTimings)
            {
                // Create a new dynamic timer to record any timings calculated at runtime.
                yield return Expression.Assign(DynamicTimer, Expression.New(typeof (InstructionTimingsBuilder)));
            }
        }

        /// <summary>
        /// Gets any expressions required to finalize the expression block.
        /// </summary>
        /// <param name="block">The decoded block.</param>
        /// <returns></returns>
        private IEnumerable<Expression> GetBlockFinalExpressions(DecodedBlock block)
        {
            if (_debug)
            {
                yield return GetDebugExpression("Block Finalize");
            }

            if (_cpuMode == CpuMode.Z80)
            {
                // Add the block length to the 7 lsb of memory refresh register.
                var blockLengthExpression = Expression.Constant(_prefetch.TotalBytesRead, typeof (int));

                // Update Z80 specific memory refresh register
                yield return GetMemoryRefreshDeltaExpression(blockLengthExpression);
            }

            var returnTarget = Expression.Label(typeof (InstructionTimings), "InstructionTimings_Return");

            // Return the dynamic timings.
            if (_usesDynamicTimings)
            {
                var getInstructionTimings =
                    ExpressionHelpers.GetMethodInfo<IInstructionTimingsBuilder>(dt => dt.GetInstructionTimings());
                yield return
                    Expression.Return(returnTarget,
                                      Expression.Call(DynamicTimer, getInstructionTimings),
                                      typeof (InstructionTimings));
            }

            yield return Expression.Label(returnTarget, Expression.Constant(default(InstructionTimings)));
        }
    }
}