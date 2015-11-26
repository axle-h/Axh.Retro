namespace Axh.Retro.GameBoy.Registers
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.GameBoy.Registers.Interfaces;
    using Axh.Retro.GameBoy.Util;

    public class InterruptEnableRegister : IInterruptEnableRegister
    {
        private InterruptEnable interruptEnable;

        public InterruptEnableRegister()
        {
            this.interruptEnable = InterruptEnable.None;
        }

        public MemoryBankType Type => MemoryBankType.RandomAccessMemory;

        public ushort Address => 0xffff;

        public ushort Length => 1;

        public byte ReadByte(ushort address)
        {
            return this.Register;
        }

        public ushort ReadWord(ushort address)
        {
            throw new NotSupportedException();
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            if (length > 1)
            {
                throw new NotSupportedException();
            }
            return new[] { this.Register };
        }

        public void ReadBytes(ushort address, byte[] buffer)
        {
            if (buffer.Length > 1)
            {
                throw new NotSupportedException();
            }
            buffer[0] = Register;
        }

        public void WriteByte(ushort address, byte value)
        {
            this.Register = value;
        }

        public void WriteWord(ushort address, ushort word)
        {
            throw new NotSupportedException();
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            if (values.Length > 1)
            {
                throw new NotSupportedException();
            }

            this.Register = values[0];
        }

        public string Name => "Interrupt Enable (IE R/W)";

        /// <summary>
        /// FFFF - IE - Interrupt Enable (R/W)
        /// Bit 0: V-Blank Interrupt Enable(INT 40h)  (1=Enable)
        /// Bit 1: LCD STAT Interrupt Enable(INT 48h)  (1=Enable)
        /// Bit 2: Timer Interrupt Enable(INT 50h)  (1=Enable)
        /// Bit 3: Serial Interrupt Enable(INT 58h)  (1=Enable)
        /// Bit 4: Joypad Interrupt Enable(INT 60h)  (1=Enable)
        /// </summary>
        public byte Register
        {
            get
            {
                return (byte)this.interruptEnable;
            }
            set
            {
                this.interruptEnable = (InterruptEnable)value;
            }
        }

        [Flags]
        private enum InterruptEnable : byte
        {
            None = 0,
            VerticalBlank = 0x01,
            LcdStatusTriggers = 0x02,
            TimerOverflow = 0x04,
            SerialLink = 0x08,
            JoyPadPress = 0x10
        }

        public string DebugView => this.ToString();

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}\nVerticalBlank: {VerticalBlank}\nLcdStatusTriggers: {LcdStatusTriggers}\nTimerOverflow: {TimerOverflow}\nSerialLink: {SerialLink}\nJoyPadPress: {JoyPadPress}";
        }

        public bool VerticalBlank => this.interruptEnable.HasFlag(InterruptEnable.VerticalBlank);

        public bool LcdStatusTriggers => this.interruptEnable.HasFlag(InterruptEnable.LcdStatusTriggers);

        public bool TimerOverflow => this.interruptEnable.HasFlag(InterruptEnable.TimerOverflow);

        public bool SerialLink => this.interruptEnable.HasFlag(InterruptEnable.SerialLink);

        public bool JoyPadPress => this.interruptEnable.HasFlag(InterruptEnable.JoyPadPress);

        public void DisableVerticalBlank()
        {
            this.interruptEnable = ~InterruptEnable.VerticalBlank;
        }

        public void DisableLcdStatusTriggers()
        {
            this.interruptEnable &= ~InterruptEnable.LcdStatusTriggers;
        }

        public void DisableTimerOverflow()
        {
            this.interruptEnable &= ~InterruptEnable.TimerOverflow;
        }

        public void DisableSerialLink()
        {
            this.interruptEnable &= ~InterruptEnable.SerialLink;
        }

        public void DisableJoyPadPress()
        {
            this.interruptEnable &= ~InterruptEnable.JoyPadPress;
        }
    }
}
