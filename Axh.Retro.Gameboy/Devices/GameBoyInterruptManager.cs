namespace Axh.Retro.GameBoy.Devices
{
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public class GameBoyInterruptManager : IGameBoyInterruptManager
    {
        private const ushort VerticalBlankAddress = 0x0040;
        private const ushort LcdStatusTriggersAddress = 0x0048;
        private const ushort TimerOverflowAddress = 0x0050;
        private const ushort SerialLinkAddress = 0x0058;
        private const ushort JoyPadPressAddress = 0x0060;

        private readonly IInterruptManager interruptManager;

        private readonly IInterruptEnableRegister interruptEnableRegister;

        public GameBoyInterruptManager(IInterruptManager interruptManager, IInterruptEnableRegister interruptEnableRegister)
        {
            this.interruptManager = interruptManager;
            this.interruptEnableRegister = interruptEnableRegister;
        }

        public void VerticalBlank()
        {
            if (!this.interruptEnableRegister.VerticalBlank)
            {
                return;
            }
            this.interruptManager.Interrupt(VerticalBlankAddress);
        }

        public void LcdStatusTriggers()
        {
            if (!this.interruptEnableRegister.LcdStatusTriggers)
            {
                return;
            }
            this.interruptManager.Interrupt(LcdStatusTriggersAddress);
        }

        public void TimerOverflow()
        {
            if (!this.interruptEnableRegister.TimerOverflow)
            {
                return;
            }
            this.interruptManager.Interrupt(TimerOverflowAddress);
        }

        public void SerialLink()
        {
            if (!this.interruptEnableRegister.SerialLink)
            {
                return;
            }
            this.interruptManager.Interrupt(SerialLinkAddress);
        }

        public void JoyPadPress()
        {
            if (!this.interruptEnableRegister.JoyPadPress)
            {
                return;
            }
            this.interruptManager.Interrupt(JoyPadPressAddress);
        }


    }
}
