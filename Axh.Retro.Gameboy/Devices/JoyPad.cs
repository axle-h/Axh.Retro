namespace Axh.Retro.GameBoy.Devices
{
    using System;
    
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;

    public class JoyPad : ICoreJoyPad
    {
        private readonly IGameBoyInterruptManager gameBoyInterruptManager;

        private MatrixColumn matrixColumn;

        private bool up;
        private bool down;
        private bool left;
        private bool right;
        private bool a;
        private bool b;
        private bool select;
        private bool start;

        public JoyPad(IGameBoyInterruptManager gameBoyInterruptManager)
        {
            this.gameBoyInterruptManager = gameBoyInterruptManager;
            this.matrixColumn = MatrixColumn.None;
        }

        public bool Up
        {
            get
            {
                return this.up;
            }
            set
            {
                this.up = value;
                this.gameBoyInterruptManager.JoyPadPress();
            }
        }

        public bool Down
        {
            get
            {
                return this.down;
            }
            set
            {
                this.down = value;
                this.gameBoyInterruptManager.JoyPadPress();
            }
        }

        public bool Left
        {
            get
            {
                return this.left;
            }
            set
            {
                this.left = value;
                this.gameBoyInterruptManager.JoyPadPress();
            }
        }

        public bool Right
        {
            get
            {
                return this.right;
            }
            set
            {
                this.right = value;
                this.gameBoyInterruptManager.JoyPadPress();
            }
        }

        public bool A
        {
            get
            {
                return this.a;
            }
            set
            {
                this.a = value;
                this.gameBoyInterruptManager.JoyPadPress();
            }
        }

        public bool B
        {
            get
            {
                return this.b;
            }
            set
            {
                this.b = value;
                this.gameBoyInterruptManager.JoyPadPress();
            }
        }

        public bool Select
        {
            get
            {
                return this.select;
            }
            set
            {
                this.select = value;
                this.gameBoyInterruptManager.JoyPadPress();
            }
        }

        public bool Start
        {
            get
            {
                return this.start;
            }
            set
            {
                this.start = value;
                this.gameBoyInterruptManager.JoyPadPress();
            }
        }

        public ushort Address => 0xff00;

        public string Name => "Joypad Port (JOYPAD R/W)";

        /// <summary>
        ///  Bit 7 - Not used
        ///  Bit 6 - Not used
        ///  Bit 5 - P15 out port
        ///  Bit 4 - P14 out port
        ///  Bit 3 - P13 in port
        ///  Bit 2 - P12 in port
        ///  Bit 1 - P11 in port
        ///  Bit 0 - P10 in port
        /// 
        ///           P14        P15
        ///            |          |
        ///  P10-------O-Right----O-A
        ///            |          |
        ///  P11-------O-Left-----O-B
        ///            |          |
        ///  P12-------O-Up-------O-Select
        ///            |          |
        ///  P13-------O-Down-----O-Start
        ///            |          |
        /// 
        /// To read a button you must set P14 or P15 to select the column
        /// Then read P10 - P13 to select the row
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
                    this.matrixColumn = MatrixColumn.P14;
                    return;
                }

                if ((value & 0x20) > 0)
                {
                    this.matrixColumn = MatrixColumn.P15;
                    return;
                }

                this.matrixColumn = MatrixColumn.None;
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

            return (byte)~value;
        }

        private enum MatrixColumn
        {
            None = 0,
            P14 = 1,
            P15 = 2
        }
    }
}
