namespace Axh.Retro.GameBoy.Devices
{
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;

    public class GameBoyInterruptManager : IGameBoyInterruptManager
    {
        private const ushort VerticalBlankAddress = 0x0040;
        private const ushort LcdStatusTriggersAddress = 0x0048;
        private const ushort TimerOverflowAddress = 0x0050;
        private const ushort SerialLinkAddress = 0x0058;
        private const ushort JoyPadPressAddress = 0x0060;

        private readonly IInterruptManager interruptManager;

        public GameBoyInterruptManager(IInterruptManager interruptManager)
        {
            this.interruptManager = interruptManager;
        }

        public void VerticalBlank()
        {
            this.interruptManager.Interrupt(VerticalBlankAddress);
        }

        public void LcdStatusTriggers()
        {
            this.interruptManager.Interrupt(LcdStatusTriggersAddress);
        }

        public void TimerOverflow()
        {
            this.interruptManager.Interrupt(TimerOverflowAddress);
        }

        public void SerialLink()
        {
            this.interruptManager.Interrupt(SerialLinkAddress);
        }

        public void JoyPadPress()
        {
            this.interruptManager.Interrupt(JoyPadPressAddress);
        }


    }
}
