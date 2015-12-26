namespace Axh.Retro.GameBoy.Devices
{
    using System.Threading;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Registers;
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

        private readonly InterruptFlagsRegister interruptFlagsRegister;

        private InterruptFlag delayedInterrupts;

        public GameBoyInterruptManager(IInterruptManager interruptManager, IInterruptEnableRegister interruptEnableRegister)
        {
            this.interruptManager = interruptManager;
            this.interruptEnableRegister = interruptEnableRegister;
            this.interruptFlagsRegister = new InterruptFlagsRegister(UpdateInterrupts);
        }
        
        public IRegister InterruptFlagsRegister => interruptFlagsRegister;

        public void UpdateInterrupts(InterruptFlag interrupts)
        {
            // Try delayed interrupts again.
            interrupts |= delayedInterrupts;

            if (interrupts.HasFlag(InterruptFlag.VerticalBlank))
            {
                if (this.interruptEnableRegister.VerticalBlank)
                {
                    interrupts &= ~InterruptFlag.VerticalBlank;
                    this.interruptManager.Interrupt(VerticalBlankAddress);
                }
            }

            if (interrupts.HasFlag(InterruptFlag.LcdStatusTriggers))
            {
                if (this.interruptEnableRegister.LcdStatusTriggers)
                {
                    interrupts &= ~InterruptFlag.LcdStatusTriggers;
                    this.interruptManager.Interrupt(LcdStatusTriggersAddress);
                }
            }

            if (interrupts.HasFlag(InterruptFlag.TimerOverflow))
            {
                if (this.interruptEnableRegister.TimerOverflow)
                {
                    interrupts &= ~InterruptFlag.TimerOverflow;
                    this.interruptManager.Interrupt(TimerOverflowAddress);
                }
            }

            if (interrupts.HasFlag(InterruptFlag.SerialLink))
            {
                if (this.interruptEnableRegister.SerialLink)
                {
                    interrupts &= ~InterruptFlag.SerialLink;
                    this.interruptManager.Interrupt(SerialLinkAddress);
                }
            }

            if (interrupts.HasFlag(InterruptFlag.JoyPadPress))
            {
                if (this.interruptEnableRegister.JoyPadPress)
                {
                    interrupts &= ~InterruptFlag.JoyPadPress;
                    this.interruptManager.Interrupt(JoyPadPressAddress);
                }
            }

            delayedInterrupts = interrupts;
        }
    }
}
