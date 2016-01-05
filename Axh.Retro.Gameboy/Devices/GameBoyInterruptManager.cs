namespace Axh.Retro.GameBoy.Devices
{
    using System.Diagnostics;
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

        public bool UpdateInterrupts(InterruptFlag interrupts)
        {
            // Combine with delayed interrupts.
            delayedInterrupts |= interrupts;

            if (delayedInterrupts == InterruptFlag.None)
            {
                return false;
            }

            if (!interruptManager.InterruptsEnabled)
            {
                return false;
            }

            if (CheckInterrupt(InterruptFlag.VerticalBlank, VerticalBlankAddress))
            {
                return false;
            }

            return CheckInterrupt(InterruptFlag.VerticalBlank, VerticalBlankAddress) ||
                   CheckInterrupt(InterruptFlag.LcdStatusTriggers, LcdStatusTriggersAddress) ||
                   CheckInterrupt(InterruptFlag.TimerOverflow, TimerOverflowAddress) ||
                   CheckInterrupt(InterruptFlag.SerialLink, SerialLinkAddress) ||
                   CheckInterrupt(InterruptFlag.JoyPadPress, JoyPadPressAddress);
        }

        private bool CheckInterrupt(InterruptFlag interrupt, ushort address)
        {
            if (!delayedInterrupts.HasFlag(interrupt) || !this.interruptEnableRegister.InterruptEnabled(interrupt))
            {
                return false;
            }
            
            // Do not await this as potentially called from CPU execution thread.
            this.interruptManager.Interrupt(address);
            delayedInterrupts &= ~interrupt;
            return true;
        }
    }
}
