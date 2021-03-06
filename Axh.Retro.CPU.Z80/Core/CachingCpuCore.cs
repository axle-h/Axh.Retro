﻿using System;
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
    /// </summary>
    public class CachingCpuCore : CpuCoreBase
    {
        private readonly IInstructionBlockCache _instructionBlockCache;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CachingCpuCore"/> class.
        /// </summary>
        /// <param name="registers">The registers.</param>
        /// <param name="interruptManager">The interrupt manager.</param>
        /// <param name="peripheralManager">The peripheral manager.</param>
        /// <param name="mmu">The mmu.</param>
        /// <param name="instructionTimer">The instruction timer.</param>
        /// <param name="alu">The alu.</param>
        /// <param name="instructionBlockDecoder">The instruction block decoder.</param>
        /// <param name="dmaController">The dma controller.</param>
        /// <param name="instructionBlockCache">The instruction block cache.</param>
        public CachingCpuCore(IRegisters registers,
            IInterruptManager interruptManager,
            IPeripheralManager peripheralManager,
            IMmu mmu,
            IInstructionTimer instructionTimer,
            IAlu alu,
            IInstructionBlockDecoder instructionBlockDecoder,
            IDmaController dmaController,
            IInstructionBlockCache instructionBlockCache)
            : base(
                registers,
                interruptManager,
                peripheralManager,
                mmu,
                instructionTimer,
                alu,
                instructionBlockDecoder,
                dmaController)
        {
            _instructionBlockCache = instructionBlockCache;
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
                var instructionBlock = _instructionBlockCache.GetOrSet(address, () => InstructionBlockDecoder.DecodeNextBlock(address));
                await ExecuteInstructionBlockAsync(instructionBlock);
            }
        }
    }
}