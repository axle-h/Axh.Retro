namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    /// <summary>
    /// Stack allocated sprite structure.
    /// </summary>
    public struct Sprite
    {
        /// <summary>
        /// Gets the X-coordinate of top-left corner.
        /// </summary>
        /// <value>
        /// The X-coordinate of top-left corner.
        /// </value>
        /// <remarks>
        /// Value stored is X-coordinate minus 8.
        /// </remarks>
        public byte X { get; }

        /// <summary>
        /// Gets the Y-coordinate of top-left corner.
        /// </summary>
        /// <value>
        /// The Y-coordinate of top-left corner.
        /// </value>
        /// <remarks>
        /// Value stored is Y-coordinate minus 16.
        /// </remarks>
        public byte Y { get; }

        /// <summary>
        /// Gets the tile number.
        /// </summary>
        /// <value>
        /// The tile number.
        /// </value>
        public byte TileNumber { get; }

        /// <summary>
        /// Gets a value indicating whether the sprite is above or below the background.
        /// </summary>
        /// <value>
        ///   <c>true</c> if sprite should be rendered below background (except colour 0); otherwise, <c>false</c>.
        /// </value>
        public bool BackgroundPriority { get; }

        /// <summary>
        /// Gets a value indicating whether the sprite is flipped in the vertical (y) direction.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the sprite is flipped in the vertical (y) direction; otherwise, <c>false</c>.
        /// </value>
        public bool YFlip { get; }

        /// <summary>
        /// Gets a value indicating whether the sprite is flipped in the horizontal (x) direction.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the sprite is flipped in the horizontal (x) direction; otherwise, <c>false</c>.
        /// </value>
        public bool XFlip { get; }

        /// <summary>
        /// Gets a value indicating whether the sprite should use sprite palette 1 or 0.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the sprite should be rendered using sprite palette 1; otherwise, <c>false</c> for sprite palette 0.
        /// </value>
        public bool UsePalette1 { get; }
    }
}
