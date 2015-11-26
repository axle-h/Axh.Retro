namespace Axh.Retro.GameBoy.Devices
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Timers;

    using Axh.Retro.CPU.Common.Config;
    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Common.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.GameBoy.Contracts.Graphics;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Registers;
    using Axh.Retro.GameBoy.Registers.Interfaces;
    using Axh.Retro.GameBoy.Util;

    public class Gpu : ICoreGpu
    {
        private static readonly IMemoryBankConfig SpriteRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral, null, 0xfe00, 0xa0);
        private static readonly IMemoryBankConfig MapRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral, null, 0x8000, 0x2000);

        private const int Scanlines = 144;
        private const int VertaicalBlankScanlines = 153;

        /// <summary>
        /// $FE00-$FE9F	OAM - Object Attribute Memory
        /// </summary>
        private readonly ArrayBackedMemoryBank spriteRam;

        /// <summary>
        /// $9C00-$9FFF	Tile map #1
        /// $9800-$9BFF Tile map #0 32*32
        /// $9000-$97FF Tile set #0: tiles 0-127
        /// $8800-$8FFF Tile set #1: tiles 128-255 & Tile set #0: tiles -128 to -1
        /// $8000-$87FF Tile set #1: tiles 0-127
        /// </summary>
        private readonly ArrayBackedMemoryBank tileRam;

        private readonly IGameBoyInterruptManager interruptManager;

        private readonly ILcdControlRegister lcdControlRegister;

        private readonly ICurrentScanlineRegister currentScanlineRegister;

        private readonly IRenderHandler renderhandler;

        /// <summary>
        /// Tile set #0: tiles -128 to 128 (tiles -128 to -1 are references to tile set #1 tiles 128 to 255)
        /// </summary>
        private readonly Tile[] tileSet0;

        /// <summary>
        /// Tile set #1: tiles 0 to 255
        /// </summary>
        private readonly Tile[] tileSet1;

        /// <summary>
        /// We expect the GPU to reuse tiles often. Better to cache the result of parsing tiles than process them every frame.
        /// TODO: How big will this grow?
        /// </summary>
        private readonly IDictionary<Guid, Tile> tileCache;
        
        private uint lastFrameDigest;
        
        private GpuMode mode;
        
        private int currentTimings;

        private bool isEnabled;

        public Gpu(IGameBoyInterruptManager interruptManager, ILcdControlRegister lcdControlRegister, ICurrentScanlineRegister currentScanlineRegister, IRenderHandler renderhandler, IInstructionTimer timer)
        {
            this.interruptManager = interruptManager;
            this.lcdControlRegister = lcdControlRegister;
            this.currentScanlineRegister = currentScanlineRegister;
            this.renderhandler = renderhandler;
            
            this.spriteRam = new ArrayBackedMemoryBank(SpriteRamConfig);
            this.tileRam = new ArrayBackedMemoryBank(MapRamConfig);
            this.lastFrameDigest = 0;

            this.tileSet0 = new Tile[256];
            this.tileSet1 = new Tile[256];
            this.tileCache = new Dictionary<Guid, Tile>();

            this.isEnabled = false;
            this.mode = GpuMode.VerticalBlank;
            this.currentTimings = 0;
            timer.TimingSync += Sync;
        }

        public IEnumerable<IAddressSegment> AddressSegments => new[] { spriteRam, tileRam };

        public void Halt()
        {
            
        }

        public void Resume()
        {
            
        }

        private void Sync(object sender, TimingSyncEventArgs args)
        {
            if (!this.lcdControlRegister.LcdOperation)
            {
                if (this.isEnabled)
                {
                    this.mode = GpuMode.VerticalBlank;
                    this.currentTimings = 0;
                    this.currentScanlineRegister.Register = 0x00;
                    // TODO: write blank frame.
                }
                return;
            }

            this.isEnabled = true;
            
            var timings = args.InstructionTimings.MachineCycles;
            currentTimings += timings;

            const int VerticalBlankClocks = 456;
            const int ReadingOamClocks = 80;
            const int ReadingVramClocks = 172;
            const int HorizontalBlankClocks = 204;
            

            switch (mode)
            {
                case GpuMode.HorizonalBlank:
                    if (currentTimings >= HorizontalBlankClocks)
                    {
                        this.currentScanlineRegister.IncrementScanline();
                        mode = this.currentScanlineRegister.Scanline == Scanlines ? GpuMode.VerticalBlank : GpuMode.ReadingOam;
                        currentTimings -= HorizontalBlankClocks;
                    }
                    break;
                case GpuMode.VerticalBlank:
                    if (currentTimings >= VerticalBlankClocks)
                    {
                        if (this.currentScanlineRegister.Scanline == VertaicalBlankScanlines)
                        {
                            // Reset
                            this.currentScanlineRegister.Register = 0x00;
                            mode = GpuMode.ReadingOam;
                            currentTimings -= VerticalBlankClocks;
                            interruptManager.VerticalBlank();
                            break;
                        }

                        this.currentScanlineRegister.IncrementScanline();
                        currentTimings -= VerticalBlankClocks;
                    }
                    break;
                case GpuMode.ReadingOam:
                    if (currentTimings >= ReadingOamClocks)
                    {
                        mode = GpuMode.ReadingVram;
                        currentTimings -= ReadingOamClocks;
                    }
                    break;
                case GpuMode.ReadingVram:
                    if (currentTimings >= ReadingVramClocks)
                    {
                        Paint();
                        mode = GpuMode.HorizonalBlank;
                        currentTimings -= ReadingVramClocks;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void Paint()
        {
            DrawBackground();
        }

        private void DrawBackground()
        {
            UpdateTileSets();

            var tileMap = this.lcdControlRegister.BackgroundTileMap ? GetTileMap(0x1c00, tileSet1) : GetTileMap(0x1800, tileSet0);

            if (tileMap == null)
            {
                return;
            }

            // TODO get background palette register
            var colors = new Dictionary<Palette, Color>
                         {
                             { Palette.Colour0, Color.FromArgb(255, 255, 255) },
                             { Palette.Colour1, Color.FromArgb(192, 192, 192) },
                             { Palette.Colour2, Color.FromArgb(96, 96, 96) },
                             { Palette.Colour3, Color.FromArgb(0, 0, 0) }
                         };
            
            var frame = new Bitmap(32 * 8, 32 * 8);
            // Paint all 32 * 32 tiles for now
            for (var r = 0; r < 32; r++)
            {
                var y = r * 8;
                for (var c = 0; c < 32; c++)
                {
                    Paint(tileMap[r][c], frame, c * 8, y, colors);
                }
            }

            this.renderhandler.Paint(frame);
        }

        private static void Paint(Tile tile, Bitmap image, int x, int y, IDictionary<Palette, Color> colors)
        {
            for (var r = 0; r < 8; r++)
            {
                for (var c = 0; c < 8; c++)
                {
                    image.SetPixel(x + c, y + r, colors[tile.PaletteMap[r][c]]);
                }
            }
        }

        private Tile[][] GetTileMap(ushort address, Tile[] tileSet)
        {
            var tileMap = new Tile[32][];

            var tileMapBytes = this.tileRam.ReadBytes(address, 1024);

            address = 0;
            var hash = new XxHash(0);
            for (var r = 0; r < 32; r++)
            {
                tileMap[r] = new Tile[32];
                for (var c = 0; c < 32; address += 1, c++)
                {
                    var index = tileMapBytes[address];
                    var tile = tileSet[index];
                    hash.Update(tile.TileData, 16);
                    tileMap[r][c] = tile;
                }
            }

            var digest = hash.Digest();
            if (digest == lastFrameDigest)
            {
                return null;
            }

            lastFrameDigest = digest;
            return tileMap;
        }

        private void UpdateTileSets()
        {
            // $8000-$87FF Tile set #1: tiles 0 to 127
            GetTiles(0x0000, this.tileSet1, 0, 256);
            
            // $9000-$97FF Tile set #0: tiles 0 (128) - 127 (255)
            GetTiles(0x1000, this.tileSet0, 128, 128);
            
            Array.Copy(this.tileSet1, 0, this.tileSet0, 0, 128);
        }

        private void GetTiles(ushort address, Tile[] tileSet, int tileId, int length)
        {
            var buffer = new byte[16];
            for (var i = 0; i < length; address += 16, i++, tileId++)
            {
                tileRam.ReadBytes(address, buffer);
                var id = new Guid(buffer); // So happnes that a Guid stores 16 bytes nicely

                if (this.tileCache.ContainsKey(id))
                {
                    tileSet[tileId] = this.tileCache[id];
                }
                else
                {
                    tileSet[tileId] = GetTile(id, buffer);
                    this.tileCache[id] = tileSet[tileId];
                }
            }
        }

        private static Tile GetTile(Guid id, byte[] buffer)
        {
            var palette = new Palette[8][];
            var address = 0;
            for (var row = 0; row < 8; row++, address += 2)
            {
                palette[row] = new Palette[8];
                var low = buffer[address];
                var high = buffer[address + 1];

                for (var col = 0; col < 8; col++)
                {
                    // Each value is a 2bit number stored in matching positions across low and high bytes
                    var mask = 0x1 << (7- col);
                    palette[row][col] = (Palette)(((low & mask) > 0 ? 1 : 0) + ((high & mask) > 0 ? 2 : 0));
                }
            }

            var tileData = new byte[16];
            Array.Copy(buffer, tileData, 16);
            var tile = new Tile(id, tileData, palette);
            return tile;
        }
        
        
    }
}
