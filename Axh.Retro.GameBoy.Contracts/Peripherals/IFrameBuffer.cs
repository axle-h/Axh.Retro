namespace Axh.Retro.GameBoy.Contracts.Peripherals
{
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Contracts.Memory;

    public interface IFrameBuffer
    {
        IEnumerable<IAddressSegment>  AddressSegments { get; }

        void Halt();

        void Resume();
    }
}