using System;
using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    /// <summary>
    ///     Direct memory access controller.
    ///     Required to avoid a cyclical dependency on the MMU for IO peripherals with DMA capabilities.
    /// </summary>
    public interface IDmaController : IDisposable
    {
        /// <summary>
        ///     Creates a DMA copy operation.
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
        ///     Tries to get a dma operation from the queue.
        ///     This should have a reasonable timeout.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>True if an operation has successfully been returned from the queue.</returns>
        bool TryGet(out IDmaOperation operation);
    }
}