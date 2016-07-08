using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Core.Decode;
using Axh.Retro.CPU.Z80.Timing;
using Axh.Retro.CPU.Z80.Util;

namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    /// <summary>
    /// Instruction block decoder using a dynamic translation from Z80 operations to expression trees.
    /// </summary>
    /// <typeparam name="TRegisters">The type of the registers.</typeparam>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Core.IInstructionBlockDecoder{TRegisters}" />
    public partial class DynaRec<TRegisters> : IInstructionBlockDecoder<TRegisters> where TRegisters : IRegisters
    {
        private readonly CpuMode _cpuMode;
        private readonly bool _debug;
        private readonly OpCodeDecoder _decoder;
        private readonly IPrefetchQueue _prefetch;

        private DecodeResult _lastDecodeResult;

        private bool _usesAccumulatorAndResult;
        private bool _usesDynamicTimings;
        private bool _usesLocalWord;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynaRec{TRegisters}"/> class.
        /// </summary>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <param name="runtimeConfig">The runtime configuration.</param>
        /// <param name="prefetch">The prefetch.</param>
        public DynaRec(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPrefetchQueue prefetch) : this()
        {
            _prefetch = prefetch;
            _cpuMode = platformConfig.CpuMode;
            _debug = runtimeConfig.DebugMode;
            _decoder = new OpCodeDecoder(platformConfig, prefetch);
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
        public IInstructionBlock<TRegisters> DecodeNextBlock(ushort address)
        {
            var decodedBlock = _decoder.DecodeNextBlock(address);
            var lambda = BuildExpressionTree(decodedBlock.Operations);

            var block = new DynaRecInstructionBlock<TRegisters>(address,
                                                                (ushort) _prefetch.TotalBytesRead,
                                                                lambda.Compile(),
                                                                decodedBlock.Timings,
                                                                _lastDecodeResult);
            if (_debug)
            {
                block.DebugInfo =
                    $"{string.Join("\n", decodedBlock.Operations.Select(x => x.ToString()))}\n\n{lambda.DebugView()}";
            }

            return block;
        }

        /// <summary>
        /// Builds the expression tree.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <returns></returns>
        private Expression<Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings>> BuildExpressionTree(
            IEnumerable<Operation> operations)
        {
            // Reset
            _usesDynamicTimings = false;
            _usesLocalWord = false;
            _usesAccumulatorAndResult = false;
            _lastDecodeResult = DecodeResult.Continue;

            // Run this first so we know what init & final expressions to add.
            var blockExpressions = operations.SelectMany(Recompile).ToArray();
            var initExpressions = GetBlockInitExpressions();
            var finalExpressions = GetBlockFinalExpressions();

            var expressions = initExpressions.Concat(blockExpressions).Concat(finalExpressions).ToArray();

            var expressionBlock = Expression.Block(GetParameterExpressions(), expressions);

            var lambda = Expression.Lambda<Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings>>(expressionBlock,
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
        /// <returns></returns>
        private IEnumerable<Expression> GetBlockFinalExpressions()
        {
            if (_debug)
            {
                yield return GetDebugExpression("Block Finalize");
            }

            if (_lastDecodeResult == DecodeResult.FinalizeAndSync || _lastDecodeResult == DecodeResult.Halt ||
                _lastDecodeResult == DecodeResult.Stop)
            {
                // Increment the program counter by how many bytes were read.
                yield return SyncProgramCounter;
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