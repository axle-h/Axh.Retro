using System;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    public class InterruptEnableRegister : IInterruptEnableRegister
    {
        private InterruptFlag interruptFlag;

        public InterruptEnableRegister()
        {
            interruptFlag = InterruptFlag.None;
        }

        public MemoryBankType Type => MemoryBankType.RandomAccessMemory;

        public ushort Address => 0xffff;

        public ushort Length => 1;

        public byte ReadByte(ushort address)
        {
            return Register;
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
            return new[] {Register};
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
            Register = value;
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

            Register = values[0];
        }

        public string Name => "Interrupt Enable (IE R/W)";

        /// <summary>
        ///     FFFF - IE - Interrupt Enable (R/W)
        ///     Bit 0: V-Blank Interrupt Enable(INT 40h)  (1=Enable)
        ///     Bit 1: LCD STAT Interrupt Enable(INT 48h)  (1=Enable)
        ///     Bit 2: Timer Interrupt Enable(INT 50h)  (1=Enable)
        ///     Bit 3: Serial Interrupt Enable(INT 58h)  (1=Enable)
        ///     Bit 4: Joypad Interrupt Enable(INT 60h)  (1=Enable)
        /// </summary>
        public byte Register
        {
            get { return (byte) interruptFlag; }
            set { interruptFlag = (InterruptFlag) value; }
        }

        public string DebugView => ToString();

        public bool VerticalBlank => interruptFlag.HasFlag(InterruptFlag.VerticalBlank);

        public bool LcdStatusTriggers => interruptFlag.HasFlag(InterruptFlag.LcdStatusTriggers);

        public bool TimerOverflow => interruptFlag.HasFlag(InterruptFlag.TimerOverflow);

        public bool SerialLink => interruptFlag.HasFlag(InterruptFlag.SerialLink);

        public bool JoyPadPress => interruptFlag.HasFlag(InterruptFlag.JoyPadPress);

        public bool InterruptEnabled(InterruptFlag flag) => interruptFlag.HasFlag(flag);

        public override string ToString()
        {
            return
                $"{Name} ({Address}) = {Register}\nVerticalBlank: {VerticalBlank}\nLcdStatusTriggers: {LcdStatusTriggers}\nTimerOverflow: {TimerOverflow}\nSerialLink: {SerialLink}\nJoyPadPress: {JoyPadPress}";
        }
    }
}