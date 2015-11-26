namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    using System;
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.GameBoy.Contracts.Graphics;

    public interface ICoreGpu : IGpu
    {
        IEnumerable<IAddressSegment> AddressSegments { get; }

        void Halt();

        void Resume();
    }
}
