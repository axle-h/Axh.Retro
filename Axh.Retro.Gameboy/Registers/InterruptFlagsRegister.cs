namespace Axh.Retro.GameBoy.Registers
{
    using System;

    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public class InterruptFlagsRegister : IRegister
    {
        private readonly Action<InterruptFlag> updateInterruptsAction;

        private InterruptFlag interruptFlag;

        public InterruptFlagsRegister(Action<InterruptFlag> updateInterruptsAction)
        {
            this.updateInterruptsAction = updateInterruptsAction;
            this.interruptFlag = InterruptFlag.None;
        }

        public ushort Address => 0xff0f;

        public string Name => "Interrupt Flag (IF R/W)";

        /// <summary>
        /// FF0F - IF - Interrupt Flags (R/W)
        /// Bit 0: V-Blank Interrupt Request(INT 40h)  (1=Requested)
        /// Bit 1: LCD STAT Interrupt Request(INT 48h)  (1=Requested)
        /// Bit 2: Timer Interrupt Request(INT 50h)  (1=Requested)
        /// Bit 3: Serial Interrupt Request(INT 58h)  (1=Requested)
        /// Bit 4: Joypad Interrupt Request(INT 60h)  (1=Requested)
        /// </summary>
        public byte Register
        {
            get
            {
                return (byte)this.interruptFlag;
            }
            set
            {
                var newInterruptFlag = (InterruptFlag)value;
                var changedFlags = interruptFlag ^ newInterruptFlag;

                if (changedFlags != InterruptFlag.None)
                {
                    updateInterruptsAction(changedFlags);
                }

                this.interruptFlag = newInterruptFlag;
            }
        }

        public string DebugView => this.ToString();

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}";
        }
    }
}