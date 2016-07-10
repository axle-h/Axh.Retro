using System.Collections.Generic;
using System.IO;
using System.Linq;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.Devices
{
    /// <summary>
    /// Gameboy GPU tile map pointer.
    /// </summary>
    internal class TileMapPointer
    {
        private readonly Sprite[] _allSprites;

        private readonly RenderSettings _renderSettings;
        private readonly IDictionary<byte, Tile> _spriteTileCache;
        private readonly byte[] _spriteTileSetBytes;
        private readonly IDictionary<int, Tile> _tileCache;
        private readonly byte[] _tileMapBytes;
        private readonly byte[] _tileSetBytes;

        private int _column;
        private Sprite[] _currentSprites;

        private Tile _currentTile;
        private int _row;
        private int _tileColumn;
        private int _tileMapColumn;
        private int _tileMapRow;
        private int _tileRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileMapPointer" /> class.
        /// </summary>
        /// <param name="renderSettings">The render settings.</param>
        /// <param name="tileSetBytes">The tile set bytes.</param>
        /// <param name="tileMapBytes">The tile map bytes.</param>
        /// <param name="spriteBytes">The sprite bytes.</param>
        /// <param name="spriteTileSetBytes">The sprite tile set bytes.</param>
        public TileMapPointer(RenderSettings renderSettings,
            byte[] tileSetBytes,
            byte[] tileMapBytes,
            byte[] spriteBytes,
            byte[] spriteTileSetBytes)
        {
            _renderSettings = renderSettings;

            _column = renderSettings.ScrollX;
            _tileMapColumn = renderSettings.ScrollX / 8;
            _tileColumn = renderSettings.ScrollX % 8;

            _row = renderSettings.ScrollY;
            _tileMapRow = renderSettings.ScrollY / 8;
            _tileRow = renderSettings.ScrollY % 8;

            // TODO: tile sets and caches can be shared when sprite and background set to same set.
            _tileSetBytes = tileSetBytes;
            _tileMapBytes = tileMapBytes;
            _spriteTileSetBytes = spriteTileSetBytes;
            _allSprites = renderSettings.SpritesEnabled ? GetAllSprites(spriteBytes).ToArray() : new Sprite[0];
            _tileCache = new Dictionary<int, Tile>();
            _spriteTileCache = new Dictionary<byte, Tile>();

            UpdateCurrentTile();
            UpdateRowSprites();
        }

        /// <summary>
        /// Gets the current pixel under the pointer.
        /// </summary>
        /// <value>
        /// The current pixel under the pointer.
        /// </value>
        public Palette Pixel
        {
            get
            {
                var background = _currentTile.Get(_tileRow, _tileColumn);
                if (!_renderSettings.SpritesEnabled)
                {
                    return background;
                }

                foreach (var sprite in _currentSprites.Where(s => _column >= s.X && _column < s.X + 8))
                {
                    // TODO: 8x16 sprites.
                    // TODO: Background priority sprites.
                    Tile spriteTile;
                    if (_spriteTileCache.ContainsKey(sprite.TileNumber))
                    {
                        spriteTile = _spriteTileCache[sprite.TileNumber];
                    }
                    else
                    {
                        _spriteTileCache[sprite.TileNumber] = spriteTile = GetTile(_spriteTileSetBytes, sprite.TileNumber * 16);
                    }

                    return spriteTile.Get(_row - sprite.Y, _column - sprite.X);
                }

                return background;
            }
        }

        /// <summary>
        /// Moves the pointer to the next column.
        /// </summary>
        public void NextColumn()
        {
            _column++;
            _tileColumn = (_tileColumn + 1) % 8;
            if (_tileColumn == 0)
            {
                _tileMapColumn++;
                UpdateCurrentTile();
            }
        }

        /// <summary>
        /// Moves the pointer to the next row.
        /// </summary>
        public void NextRow()
        {
            _row++;
            _tileRow = (_tileRow + 1) % 8;
            if (_tileRow == 0)
            {
                _tileMapRow++;
            }

            UpdateRowSprites();

            // Reset column.
            _column = _renderSettings.ScrollX;
            _tileMapColumn = _renderSettings.ScrollX / 8;
            _tileColumn = _renderSettings.ScrollX % 8;
            UpdateCurrentTile();
        }

        /// <summary>
        /// Updates the row sprites.
        /// </summary>
        private void UpdateRowSprites()
        {
            // TODO: Multiple sprite priority.
            // TODO: Max 10 sprites per scan.
            // TODO: 8x16 sprites.
            _currentSprites = _allSprites.Where(s => _row >= s.Y && _row < s.Y + 8).ToArray();
        }

        /// <summary>
        /// Updates the current tile.
        /// </summary>
        private void UpdateCurrentTile()
        {
            var tileMapIndex = _tileMapRow * 32 + _tileMapColumn;
            var tileMapValue = _tileMapBytes[tileMapIndex];
            var tileSetIndex = _renderSettings.TileSetIsSigned ? (sbyte) tileMapValue + 128 : tileMapValue;
            if (_tileCache.ContainsKey(tileSetIndex))
            {
                _currentTile = _tileCache[tileSetIndex];
            }
            else
            {
                _tileCache[tileSetIndex] = _currentTile = GetTile(_tileSetBytes, tileSetIndex * 16);
            }
        }

        /// <summary>
        /// Gets the tile.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private static Tile GetTile(byte[] buffer, int offset)
        {
            var palette = new Palette[64];
            var address = offset;
            for (var row = 0; row < 8; row++, address += 2)
            {
                var low = buffer[address];
                var high = buffer[address + 1];
                var baseIndex = 8 * row;

                for (var col = 0; col < 8; col++)
                {
                    // Each value is a 2bit number stored in matching positions across low and high bytes
                    var mask = 0x1 << (7 - col);
                    palette[col + baseIndex] = (Palette) (((low & mask) > 0 ? 1 : 0) + ((high & mask) > 0 ? 2 : 0));
                }
            }

            return new Tile(palette);
        }

        /// <summary>
        /// Gets all sprites.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        private static IEnumerable<Sprite> GetAllSprites(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                for (var i = 0; i < 40; i++)
                {
                    var y = stream.ReadByte() - 16;
                    var x = stream.ReadByte() - 8;
                    var n = (byte) stream.ReadByte();
                    var flags = stream.ReadByte();

                    if (x <= 0 || x >= Gpu.LcdWidth || y <= 0 || y >= Gpu.LcdHeight)
                    {
                        // Off screen sprite.
                        continue;
                    }

                    yield return
                        new Sprite((byte) x,
                                   (byte) y,
                                   n,
                                   (flags & 0x08) > 0,
                                   (flags & 0x04) > 0,
                                   (flags & 0x02) > 0,
                                   (flags & 0x01) > 0);
                }
            }
        }
    }
}