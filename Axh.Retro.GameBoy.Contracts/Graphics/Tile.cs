using System.Collections.Generic;

namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    /// <summary>
    ///     Stack allocated 8*8=64 byte tile structure.
    ///     TODO: Is this worth it?!
    /// </summary>
    public struct Tile
    {
        private readonly Palette _palette00,
            _palette01,
            _palette02,
            _palette03,
            _palette04,
            _palette05,
            _palette06,
            _palette07,
            _palette10,
            _palette11,
            _palette12,
            _palette13,
            _palette14,
            _palette15,
            _palette16,
            _palette17,
            _palette20,
            _palette21,
            _palette22,
            _palette23,
            _palette24,
            _palette25,
            _palette26,
            _palette27,
            _palette30,
            _palette31,
            _palette32,
            _palette33,
            _palette34,
            _palette35,
            _palette36,
            _palette37,
            _palette40,
            _palette41,
            _palette42,
            _palette43,
            _palette44,
            _palette45,
            _palette46,
            _palette47,
            _palette50,
            _palette51,
            _palette52,
            _palette53,
            _palette54,
            _palette55,
            _palette56,
            _palette57,
            _palette60,
            _palette61,
            _palette62,
            _palette63,
            _palette64,
            _palette65,
            _palette66,
            _palette67,
            _palette70,
            _palette71,
            _palette72,
            _palette73,
            _palette74,
            _palette75,
            _palette76,
            _palette77;

        public Tile(IReadOnlyList<Palette> paletteMap) : this()
        {
            _palette00 = paletteMap[0];
            _palette01 = paletteMap[1];
            _palette02 = paletteMap[2];
            _palette03 = paletteMap[3];
            _palette04 = paletteMap[4];
            _palette05 = paletteMap[5];
            _palette06 = paletteMap[6];
            _palette07 = paletteMap[7];
            _palette10 = paletteMap[8];
            _palette11 = paletteMap[9];
            _palette12 = paletteMap[10];
            _palette13 = paletteMap[11];
            _palette14 = paletteMap[12];
            _palette15 = paletteMap[13];
            _palette16 = paletteMap[14];
            _palette17 = paletteMap[15];
            _palette20 = paletteMap[16];
            _palette21 = paletteMap[17];
            _palette22 = paletteMap[18];
            _palette23 = paletteMap[19];
            _palette24 = paletteMap[20];
            _palette25 = paletteMap[21];
            _palette26 = paletteMap[22];
            _palette27 = paletteMap[23];
            _palette30 = paletteMap[24];
            _palette31 = paletteMap[25];
            _palette32 = paletteMap[26];
            _palette33 = paletteMap[27];
            _palette34 = paletteMap[28];
            _palette35 = paletteMap[29];
            _palette36 = paletteMap[30];
            _palette37 = paletteMap[31];
            _palette40 = paletteMap[32];
            _palette41 = paletteMap[33];
            _palette42 = paletteMap[34];
            _palette43 = paletteMap[35];
            _palette44 = paletteMap[36];
            _palette45 = paletteMap[37];
            _palette46 = paletteMap[38];
            _palette47 = paletteMap[39];
            _palette50 = paletteMap[40];
            _palette51 = paletteMap[41];
            _palette52 = paletteMap[42];
            _palette53 = paletteMap[43];
            _palette54 = paletteMap[44];
            _palette55 = paletteMap[45];
            _palette56 = paletteMap[46];
            _palette57 = paletteMap[47];
            _palette60 = paletteMap[48];
            _palette61 = paletteMap[49];
            _palette62 = paletteMap[50];
            _palette63 = paletteMap[51];
            _palette64 = paletteMap[52];
            _palette65 = paletteMap[53];
            _palette66 = paletteMap[54];
            _palette67 = paletteMap[55];
            _palette70 = paletteMap[56];
            _palette71 = paletteMap[57];
            _palette72 = paletteMap[58];
            _palette73 = paletteMap[59];
            _palette74 = paletteMap[60];
            _palette75 = paletteMap[61];
            _palette76 = paletteMap[62];
            _palette77 = paletteMap[63];
        }

        public Palette Get(int row, int column)
        {
            switch (row)
            {
                case 0:
                    switch (column)
                    {
                        case 0:
                            return _palette00;
                        case 1:
                            return _palette01;
                        case 2:
                            return _palette02;
                        case 3:
                            return _palette03;
                        case 4:
                            return _palette04;
                        case 5:
                            return _palette05;
                        case 6:
                            return _palette06;
                        case 7:
                            return _palette07;
                        default:
                            return 0;
                    }
                case 1:
                    switch (column)
                    {
                        case 0:
                            return _palette10;
                        case 1:
                            return _palette11;
                        case 2:
                            return _palette12;
                        case 3:
                            return _palette13;
                        case 4:
                            return _palette14;
                        case 5:
                            return _palette15;
                        case 6:
                            return _palette16;
                        case 7:
                            return _palette17;
                        default:
                            return 0;
                    }
                case 2:
                    switch (column)
                    {
                        case 0:
                            return _palette20;
                        case 1:
                            return _palette21;
                        case 2:
                            return _palette22;
                        case 3:
                            return _palette23;
                        case 4:
                            return _palette24;
                        case 5:
                            return _palette25;
                        case 6:
                            return _palette26;
                        case 7:
                            return _palette27;
                        default:
                            return 0;
                    }
                case 3:
                    switch (column)
                    {
                        case 0:
                            return _palette30;
                        case 1:
                            return _palette31;
                        case 2:
                            return _palette32;
                        case 3:
                            return _palette33;
                        case 4:
                            return _palette34;
                        case 5:
                            return _palette35;
                        case 6:
                            return _palette36;
                        case 7:
                            return _palette37;
                        default:
                            return 0;
                    }
                case 4:
                    switch (column)
                    {
                        case 0:
                            return _palette40;
                        case 1:
                            return _palette41;
                        case 2:
                            return _palette42;
                        case 3:
                            return _palette43;
                        case 4:
                            return _palette44;
                        case 5:
                            return _palette45;
                        case 6:
                            return _palette46;
                        case 7:
                            return _palette47;
                        default:
                            return 0;
                    }
                case 5:
                    switch (column)
                    {
                        case 0:
                            return _palette50;
                        case 1:
                            return _palette51;
                        case 2:
                            return _palette52;
                        case 3:
                            return _palette53;
                        case 4:
                            return _palette54;
                        case 5:
                            return _palette55;
                        case 6:
                            return _palette56;
                        case 7:
                            return _palette57;
                        default:
                            return 0;
                    }
                case 6:
                    switch (column)
                    {
                        case 0:
                            return _palette60;
                        case 1:
                            return _palette61;
                        case 2:
                            return _palette62;
                        case 3:
                            return _palette63;
                        case 4:
                            return _palette64;
                        case 5:
                            return _palette65;
                        case 6:
                            return _palette66;
                        case 7:
                            return _palette67;
                        default:
                            return 0;
                    }
                case 7:
                    switch (column)
                    {
                        case 0:
                            return _palette70;
                        case 1:
                            return _palette71;
                        case 2:
                            return _palette72;
                        case 3:
                            return _palette73;
                        case 4:
                            return _palette74;
                        case 5:
                            return _palette75;
                        case 6:
                            return _palette76;
                        case 7:
                            return _palette77;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }
    }
}