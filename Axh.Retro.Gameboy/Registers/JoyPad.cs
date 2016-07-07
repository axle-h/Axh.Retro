using System;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    public class JoyPad : IJoyPadRegister
    {
        private readonly IInterruptFlagsRegister _interruptFlagsRegister;
        private bool _a;
        private bool _b;
        private bool _down;
        private bool _left;

        private MatrixColumn _matrixColumn;
        private bool _right;
        private bool _select;
        private bool _start;

        private bool _up;

        public JoyPad(IInterruptFlagsRegister interruptFlagsRegister)
        {
            _interruptFlagsRegister = interruptFlagsRegister;
            _matrixColumn = MatrixColumn.None;
        }

        public bool Up
        {
            get { return _up; }
            set
            {
                _up = value;
                _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Down
        {
            get { return _down; }
            set
            {
                _down = value;
                _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Left
        {
            get { return _left; }
            set
            {
                _left = value;
                _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Right
        {
            get { return _right; }
            set
            {
                _right = value;
                _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool A
        {
            get { return _a; }
            set
            {
                _a = value;
                _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool B
        {
            get { return _b; }
            set
            {
                _b = value;
                _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Select
        {
            get { return _select; }
            set
            {
                _select = value;
                _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public bool Start
        {
            get { return _start; }
            set
            {
                _start = value;
                _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        public ushort Address => 0xff00;

        public string Name => "Joypad Port (JOYPAD R/W)";

        /// <summary>
        /// Bit 7 - Not used
        /// Bit 6 - Not used
        /// Bit 5 - P15 out port
        /// Bit 4 - P14 out port
        /// Bit 3 - P13 in port
        /// Bit 2 - P12 in port
        /// Bit 1 - P11 in port
        /// Bit 0 - P10 in port
        /// P14        P15
        /// |          |
        /// P10-------O-Right----O-A
        /// |          |
        /// P11-------O-Left-----O-B
        /// |          |
        /// P12-------O-Up-------O-Select
        /// |          |
        /// P13-------O-Down-----O-Start
        /// |          |
        /// To read a button you must set P14 or P15 to select the column
        /// Then read P10 - P13 to select the row
        /// </summary>
        public byte Register
        {
            get
            {
                switch (_matrixColumn)
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
                    _matrixColumn = MatrixColumn.P14;
                    return;
                }

                if ((value & 0x20) > 0)
                {
                    _matrixColumn = MatrixColumn.P15;
                    return;
                }

                _matrixColumn = MatrixColumn.None;
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