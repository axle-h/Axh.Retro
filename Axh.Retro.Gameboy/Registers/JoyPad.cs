using System;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    /// <summary>
    /// The GameBoy joypad register.
    /// </summary>
    /// <seealso cref="Axh.Retro.GameBoy.Registers.Interfaces.IJoyPadRegister" />
    public class JoyPad : IJoyPadRegister
    {
        private readonly IInterruptFlagsRegister _interruptFlagsRegister;
        private MatrixColumn _matrixColumn;
        private JoyPadButton _buttons;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoyPad"/> class.
        /// </summary>
        /// <param name="interruptFlagsRegister">The interrupt flags register.</param>
        public JoyPad(IInterruptFlagsRegister interruptFlagsRegister)
        {
            _interruptFlagsRegister = interruptFlagsRegister;
            _matrixColumn = MatrixColumn.None;
            _buttons = JoyPadButton.None;
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public ushort Address => 0xff00;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
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
                        return GetRegister(Buttons.HasFlag(JoyPadButton.Right),
                                           Buttons.HasFlag(JoyPadButton.Left),
                                           Buttons.HasFlag(JoyPadButton.Up),
                                           Buttons.HasFlag(JoyPadButton.Down));
                    case MatrixColumn.P15:
                        return GetRegister(Buttons.HasFlag(JoyPadButton.A),
                                           Buttons.HasFlag(JoyPadButton.B),
                                           Buttons.HasFlag(JoyPadButton.Select),
                                           Buttons.HasFlag(JoyPadButton.Start));
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

        /// <summary>
        /// Gets the debug view.
        /// </summary>
        /// <value>
        /// The debug view.
        /// </value>
        public string DebugView => ToString();

        /// <summary>
        /// Gets the register.
        /// </summary>
        /// <param name="p10">if set to <c>true</c> [P10].</param>
        /// <param name="p11">if set to <c>true</c> [P11].</param>
        /// <param name="p12">if set to <c>true</c> [P12].</param>
        /// <param name="p13">if set to <c>true</c> [P13].</param>
        /// <returns></returns>
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

        /// <summary>
        /// The column in the joypad matrix to read.
        /// </summary>
        private enum MatrixColumn
        {
            /// <summary>
            /// No column. Value is unset.
            /// </summary>
            None = 0,

            /// <summary>
            /// Column P14.
            /// </summary>
            P14 = 1,

            /// <summary>
            /// Column P15.
            /// </summary>
            P15 = 2
        }

        /// <summary>
        /// Gets or sets the buttons.
        /// Note: <see cref="JoyPadButton"/> is a flags register.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        public JoyPadButton Buttons {
            get { return _buttons; }
            set
            {
                _buttons = value;
                _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.JoyPadPress);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => $"{Name} ({Address}) = {Register} ({_buttons})";
    }
}