using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Common.Memory
{
    /// <summary>
    ///     Direct memory access controller.
    ///     Required to avoid a cyclical dependency on the MMU for IO peripherals with DMA capabilities.
    /// </summary>
    public class DmaController : IDmaController
    {
        private const int Timeout = 100;
        private readonly object _disposingContext = new object();
        private readonly BlockingCollection<IDmaOperation> _dmaOperations;
        private bool _disposed;


        /// <summary>
        ///     Initializes a new instance of the <see cref="DmaController" /> class.
        /// </summary>
        public DmaController()
        {
            _dmaOperations = new BlockingCollection<IDmaOperation>();
        }

        /// <summary>
        ///     Creates a DMA copy operation.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="destinationAddress">The destination address.</param>
        /// <param name="length">The length.</param>
        /// <param name="timings">The cpu cycles required to execute this operation.</param>
        /// <param name="lockedAddressesRanges">The address ranges to lock during the copy operation.</param>
        public void Copy(ushort sourceAddress,
            ushort destinationAddress,
            int length,
            InstructionTimings timings,
            IEnumerable<AddressRange> lockedAddressesRanges)
        {
            _dmaOperations.Add(new DmaCopyOperation(sourceAddress,
                destinationAddress,
                length,
                timings,
                lockedAddressesRanges));
        }

        /// <summary>
        ///     Tries to get a dma operation from the queue.
        ///     This should have a reasonable timeout.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>True if an operation has successfully been returned from the queue.</returns>
        public bool TryGet(out IDmaOperation operation)
        {
            if (!_disposed)
            {
                return _dmaOperations.TryTake(out operation, Timeout) && operation != null;
            }

            operation = null;
            return false;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            lock (_disposingContext)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
            }

            _dmaOperations.CompleteAdding();

            var minimumTimeout = Task.Delay(Timeout);
            var timeout = Task.Delay(Timeout * 10);
            while (_dmaOperations.Any())
            {
                var iteration = Task.Delay(100);
                var completedTask = Task.WhenAny(timeout, iteration).Result;

                if (completedTask == timeout)
                {
                    throw new Exception("Cannot dispose");
                }
            }

            minimumTimeout.Wait();
            _dmaOperations.Dispose();
        }
    }
}