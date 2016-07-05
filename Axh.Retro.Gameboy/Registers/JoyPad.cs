using System;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    public class JoyPad : IJoyPadRegister
    {
        private readonly IInterruptFlagsRegister interruptFlagsRegister;
        private bool a;
        private bool b;
        private bool down;
        private bool left;

        private MatrixColumn matrixColumn;
        private bool right;
        private bool select;
        private bool start;

        private bool up;

        public JoyPad(IInterruptFlagsRegister interruptFlagsRegister)
        {
            this.interruptFlagsRegister = interruptFlagsRegister;
            matrixColumn = MatrixColumn.None;
        }

        public bool Up
        {
            get { return up; }
            set
            {
                up = value;
                interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Down
        {
            get { return down; }
            set
            {
                down = value;
                interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Left
        {
            get { return left; }
            set
            {
                left = value;
                interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Right
        {
            get { return right; }
            set
            {
                right = value;
                interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool A
        {
            get { return a; }
            set
            {
                a = value;
                interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool B
        {
            get { return b; }
            set
            {
                b = value;
                interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Select
        {
            get { return select; }
            set
            {
                select = value;
                interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Start
        {
            get { return start; }
            set
            {
                start = value;
                interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public ushort Address => 0xff00;

        public string Name => "Joypad Port (JOYPAD R/W)";

        /// <summary>
        ///     Bit 7 - Not used
        ///     Bit 6 - Not used
        ///     Bit 5 - P15 out port
        ///     Bit 4 - P14 out port
        ///     Bit 3 - P13 in port
        ///     Bit 2 - P12 in port
        ///     Bit 1 - P11 in port
        ///     Bit 0 - P10 in port
        ///     P14        P15
        ///     |          |
        ///     P10-------O-Right----O-A
        ///     |          |
        ///     P11-------O-Left-----O-B
        ///     |          |
        ///     P12-------O-Up-------O-Select
        ///     |          |
        ///     P13-------O-Down-----O-Start
        ///     |          |
        ///     To read a button you must set P14 or P15 to select the column
        ///     Then read P10 - P13 to select the row
        /// </summary>
        public byte Register
        {
            get
            {
                switch (matrixColumn)
                {
                    case MatrixColumn.None:
                        return 0xff;
                    case MatrixColumn.P14:
                        return GetRegister(Right, Left, Up, Down);
                    case MatrixColumn.P15:
                        return GetRegister(A, B, Select, Start);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                // Check which matrix column is set.
                // No idea what happens when both are set...
                if ((value & 0x10) > 0)
                {
                    matrixColumn = MatrixColumn.P14;
                    return;
                }

                if ((value & 0x20) > 0)
                {
                    matrixColumn = MatrixColumn.P15;
                    return;
                }

                matrixColumn = MatrixColumn.None;
            }
        }

        public string DebugView { get; }

        private static byte GetRegister(bool p10, bool p11, bool p12, bool p13)
        {
            var value = 0xf0;
            if (p13)
            {
                value |= 0x8;
            }

            if (p12)
            {
                value |= 0x4;
            }

            if (p11)
            {
                value |= 0x2;
            }

            if (p10)
            {
                value |= 0x1;
            }

            return (byte) ~value;
        }

        private enum MatrixColumn
        {
            None = 0,
            P14 = 1,
            P15 = 2
        }
    }
}