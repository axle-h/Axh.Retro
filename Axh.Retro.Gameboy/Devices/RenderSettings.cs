namespace Axh.Retro.GameBoy.Devices
{
    /// <summary>
    /// GameBoy GPU render settings.
    /// </summary>
    internal struct RenderSettings
    {
        public readonly ushort TileMapAddress;
        public readonly ushort TileSetAddress;
        public readonly bool TileSetIsSigned;
        public readonly byte ScrollX;
        public readonly byte ScrollY;
        public readonly byte SpriteHeight;
        public readonly bool SpritesEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSettings" /> struct.
        /// </summary>
        /// <param name="tileMapAddress">The tile map address.</param>
        /// <param name="tileSetAddress">The tile set address.</param>
        /// <param name="tileSetIsSigned">if set to <c>true</c> [tile set is signed].</param>
        /// <param name="scrollX">The scroll x.</param>
        /// <param name="scrollY">The scroll y.</param>
        /// <param name="bigSprites">if set to <c>true</c> [big sprites].</param>
        /// <param name="spritesEnabled">if set to <c>true</c> [sprites enabled].</param>
        public RenderSettings(ushort tileMapAddress,
            ushort tileSetAddress,
            bool tileSetIsSigned,
            byte scrollX,
            byte scrollY,
            bool bigSprites,
            bool spritesEnabled) : this()
        {
            TileMapAddress = tileMapAddress;
            TileSetAddress = tileSetAddress;
            TileSetIsSigned = tileSetIsSigned;
            ScrollX = scrollX;
            ScrollY = scrollY;
            SpriteHeight = (byte) (bigSprites ? 16 : 8);
            SpritesEnabled = spritesEnabled;
        }

        public bool Equals(RenderSettings other)
        {
            return TileMapAddress == other.TileMapAddress && TileSetAddress == other.TileSetAddress &&
                   TileSetIsSigned == other.TileSetIsSigned && ScrollX == other.ScrollX && ScrollY == other.ScrollY &&
                   SpriteHeight == other.SpriteHeight && SpritesEnabled == other.SpritesEnabled;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current instance. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is RenderSettings && Equals((RenderSettings) obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TileMapAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ TileSetAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ TileSetIsSigned.GetHashCode();
                hashCode = (hashCode * 397) ^ ScrollX.GetHashCode();
                hashCode = (hashCode * 397) ^ ScrollY.GetHashCode();
                hashCode = (hashCode * 397) ^ SpriteHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ SpritesEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}