using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    public class InterruptFlagsRegister : IInterruptFlagsRegister
    {
        private const ushort VerticalBlankAddress = 0x0040;
        private const ushort LcdStatusTriggersAddress = 0x0048;
        private const ushort TimerOverflowAddress = 0x0050;
        private const ushort SerialLinkAddress = 0x0058;
        private const ushort JoyPadPressAddress = 0x0060;

        private readonly IInterruptEnableRegister interruptEnableRegister;

        private readonly IInterruptManager interruptManager;

        private InterruptFlag interruptFlag;

        public InterruptFlagsRegister(IInterruptManager interruptManager,
                                      IInterruptEnableRegister interruptEnableRegister)
        {
            this.interruptManager = interruptManager;
            this.interruptEnableRegister = interruptEnableRegister;
            interruptFlag = InterruptFlag.None;
        }

        public ushort Address => 0xff0f;

        public string Name => "Interrupt Flag (IF R/W)";

        /// <summary>
        ///     FF0F - IF - Interrupt Flags (R/W)
        ///     Bit 0: V-Blank Interrupt Request(INT 40h)  (1=Requested)
        ///     Bit 1: LCD STAT Interrupt Request(INT 48h)  (1=Requested)
        ///     Bit 2: Timer Interrupt Request(INT 50h)  (1=Requested)
        ///     Bit 3: Serial Interrupt Request(INT 58h)  (1=Requested)
        ///     Bit 4: Joypad Interrupt Request(INT 60h)  (1=Requested)
        /// </summary>
        public byte Register
        {
            get { return (byte) interruptFlag; }
            set
            {
                var newInterruptFlag = (InterruptFlag) value;
                var changedFlags = interruptFlag ^ newInterruptFlag;

                if (changedFlags != InterruptFlag.None)
                {
                    UpdateInterrupts(changedFlags);
                }

                interruptFlag = newInterruptFlag;
            }
        }

        public string DebugView => ToString();

        public void UpdateInterrupts(InterruptFlag interrupts)
        {
            // Combine with delayed interrupts.
            interruptFlag |= interrupts;

            if (interruptFlag == InterruptFlag.None)
            {
                return;
            }

            if (!interruptManager.InterruptsEnabled)
            {
                // Interrupts disabled.
                return;
            }

            if (CheckInterrupt(InterruptFlag.VerticalBlank, VerticalBlankAddress))
            {
                return;
            }

            if (CheckInterrupt(InterruptFlag.LcdStatusTriggers, LcdStatusTriggersAddress))
            {
                return;
            }

            if (CheckInterrupt(InterruptFlag.TimerOverflow, TimerOverflowAddress))
            {
                return;
            }

            if (CheckInterrupt(InterruptFlag.SerialLink, SerialLinkAddress))
            {
                return;
            }

            CheckInterrupt(InterruptFlag.JoyPadPress, JoyPadPressAddress);
        }

        public override string ToString() => $"{Name} ({Address}) = {Register}";

        private bool CheckInterrupt(InterruptFlag interrupt, ushort address)
        {
            if (!interruptFlag.HasFlag(interrupt) || !interruptEnableRegister.InterruptEnabled(interrupt))
            {
                // Interrupt flag is not set or enabled.
                return false;
            }

            // Do interrupt.
            interruptManager.Interrupt(address);

            // Clear the interrupt flag.
            interruptFlag &= ~interrupt;
            return true;
        }
    }
}