namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    using System;

    public class Tile
    {
        public Tile(Guid id, Palette[][] paletteMap)
        {
            this.Id = id;
            this.PaletteMap = paletteMap;
        }

        public Palette[][] PaletteMap { get; }

        public Guid Id { get; }
    }

    public enum Palette : byte
    {
        Colour0 = 0,
        Colour1 = 1,
        Colour2 = 2,
        Colour3 = 3
    }
}
