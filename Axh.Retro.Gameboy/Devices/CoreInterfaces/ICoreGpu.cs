using System;
using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    public interface ICoreGpu : IGpu, IDisposable
    {
        IEnumerable<IAddressSegment> AddressSegments { get; }

        void Halt();

        void Resume();
    }
}