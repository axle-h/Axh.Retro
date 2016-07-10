using System;
using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    /// <summary>
    /// The GameBoy GPU.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IGpu : IDisposable
    {
        /// <summary>
        /// Gets the address segments.
        /// </summary>
        /// <value>
        /// The address segments.
        /// </value>
        IEnumerable<IAddressSegment> AddressSegments { get; }

        /// <summary>
        /// Halts the GPU thread.
        /// </summary>
        void Halt();

        /// <summary>
        /// Resumes the GPU thread.
        /// </summary>
        void Resume();
    }
}