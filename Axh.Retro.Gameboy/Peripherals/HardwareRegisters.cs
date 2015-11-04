namespace Axh.Retro.GameBoy.Peripherals
{
    using System.Collections.Generic;

    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.CPU.X80.Memory;

    public class HardwareRegisters : IMemoryMappedPeripheral
    {
        private static readonly IMemoryBankConfig RegistersConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral, null, 0xff00, 0x7f);

        /// <summary>
        /// $FF00-$FF7F - Hardware I/O Registers
        /// </summary>
        private readonly ArrayBackedMemoryBank registers;

        public HardwareRegisters()
        {
            this.registers = new ArrayBackedMemoryBank(RegistersConfig);
        }

        public void Halt()
        {
            // Do nothing
        }

        public void Resume()
        {
            // Do nothing
        }

        public IEnumerable<IAddressSegment> AddressSegments => new[] { registers };
    }
}
