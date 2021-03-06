﻿using System;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    /// <summary>
    /// FFFF - IE - Interrupt Enable (R/W)
    /// 
    /// Bit 0: V-Blank Interrupt Enable(INT 40h)  (1=Enable)
    /// Bit 1: LCD STAT Interrupt Enable(INT 48h)  (1=Enable)
    /// Bit 2: Timer Interrupt Enable(INT 50h)  (1=Enable)
    /// Bit 3: Serial Interrupt Enable(INT 58h)  (1=Enable)
    /// Bit 4: Joypad Interrupt Enable(INT 60h)  (1=Enable)
    /// </summary>
    /// <seealso cref="Axh.Retro.GameBoy.Registers.Interfaces.IInterruptEnableRegister" />
    /// <seealso cref="Axh.Retro.GameBoy.Registers.Interfaces.IRegister" />
    public class InterruptEnableRegister : IInterruptEnableRegister, IRegister
    {
        private InterruptFlag _interruptFlag;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptEnableRegister"/> class.
        /// </summary>
        public InterruptEnableRegister()
        {
            _interruptFlag = InterruptFlag.None;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public MemoryBankType Type => MemoryBankType.Peripheral;

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public ushort Address => 0xffff;

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public ushort Length => 1;

        /// <summary>
        /// Reads a byte from this address segment.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public byte ReadByte(ushort address) => Register;

        /// <summary>
        /// Reads a word from this address segment.
        /// If the MMU is implemented correctly, this should never be called on this address segment.
        /// So throw an exception if we try to.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public ushort ReadWord(ushort address)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads bytes from this address segment into a new buffer.
        /// If the MMU is implemented correctly, length will always == 1. So throw an exception otherwise.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public byte[] ReadBytes(ushort address, int length)
        {
            if (length > 1)
            {
                throw new NotSupportedException();
            }
            return new[] { Register };
        }

        /// <summary>
        /// Reads bytes from this address segment into the specified buffer.
        /// If the MMU is implemented correctly, length will always == 1. So throw an exception otherwise.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="buffer">The buffer.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public void ReadBytes(ushort address, byte[] buffer)
        {
            if (buffer.Length > 1)
            {
                throw new NotSupportedException();
            }
            buffer[0] = Register;
        }

        /// <summary>
        /// Writes a byte to this address segment.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        public void WriteByte(ushort address, byte value) => Register = value;

        /// <summary>
        /// Writes a word to this address segment.
        /// If the MMU is implemented correctly, this should never be called on this address segment.
        /// So throw an exception if we try to.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="word">The word.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public void WriteWord(ushort address, ushort word)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes bytes to this address segment.
        /// If the MMU is implemented correctly, length will always == 1. So throw an exception otherwise.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="System.NotSupportedException"></exception>
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
        /// FFFF - IE - Interrupt Enable (R/W)
        /// Bit 0: V-Blank Interrupt Enable(INT 40h)  (1=Enable)
        /// Bit 1: LCD STAT Interrupt Enable(INT 48h)  (1=Enable)
        /// Bit 2: Timer Interrupt Enable(INT 50h)  (1=Enable)
        /// Bit 3: Serial Interrupt Enable(INT 58h)  (1=Enable)
        /// Bit 4: Joypad Interrupt Enable(INT 60h)  (1=Enable)
        /// </summary>
        public byte Register
        {
            get { return (byte) _interruptFlag; }
            set { _interruptFlag = (InterruptFlag) value; }
        }

        /// <summary>
        /// Gets the debug view.
        /// </summary>
        /// <value>
        /// The debug view.
        /// </value>
        public string DebugView => ToString();

        /// <summary>
        /// Vertical blank interrupt enabled.
        /// Bit 0: V-Blank  Interrupt Enable  (INT 40h)  (1=Enable)
        /// The V-Blank interrupt occurs ca. 59.7 times a second on a regular GB and ca. 61.1 times a second on a Super GB (SGB).
        /// This interrupt occurs at the beginning of the V-Blank period (LY=144).
        /// During this period video hardware is not using video ram so it may be freely accessed.This period lasts approximately 1.1 milliseconds.
        /// </summary>
        public bool VerticalBlank => _interruptFlag.HasFlag(InterruptFlag.VerticalBlank);

        /// <summary>
        /// TODO: Implement.
        /// LCD status triggers interrupt enabled.
        /// Bit 1: LCD STAT Interrupt Enable  (INT 48h)  (1=Enable)
        /// There are various reasons for this interrupt to occur as described by the STAT register ($FF40).
        /// One very popular reason is to indicate to the user when the video hardware is about to redraw a given LCD line.
        /// This can be useful for dynamically controlling the SCX/SCY registers ($FF43/$FF42) to perform special video effects.
        /// </summary>
        public bool LcdStatusTriggers => _interruptFlag.HasFlag(InterruptFlag.LcdStatusTriggers);

        /// <summary>
        /// TODO: Implement.
        /// Bit 2: Timer    Interrupt Enable  (INT 50h)  (1=Enable)
        /// Each time when the timer overflows (ie. when TIMA gets bigger than FFh), then an interrupt is requested by setting Bit 2 in the IF Register (FF0F).
        /// When that interrupt is enabled, then the CPU will execute it by calling the timer interrupt vector at 0050h.
        /// </summary>
        public bool TimerOverflow => _interruptFlag.HasFlag(InterruptFlag.TimerOverflow);

        /// <summary>
        /// Bit 3: Serial   Interrupt Enable  (INT 58h)  (1=Enable)
        /// When the transfer has completed (ie. after sending/receiving 8 bits, if any) then an interrupt is requested by setting Bit 3 of the IF Register (FF0F).
        /// When that interrupt is enabled, then the Serial Interrupt vector at 0058 is called.
        /// </summary>
        public bool SerialLink => _interruptFlag.HasFlag(InterruptFlag.SerialLink);

        /// <summary>
        /// Bit 4: Joypad   Interrupt Enable  (INT 60h)  (1=Enable)
        /// Joypad interrupt is requested when any of the above Input lines changes from High to Low.
        /// Generally this should happen when a key becomes pressed (provided that the button/direction key is enabled by above Bit4/5).
        /// However, because of switch bounce, one or more High to Low transitions are usually produced both when pressing or releasing a key.
        /// 
        /// TODO: Maybe we shouldn't waste cycles on a full implementation of this. It should only interrupt when the CPU is in STOP state. Ref:
        /// Using the Joypad Interrupt
        /// It's more or less useless for programmers, even when selecting both buttons and direction keys simultaneously it still cannot recognize all keystrokes,
        /// because in that case a bit might be already held low by a button key, and pressing the corresponding direction key would thus cause no difference.
        /// The only meaningful purpose of the keystroke interrupt would be to terminate STOP (low power) standby state.
        /// Also, the joypad interrupt does not appear to work with CGB and GBA hardware(the STOP function can be still terminated by joypad keystrokes though).
        /// </summary>
        public bool JoyPadPress => _interruptFlag.HasFlag(InterruptFlag.JoyPadPress);

        /// <summary>
        /// Gets a value indication whether interrupts are enabled for the specific interrupt flag.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <returns></returns>
        public bool InterruptEnabled(InterruptFlag flag) => _interruptFlag.HasFlag(flag);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
            =>
                $"{Name} ({Address}) = {Register}\nVerticalBlank: {VerticalBlank}\nLcdStatusTriggers: {LcdStatusTriggers}\nTimerOverflow: {TimerOverflow}\nSerialLink: {SerialLink}\nJoyPadPress: {JoyPadPress}";
    }
}