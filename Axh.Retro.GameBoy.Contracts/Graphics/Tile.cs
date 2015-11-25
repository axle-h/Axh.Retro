namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class Tile
    {
        public Tile(Guid id, byte[] tileData, Palette[][] paletteMap)
        {
            this.Id = id;
            this.TileData = tileData;
            this.PaletteMap = paletteMap;
        }

        public Palette[][] PaletteMap { get; }

        public Guid Id { get; }

        public byte[] TileData { get; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public enum Palette : byte
    {
        Colour0 = 0,
        Colour1 = 1,
        Colour2 = 2,
        Colour3 = 3
    }
}
