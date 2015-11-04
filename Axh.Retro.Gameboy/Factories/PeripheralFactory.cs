namespace Axh.Retro.GameBoy.Factories
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Contracts.Factories;
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
            var interruptRegister = new InterruptRegister();

            var joyPad = new JoyPad();
            var hardwareRegisters = new HardwareRegisters(joyPad);
            var hardwareRegistersPeripheral = new HardwareRegistersPeripheral(hardwareRegisters);

            var renderhandler = this.renderHandlerFactory.GetIRenderHandler();
            var graphicsFrameBuffer = new GraphicsFrameBuffer(interruptManager, hardwareRegisters, renderhandler);
            return new IMemoryMappedPeripheral[] { hardwareRegistersPeripheral, graphicsFrameBuffer, interruptRegister };
        }
    }
}
