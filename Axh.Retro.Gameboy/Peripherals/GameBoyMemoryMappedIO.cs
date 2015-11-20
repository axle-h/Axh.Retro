namespace Axh.Retro.GameBoy.Peripherals
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Contracts.Peripherals;

    public class GameBoyMemoryMappedIO : IMemoryMappedPeripheral, IGameBoyMemoryMappedIO
    {
        public GameBoyMemoryMappedIO(IHardwareRegisters hardwareRegisters, IInterruptRegister interruptRegister, IFrameBuffer frameBuffer)
        {
            this.HardwareRegisters = hardwareRegisters;
            this.InterruptRegister = interruptRegister;
            this.FrameBuffer = frameBuffer;
        }

        public void Halt()
        {
            // Do nothing
        }

        public void Resume()
        {
            // Do nothing
        }

        public IEnumerable<IAddressSegment> AddressSegments => new IAddressSegment[] { HardwareRegisters, InterruptRegister }.Concat(FrameBuffer.AddressSegments).ToArray();

        public IHardwareRegisters HardwareRegisters { get; }

        public IInterruptRegister InterruptRegister { get; }

        public IFrameBuffer FrameBuffer { get; }
    }
}
