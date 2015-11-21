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
            var gameBoyInterruptManager = new GameBoyInterruptManager(interruptManager);

            // Build devices
            var renderhandler = this.renderHandlerFactory.GetRenderHandler();
            var joyPad = new JoyPad(gameBoyInterruptManager);
            var serialPort = new SyncSerialPort(gameBoyInterruptManager);
            var dividerRegister = new LazyDividerRegister();
            var lcdControlRegister = new LcdControlRegister();
            var registers = new IRegister[] { joyPad, serialPort, dividerRegister, lcdControlRegister };

            // Build registers
            var hardwareRegisters = new HardwareRegisters(registers, joyPad, serialPort);
            var interruptRegister = new InterruptEnableRegister();

            // Build framebuffer
            var frameBuffer = new Gpu(interruptManager, hardwareRegisters, renderhandler);

            // Build peripherals
            var hardwareRegistersPeripheral = new GameBoyMemoryMappedIO(hardwareRegisters, interruptRegister, frameBuffer);
            
            return new IMemoryMappedPeripheral[] { hardwareRegistersPeripheral };
        }
    }
}
