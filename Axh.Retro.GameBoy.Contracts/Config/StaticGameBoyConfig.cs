using System.Collections.Generic;
using System.Drawing;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.Contracts.Config
{
    /// <summary>
    /// Immutable gameboy config.
    /// </summary>
    /// <seealso cref="Axh.Retro.GameBoy.Contracts.Config.IGameBoyConfig" />
    public class StaticGameBoyConfig : IGameBoyConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticGameBoyConfig" /> class.
        /// </summary>
        /// <param name="cartridge">The cartridge.</param>
        public StaticGameBoyConfig(byte[] cartridge)
        {
            CartridgeData = cartridge;
        }

        /// <summary>
        /// Gets the cartridge data.
        /// </summary>
        /// <value>
        /// The cartridge data.
        /// </value>
        public byte[] CartridgeData { get; }

        /// <summary>
        /// Gets the type of the game boy.
        /// </summary>
        /// <value>
        /// The type of the game boy.
        /// </value>
        public GameBoyType GameBoyType => GameBoyType.GameBoy;

        /// <summary>
        /// Gets a value indicating whether [run gpu].
        /// </summary>
        /// <value>
        /// <c>true</c> if [run gpu]; otherwise, <c>false</c>.
        /// </value>
        public bool RunGpu => true;

        /// <summary>
        /// Gets the monocrome palette.
        /// </summary>
        /// <value>
        /// The monocrome palette.
        /// </value>
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