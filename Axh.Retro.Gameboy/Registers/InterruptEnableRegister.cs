namespace Axh.Retro.GameBoy.Registers
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.GameBoy.Registers.Interfaces;
    using Axh.Retro.GameBoy.Util;

    public class InterruptEnableRegister : IInterruptEnableRegister
    {
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
                return RegisterHelpers.GetRegister(false, false, false, JoyPadPress, SerialLink, TimerOverflow, LcdStatusTriggers, VerticalBlank);
            }
            set
            {
                JoyPadPress = RegisterHelpers.GetBit(value, 4);
                SerialLink = RegisterHelpers.GetBit(value, 3);
                TimerOverflow = RegisterHelpers.GetBit(value, 2);
                LcdStatusTriggers = RegisterHelpers.GetBit(value, 1);
                VerticalBlank = RegisterHelpers.GetBit(value, 0);
            }
        }

        public string DebugView => this.ToString();

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}\nVerticalBlank: {VerticalBlank}\nLcdStatusTriggers: {LcdStatusTriggers}\nTimerOverflow: {TimerOverflow}\nSerialLink: {SerialLink}\nJoyPadPress: {JoyPadPress}";
        }

        public bool VerticalBlank { get; private set; }

        public bool LcdStatusTriggers { get; private set; }

        public bool TimerOverflow { get; private set; }

        public bool SerialLink { get; private set; }

        public bool JoyPadPress { get; private set; }
    }
}
