using System.Collections.Generic;
using System.Drawing;
using Axh.Retro.GameBoy.Contracts;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.Console.Config
{
    public class StaticGameBoyConfig : IGameBoyConfig
    {
        public StaticGameBoyConfig(byte[] cartridge)
        {
            CartridgeData = cartridge;
        }

        public byte[] CartridgeData { get; }

        public GameBoyType GameBoyType => GameBoyType.GameBoy;

        public bool RunGpu => true;

        public bool UseGameBoyTimings => false;

        public IDictionary<MonocromeShade, Color> MonocromePalette
            =>
                new Dictionary<MonocromeShade, Color>
                {
                    [MonocromeShade.White] = Color.FromArgb(255, 255, 255),
                    [MonocromeShade.LightGray] = Color.FromArgb(192, 192, 192),
                    [MonocromeShade.DarkGray] = Color.FromArgb(96, 96, 96),
                    [MonocromeShade.Black] = Color.FromArgb(0, 0, 0)
                };
    }
}