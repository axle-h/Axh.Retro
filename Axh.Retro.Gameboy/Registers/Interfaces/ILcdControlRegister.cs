namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    public interface ILcdControlRegister : IRegister
    {
        /// <summary>
        /// LCD status
        /// True: On
        /// False: Off
        /// </summary>
        bool LcdOperation { get; }

        /// <summary>
        /// Sets which tile map the window uses
        /// True: 9C00-9FFF (1)
        /// False: 9800-9BFF (0)
        /// </summary>
        bool WindowTileMap { get; }

        /// <summary>
        /// Window status
        /// True: On
        /// False: Off
        /// </summary>
        bool WindowDisplay { get; }

        /// <summary>
        /// Sets which tile pattern table to use
        /// True: 8000-8FFF (1)
        /// False: 8800-97FF (0)
        /// </summary>
        bool TilePatternTable { get; }

        /// <summary>
        /// Sets which tile map the background uses
        /// True: 9C00-9FFF (1)
        /// False: 9800-9BFF (0)
        /// </summary>
        bool BackgroundTileMap { get; }

        /// <summary>
        /// Sets the sprite size
        /// True: 8x16
        /// False: 8x8
        /// </summary>
        bool SpriteSize { get; }

        /// <summary>
        /// Sets the transparency of colour 0 on the window
        /// True: SOLID
        /// False: TRANSPARENT
        /// </summary>
        bool WindowColor0Transparent { get; }

        /// <summary>
        /// Background status
        /// True: On
        /// False: Off
        /// </summary>
        bool BackgroundDisplay { get; }
    }
}