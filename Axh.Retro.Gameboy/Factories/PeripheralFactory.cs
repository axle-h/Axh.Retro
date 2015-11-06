namespace Axh.Retro.GameBoy.Factories
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Contracts.Factories;
    using Axh.Retro.GameBoy.Devices;
    using Axh.Retro.GameBoy.Peripherals;

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
            var joyPad = new JoyPad(gameBoyInterruptManager);
            var serialPort = new SyncSerialPort(gameBoyInterruptManager);
            var dividerRegister = new DividerRegister();
            var renderhandler = this.renderHandlerFactory.GetRenderHandler();

            // Build registers
            var hardwareRegisters = new HardwareRegisters(joyPad, serialPort, dividerRegister);
            var interruptRegister = new InterruptRegister();

            // Build peripherals
            var hardwareRegistersPeripheral = new GameBoyRegisters(hardwareRegisters, interruptRegister);
            var graphicsFrameBuffer = new GraphicsFrameBuffer(interruptManager, hardwareRegisters, renderhandler);

            return new IMemoryMappedPeripheral[] { hardwareRegistersPeripheral, graphicsFrameBuffer };
        }
    }
}
