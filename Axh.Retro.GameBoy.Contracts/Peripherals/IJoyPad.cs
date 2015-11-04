namespace Axh.Retro.GameBoy.Contracts.Peripherals
{
    public interface IJoyPad
    {
        bool Up { get; set; }

        bool Down { get; set; }

        bool Left { get; set; }

        bool Right { get; set; }

        bool A { get; set; }

        bool B { get; set; }

        bool Select { get; set; }

        bool Start { get; set; }

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
        byte Register { get; set; }
    }
}
