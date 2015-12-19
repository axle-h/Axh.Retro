namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Stack allocated 8*8=64 byte tile structure.
    /// TODO: Is this worth it?!
    /// </summary>
    public struct Tile
    {
        private readonly Palette palette00; private readonly Palette palette10; private readonly Palette palette20; private readonly Palette palette30; private readonly Palette palette40; private readonly Palette palette50; private readonly Palette palette60; private readonly Palette palette70;
        private readonly Palette palette01; private readonly Palette palette11; private readonly Palette palette21; private readonly Palette palette31; private readonly Palette palette41; private readonly Palette palette51; private readonly Palette palette61; private readonly Palette palette71;
        private readonly Palette palette02; private readonly Palette palette12; private readonly Palette palette22; private readonly Palette palette32; private readonly Palette palette42; private readonly Palette palette52; private readonly Palette palette62; private readonly Palette palette72;
        private readonly Palette palette03; private readonly Palette palette13; private readonly Palette palette23; private readonly Palette palette33; private readonly Palette palette43; private readonly Palette palette53; private readonly Palette palette63; private readonly Palette palette73;
        private readonly Palette palette04; private readonly Palette palette14; private readonly Palette palette24; private readonly Palette palette34; private readonly Palette palette44; private readonly Palette palette54; private readonly Palette palette64; private readonly Palette palette74;
        private readonly Palette palette05; private readonly Palette palette15; private readonly Palette palette25; private readonly Palette palette35; private readonly Palette palette45; private readonly Palette palette55; private readonly Palette palette65; private readonly Palette palette75;
        private readonly Palette palette06; private readonly Palette palette16; private readonly Palette palette26; private readonly Palette palette36; private readonly Palette palette46; private readonly Palette palette56; private readonly Palette palette66; private readonly Palette palette76;
        private readonly Palette palette07; private readonly Palette palette17; private readonly Palette palette27; private readonly Palette palette37; private readonly Palette palette47; private readonly Palette palette57; private readonly Palette palette67; private readonly Palette palette77;

        public Tile(IReadOnlyList<Palette> paletteMap) : this()
        {
            palette00 = paletteMap[0]; palette01 = paletteMap[1]; palette02 = paletteMap[2]; palette03 = paletteMap[3]; palette04 = paletteMap[4]; palette05 = paletteMap[5]; palette06 = paletteMap[6]; palette07 = paletteMap[7];
            palette10 = paletteMap[8]; palette11 = paletteMap[9]; palette12 = paletteMap[10]; palette13 = paletteMap[11]; palette14 = paletteMap[12]; palette15 = paletteMap[13]; palette16 = paletteMap[14]; palette17 = paletteMap[15];
            palette20 = paletteMap[16]; palette21 = paletteMap[17]; palette22 = paletteMap[18]; palette23 = paletteMap[19]; palette24 = paletteMap[20]; palette25 = paletteMap[21]; palette26 = paletteMap[22]; palette27 = paletteMap[23];
            palette30 = paletteMap[24]; palette31 = paletteMap[25]; palette32 = paletteMap[26]; palette33 = paletteMap[27]; palette34 = paletteMap[28]; palette35 = paletteMap[29]; palette36 = paletteMap[30]; palette37 = paletteMap[31];
            palette40 = paletteMap[32]; palette41 = paletteMap[33]; palette42 = paletteMap[34]; palette43 = paletteMap[35]; palette44 = paletteMap[36]; palette45 = paletteMap[37]; palette46 = paletteMap[38]; palette47 = paletteMap[39];
            palette50 = paletteMap[40]; palette51 = paletteMap[41]; palette52 = paletteMap[42]; palette53 = paletteMap[43]; palette54 = paletteMap[44]; palette55 = paletteMap[45]; palette56 = paletteMap[46]; palette57 = paletteMap[47];
            palette60 = paletteMap[48]; palette61 = paletteMap[49]; palette62 = paletteMap[50]; palette63 = paletteMap[51]; palette64 = paletteMap[52]; palette65 = paletteMap[53]; palette66 = paletteMap[54]; palette67 = paletteMap[55];
            palette70 = paletteMap[56]; palette71 = paletteMap[57]; palette72 = paletteMap[58]; palette73 = paletteMap[59]; palette74 = paletteMap[60]; palette75 = paletteMap[61]; palette76 = paletteMap[62]; palette77 = paletteMap[63];
        }

        public Palette Get(int x, int y)
        {
            switch (x)
            {
                case 0:
                    switch (y)
                    {
                        case 0: return palette00;
                        case 1: return palette01;
                        case 2: return palette02;
                        case 3: return palette03;
                        case 4: return palette04;
                        case 5: return palette05;
                        case 6: return palette06;
                        case 7: return palette07;
                        default: return 0;
                    }
                case 1:
                    switch (y)
                    {
                        case 0: return palette10;
                        case 1: return palette11;
                        case 2: return palette12;
                        case 3: return palette13;
                        case 4: return palette14;
                        case 5: return palette15;
                        case 6: return palette16;
                        case 7: return palette17;
                        default: return 0;
                    }
                case 2:
                    switch (y)
                    {
                        case 0: return palette20;
                        case 1: return palette21;
                        case 2: return palette22;
                        case 3: return palette23;
                        case 4: return palette24;
                        case 5: return palette25;
                        case 6: return palette26;
                        case 7: return palette27;
                        default: return 0;
                    }
                case 3:
                    switch (y)
                    {
                        case 0: return palette30;
                        case 1: return palette31;
                        case 2: return palette32;
                        case 3: return palette33;
                        case 4: return palette34;
                        case 5: return palette35;
                        case 6: return palette36;
                        case 7: return palette37;
                        default: return 0;
                    }
                case 4:
                    switch (y)
                    {
                        case 0: return palette40;
                        case 1: return palette41;
                        case 2: return palette42;
                        case 3: return palette43;
                        case 4: return palette44;
                        case 5: return palette45;
                        case 6: return palette46;
                        case 7: return palette47;
                        default: return 0;
                    }
                case 5:
                    switch (y)
                    {
                        case 0: return palette50;
                        case 1: return palette51;
                        case 2: return palette52;
                        case 3: return palette53;
                        case 4: return palette54;
                        case 5: return palette55;
                        case 6: return palette56;
                        case 7: return palette57;
                        default: return 0;
                    }
                case 6:
                    switch (y)
                    {
                        case 0: return palette60;
                        case 1: return palette61;
                        case 2: return palette62;
                        case 3: return palette63;
                        case 4: return palette64;
                        case 5: return palette65;
                        case 6: return palette66;
                        case 7: return palette67;
                        default: return 0;
                    }
                case 7:
                    switch (y)
                    {
                        case 0: return palette70;
                        case 1: return palette71;
                        case 2: return palette72;
                        case 3: return palette73;
                        case 4: return palette74;
                        case 5: return palette75;
                        case 6: return palette76;
                        case 7: return palette77;
                        default: return 0;
                    }
                default: return 0;
            }
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
