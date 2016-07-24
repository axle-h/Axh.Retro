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
    /// A simple Z80 CPU core.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Core.CpuCoreBase" />
    public class CpuCore : CpuCoreBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CpuCore"/> class.
        /// </summary>
        /// <param name="registers">The registers.</param>
        /// <param name="interruptManager">The interrupt manager.</param>
        /// <param name="peripheralManager">The peripheral manager.</param>
        /// <param name="mmu">The mmu.</param>
        /// <param name="instructionTimer">The instruction timer.</param>
        /// <param name="alu">The alu.</param>
        /// <param name="instructionBlockDecoder">The instruction block decoder.</param>
        /// <param name="dmaController">The dma controller.</param>
        public CpuCore(IRegisters registers,
            IInterruptManager interruptManager,
            IPeripheralManager peripheralManager,
            IMmu mmu,
            IInstructionTimer instructionTimer,
            IAlu alu,
            IInstructionBlockDecoder instructionBlockDecoder,
            IDmaController dmaController)
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
        }

        /// <summary>
        /// Starts the core process.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public override async Task StartCoreProcessAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var address = GetNextAddress();
                var instructionBlock = InstructionBlockDecoder.DecodeNextBlock(address);
                await ExecuteInstructionBlockAsync(instructionBlock);
            }
        }
    }
}
