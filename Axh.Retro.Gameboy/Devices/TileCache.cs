﻿using System.Collections.Generic;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.Devices
{
    /// <summary>
    /// A cache of <see cref="Tile"/> objects.
    /// </summary>
    public class TileCache
    {
        private readonly byte[] _bytes;
        private readonly IDictionary<int, Tile> _cache;

        public TileCache(byte[] bytes)
        {
            _bytes = bytes;
            _cache = new Dictionary<int, Tile>();
        }

        /// <summary>
        /// Gets the tile at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Tile GetTile(int index)
        {
            if (_cache.ContainsKey(index))
            {
                return _cache[index];
            }

            var tile = ReadTile(index);
            _cache[index] = tile;
            return tile;
        }

        /// <summary>
        /// Reads the tile directly from tile RAM.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Tile ReadTile(int index)
        {
            var palette = new Palette[64];
            var address = index * 16;
            for (var row = 0; row < 8; row++, address += 2)
            {
                var low = _bytes[address];
                var high = _bytes[address + 1];
                var baseIndex = 8 * row;

                for (var col = 0; col < 8; col++)
                {
                    // Each value is a 2bit number stored in matching positions across low and high bytes
                    var mask = 0x1 << (7 - col);
                    palette[col + baseIndex] = (Palette)(((low & mask) > 0 ? 1 : 0) + ((high & mask) > 0 ? 2 : 0));
                }
            }

            return new Tile(palette);
        }
    }
}
