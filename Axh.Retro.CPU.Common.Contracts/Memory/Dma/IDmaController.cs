using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Common.Contracts.Memory.Dma
{
    /// <summary>
    /// Direct memory access controller.
    /// Required to avoid a cyclical dependency on the MMU for IO peripherals with DMA capabilities.
    /// </summary>
    public interface IDmaController : IDisposable
    {
        /// <summary>
        /// Creates a DMA copy operation.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationAddress">The destination address.</param>
        /// <param name="length">The length.</param>
        /// <param name="timings">The cpu cycles required to execute this operation.</param>
        /// <param name="lockedAddressesRanges">The address ranges to lock during the copy operation.</param>
        void Copy(ushort sourceAddress,
            ushort destinationAddress,
            int length,
            InstructionTimings timings,
            IEnumerable<AddressRange> lockedAddressesRanges);
        
        /// <summary>
        /// Gets the next DMA operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IDmaOperation> GetNextAsync(CancellationToken cancellationToken);
    }
}