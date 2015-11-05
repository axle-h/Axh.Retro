namespace Axh.Retro.GameBoy.Peripherals
{
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Contracts.Peripherals;

    public class GameBoyRegisters : IMemoryMappedPeripheral, IGameBoyRegisters
    {
        public GameBoyRegisters(IHardwareRegisters hardwareRegisters, IInterruptRegister interruptRegister)
        {
            this.HardwareRegisters = hardwareRegisters;
            this.InterruptRegister = interruptRegister;
        }

        public void Halt()
        {
            // Do nothing
        }

        public void Resume()
        {
            // Do nothing
        }

        public IEnumerable<IAddressSegment> AddressSegments => new IAddressSegment[] { HardwareRegisters, InterruptRegister };

        public IHardwareRegisters HardwareRegisters { get; }

        public IInterruptRegister InterruptRegister { get; }
    }
}
