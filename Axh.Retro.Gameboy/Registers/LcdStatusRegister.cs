﻿using Axh.Retro.GameBoy.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    /// <summary>
    /// FF41 - STAT - LCDC Status (R/W)
    /// Bit 6 - LYC=LY Coincidence Interrupt(1=Enable) (Read/Write)
    /// Bit 5 - Mode 2 OAM Interrupt(1=Enable) (Read/Write)
    /// Bit 4 - Mode 1 V-Blank Interrupt(1=Enable) (Read/Write)
    /// Bit 3 - Mode 0 H-Blank Interrupt(1=Enable) (Read/Write)
    /// Bit 2 - Coincidence Flag(0:LYC LY, 1:LYC= LY) (Read Only)
    /// Bit 1-0 - Mode Flag(Mode 0-3, see below) (Read Only)
    /// 0: During H-Blank
    /// 1: During V-Blank
    /// 2: During Searching OAM-RAM
    /// 3: During Transfering Data to LCD Driver
    /// The two lower STAT bits show the current status of the LCD controller.
    /// Mode 0: The LCD controller is in the H-Blank period and
    /// the CPU can access both the display RAM(8000h-9FFFh)
    /// and OAM(FE00h-FE9Fh)
    /// Mode 1: The LCD contoller is in the V-Blank period(or the
    /// display is disabled) and the CPU can access both the
    /// display RAM(8000h-9FFFh) and OAM(FE00h-FE9Fh)
    /// Mode 2: The LCD controller is reading from OAM memory.
    /// The CPU cannot access OAM memory (FE00h-FE9Fh)
    /// during this period.
    /// Mode 3: The LCD controller is reading from both OAM and VRAM,
    /// The CPU cannot access OAM and VRAM during this period.
    /// CGB Mode: Cannot access Palette Data (FF69, FF6B) either.
    /// The following are typical when the display is enabled:
    /// Mode 2  2_____2_____2_____2_____2_____2___________________2____
    /// Mode 3  _33____33____33____33____33____33__________________3___
    /// Mode 0  ___000___000___000___000___000___000________________000
    /// Mode 1  ____________________________________11111111111111_____
    /// The Mode Flag goes through the values 0, 2, and 3 at a cycle of about 109uS. 0 is present about 48.6uS, 2 about
    /// 19uS, and 3 about 41uS. This is interrupted every 16.6ms by the VBlank (1). The mode flag stays set at 1 for about
    /// 1.08 ms.
    /// Mode 0 is present between 201 - 207 clks, 2 about 77 - 83 clks, and 3 about 169 - 175 clks.A complete cycle through
    /// these states takes 456 clks.VBlank lasts 4560 clks.A complete screen refresh occurs every 70224 clks.)
    /// </summary>
    public class LcdStatusRegister : ILcdStatusRegister
    {
        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public ushort Address => 0xff41;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name => "STAT - LCDC Status (R/W)";

        /// <summary>
        /// Gets or sets the raw register value.
        /// </summary>
        /// <value>
        /// The raw register value.
        /// </value>
        public byte Register
        {
            get
            {
                var value = 0;

                value |= (int) GpuMode & 0x3;
                if (CoincidenceFlag)
                {
                    value |= 0x4;
                }

                if (Mode0HBlankInterrupt)
                {
                    value |= 0x8;
                }

                if (Mode1VBlankInterrupt)
                {
                    value |= 0x10;
                }

                if (Mode2OamInterrupt)
                {
                    value |= 0x20;
                }

                if (CoincidenceInterrupt)
                {
                    value |= 0x40;
                }

                return (byte) value;
            }
            set
            {
                GpuMode = (GpuMode) (value & 0x3);
                CoincidenceFlag = (value & 0x4) > 0;
                Mode0HBlankInterrupt = (value & 0x8) > 0;
                Mode1VBlankInterrupt = (value & 0x10) > 0;
                Mode2OamInterrupt = (value & 0x20) > 0;
                CoincidenceInterrupt = (value & 0x40) > 0;
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
        /// Gets or sets the gpu mode.
        /// </summary>
        /// <value>
        /// The gpu mode.
        /// </value>
        public GpuMode GpuMode { get; set; }

        /// <summary>
        /// Coincidence Interrupt (1=Enable) (Read/Write)
        /// </summary>
        /// <value>
        /// <c>true</c> if [coincidence interrupt enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool CoincidenceInterrupt { get; set; }

        /// <summary>
        /// Mode 2 OAM Interrupt (1=Enable) (Read/Write)
        /// </summary>
        /// <value>
        /// <c>true</c> if [mode 2 oam interrupt enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool Mode2OamInterrupt { get; set; }

        /// <summary>
        /// Mode 1 V-Blank Interrupt (1=Enable) (Read/Write)
        /// </summary>
        /// <value>
        /// <c>true</c> if [mode 1 v-blank interrupt enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool Mode1VBlankInterrupt { get; set; }

        /// <summary>
        /// Mode 0 H-Blank Interrupt (1=Enable) (Read/Write)
        /// </summary>
        /// <value>
        /// <c>true</c> if [mode 0 h-blank interrupt enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool Mode0HBlankInterrupt { get; set; }

        /// <summary>
        /// Coincidence Flag  (0:LYC!=LY, 1:LYC=LY) (Read Only)
        /// </summary>
        /// <value>
        /// <c>true</c> if [LYC=LY]; otherwise, <c>false</c>.
        /// </value>
        public bool CoincidenceFlag { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
            =>
                $"GpuMode: {GpuMode}, CoincidenceInterrupt: {CoincidenceInterrupt}, Mode2OamInterrupt: {Mode2OamInterrupt}, Mode1VBlankInterrupt: {Mode1VBlankInterrupt}, Mode0HBlankInterrupt: {Mode0HBlankInterrupt}, CoincidenceFlag: {CoincidenceFlag}";
    }
}