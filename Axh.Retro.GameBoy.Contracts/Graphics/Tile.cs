namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class Tile
    {
        public Tile(Guid id, Palette[][] paletteMap)
        {
            this.Id = id;
            this.PaletteMap = paletteMap;
        }

        public Palette[][] PaletteMap { get; }

        public Guid Id { get; }

        public void Paint(Bitmap image, int x, int y, IDictionary<Palette, Color> colors)
        {
            for (var r = 0; r < 8; r++)
            {
                for (var c = 0; c < 8; c++)
                {
                    image.SetPixel(x + r, y + c, colors[PaletteMap[r][c]]);
                }
            }
        }

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
