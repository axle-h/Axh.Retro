namespace Axh.Retro.GameBoy.Peripherals
{
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Contracts.Peripherals;

    public class HardwareRegistersPeripheral : IMemoryMappedPeripheral
    {
        private readonly IAddressSegment hardwareRegistersAddressSegment;

        public HardwareRegistersPeripheral(IAddressSegment hardwareRegistersAddressSegment)
        {
            this.hardwareRegistersAddressSegment = hardwareRegistersAddressSegment;
        }

        public void Halt()
        {
            // Do nothing
        }

        public void Resume()
        {
            // Do nothing
        }

        public IEnumerable<IAddressSegment> AddressSegments => new[] { hardwareRegistersAddressSegment };
    }
}
