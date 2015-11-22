namespace Axh.Retro.GameBoy.Factories
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Contracts.Factories;
    using Axh.Retro.GameBoy.Devices;
    using Axh.Retro.GameBoy.Registers;
    using Axh.Retro.GameBoy.Peripherals;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public class PeripheralFactory : IPeripheralFactory
    {
        private readonly IRenderHandlerFactory renderHandlerFactory;

        public PeripheralFactory(IRenderHandlerFactory renderHandlerFactory)
        {
            this.renderHandlerFactory = renderHandlerFactory;
        }

        public IEnumerable<IIOPeripheral> GetIOMappedPeripherals(IInterruptManager interruptManager)
        {
            // No IO mapped peripherals on GB
            return Enumerable.Empty<IIOPeripheral>();
        }

        public IEnumerable<IMemoryMappedPeripheral> GetMemoryMappedPeripherals(IInterruptManager interruptManager)
        {
            // TODO: a secondary DI kernel could probably do all this wiring up.
            // Build devices
            var interruptEnableRegister = new InterruptEnableRegister();
            var gameBoyInterruptManager = new GameBoyInterruptManager(interruptManager, interruptEnableRegister);
            var renderhandler = this.renderHandlerFactory.GetRenderHandler();
            var joyPad = new JoyPad(gameBoyInterruptManager);
            var serialPort = new SyncSerialPort(gameBoyInterruptManager);
            var dividerRegister = new LazyDividerRegister();
            var lcdControlRegister = new LcdControlRegister();
            var registers = new[] { joyPad, serialPort, serialPort.SerialData, dividerRegister, lcdControlRegister };
            var hardwareRegisters = new HardwareRegisters(registers, joyPad, serialPort);
            
            // Build framebuffer
            var frameBuffer = new Gpu(interruptManager, hardwareRegisters, renderhandler);

            // Build peripherals
            var hardwareRegistersPeripheral = new GameBoyMemoryMappedIO(hardwareRegisters, interruptEnableRegister, frameBuffer);
            
            return new IMemoryMappedPeripheral[] { hardwareRegistersPeripheral };
        }
    }
}
