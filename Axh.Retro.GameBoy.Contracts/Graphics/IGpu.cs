using System;
using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    public interface IGpu : IDisposable
    {
        IEnumerable<IAddressSegment> AddressSegments { get; }

        void Halt();

        void Resume();
    }
}