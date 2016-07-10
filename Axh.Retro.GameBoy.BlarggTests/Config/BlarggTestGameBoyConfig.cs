using System.Collections.Generic;
using System.Drawing;
using Axh.Retro.GameBoy.Contracts;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.BlarggTests.Config
{
    public class BlarggTestGameBoyConfig : IGameBoyConfig
    {
        public byte[] CartridgeData { get; set; }

        public GameBoyType GameBoyType => GameBoyType.GameBoy;

        public bool RunGpu => false;

        public bool UseGameBoyTimings => false;

        public IDictionary<MonochromeShade, Color> MonocromePalette
            =>
                new Dictionary<MonochromeShade, Color>
                {
                    [MonochromeShade.White] = Color.FromArgb(255, 255, 255),
                    [MonochromeShade.LightGray] = Color.FromArgb(192, 192, 192),
                    [MonochromeShade.DarkGray] = Color.FromArgb(96, 96, 96),
                    [MonochromeShade.Black] = Color.FromArgb(0, 0, 0)
                };
    }
}