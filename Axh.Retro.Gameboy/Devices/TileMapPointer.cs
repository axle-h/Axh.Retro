using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.Devices
{
    /// <summary>
    /// Gameboy GPU tile map pointer.
    /// </summary>
    internal class TileMapPointer
    {
        private RenderSettings _renderSettings;
        private Sprite[] _allSprites;
        private TileCache _tiles;
        private TileCache _spriteTiles;
        private byte[] _backgroundTileMap;

        private int _column;
        
        private Sprite[] _currentSprites;

        private Tile _currentTile;
        private int _row;
        private int _tileColumn;
        private int _tileMapColumn;
        private int _tileMapRow;
        private int _tileRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileMapPointer"/> class.
        /// </summary>
        /// <param name="renderState">State of the render.</param>
        public TileMapPointer(RenderState renderState)
        {
            Reset(renderState, RenderStateChange.All);
        }

        /// <summary>
        /// Resets the specified render state.
        /// </summary>
        /// <param name="renderState">State of the render.</param>
        /// <param name="stateChange">The state change.</param>
        public TileMapPointer Reset(RenderState renderState, RenderStateChange stateChange)
        {
            _renderSettings = renderState.RenderSettings;
            _column = renderState.RenderSettings.ScrollX;
            _tileMapColumn = renderState.RenderSettings.ScrollX / 8;
            _tileColumn = renderState.RenderSettings.ScrollX % 8;

            _row = renderState.RenderSettings.ScrollY;
            _tileMapRow = renderState.RenderSettings.ScrollY / 8;
            _tileRow = renderState.RenderSettings.ScrollY % 8;

            _backgroundTileMap = renderState.BackgroundTileMap;

            // Can use existing cache if the tile set has not changed.
            if (stateChange.HasFlag(RenderStateChange.TileSet))
            {
                _tiles = new TileCache(renderState.TileSet);
            }

            if (_renderSettings.SpritesEnabled)
            {
                if (_spriteTiles == null || stateChange.HasFlag(RenderStateChange.SpriteTileSet))
                {
                    _spriteTiles = renderState.RenderSettings.SpriteAndBackgroundTileSetShared
                                       ? _tiles
                                       : new TileCache(renderState.SpriteTileSet);
                }

                if (_allSprites == null || stateChange.HasFlag(RenderStateChange.SpriteOam))
                {
                    _allSprites = GetAllSprites(renderState.SpriteOam).ToArray();
                }

                UpdateRowSprites();
            }
            else
            {
                _spriteTiles = null;
                _allSprites = null;
                _currentSprites = null;
            }
            
            UpdateCurrentTile();

            return this;
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
                    var spriteTile = _spriteTiles.GetTile(sprite.TileNumber);
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
            if (!_renderSettings.SpritesEnabled)
            {
                return;
            }

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
            var tileMapValue = _backgroundTileMap[tileMapIndex];

            int index;

            if (_renderSettings.TileSetIsSigned)
            {
                var signedTileMapValue = (sbyte) tileMapValue;
                index = signedTileMapValue < 0 ? Math.Abs(signedTileMapValue) - 1 : signedTileMapValue + 128;
            }
            else
            {
                index = tileMapValue;
            }

            _currentTile = _tiles.GetTile(index);
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