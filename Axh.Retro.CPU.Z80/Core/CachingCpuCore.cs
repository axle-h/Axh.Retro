using System;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Memory.Dma;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Cache;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Core
{
    /// <summary>
    /// A Z80 CPU core that caches decoded instruction blocks.
    /// This must be used with an <see cref="IInstructionBlockDecoder{TRegisters}"/> that suppots caching.
    /// </summary>
    /// <typeparam name="TRegisters">The type of the registers.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class CachingCpuCore<TRegisters> : CpuCoreBase<TRegisters>
        where TRegisters : IRegisters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachingCpuCore{TRegisters}"/> class.
        /// </summary>
        /// <param name="registers"></param>
        /// <param name="interruptManager"></param>
        /// <param name="peripheralManager"></param>
        /// <param name="mmu"></param>
        /// <param name="instructionTimer"></param>
        /// <param name="alu"></param>
        /// <param name="instructionBlockCache"></param>
        /// <param name="instructionBlockDecoder"></param>
        /// <param name="dmaController"></param>
        public CachingCpuCore(TRegisters registers,
            IInterruptManager interruptManager,
            IPeripheralManager peripheralManager,
            IMmu mmu,
            IInstructionTimer instructionTimer,
            IAlu alu,
            IInstructionBlockCache<TRegisters> instructionBlockCache,
            IInstructionBlockDecoder<TRegisters> instructionBlockDecoder,
            IDmaController dmaController)
            : base(
                registers,
                interruptManager,
                peripheralManager,
                mmu,
                instructionTimer,
                alu,
                instructionBlockCache,
                instructionBlockDecoder,
                dmaController)
        {
        }

        /// <summary>
        /// Starts the core process.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">interruptManager</exception>
        /// <exception cref="System.Exception">Instruction block decoder must support caching</exception>
        public override async Task StartCoreProcessAsync(CancellationToken cancellationToken)
        {
            if (!InstructionBlockDecoder.SupportsInstructionBlockCaching)
            {
                throw new Exception("Instruction block decoder must support caching");
            }
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var address = GetNextAddress();
                var instructionBlock = InstructionBlockCache.GetOrSet(address, () => InstructionBlockDecoder.DecodeNextBlock(address));
                await ExecuteInstructionBlockAsync(instructionBlock);
            }
        }
    }
}