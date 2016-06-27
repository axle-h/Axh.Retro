using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using Axh.Retro.CPU.Common.Config;
using Axh.Retro.CPU.Common.Contracts.Config;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Common.Memory;
using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Axh.Retro.GameBoy.Devices.CoreInterfaces;
using Axh.Retro.GameBoy.Registers.Interfaces;
using Axh.Retro.GameBoy.Util;

namespace Axh.Retro.GameBoy.Devices
{
    public class Gpu : ICoreGpu
    {
        private static readonly IMemoryBankConfig SpriteRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral, null, 0xfe00, 0xa0);
        private static readonly IMemoryBankConfig MapRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral, null, 0x8000, 0x2000);

        private const int Scanlines = 144;
        private const int VertaicalBlankScanlines = 153;
        private const int VerticalBlankClocks = 456;
        private const int ReadingOamClocks = 80;
        private const int ReadingVramClocks = 172;
        private const int HorizontalBlankClocks = 204;

        private const int LcdWidth = 160;
        private const int LcdHeight = 144;
        
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

        private readonly IInterruptFlagsRegister interruptFlagsRegister;

        private readonly IGpuRegisters gpuRegisters;

        private readonly IRenderHandler renderhandler;

        private readonly IGameBoyConfig gameBoyConfig;
        
        /// <summary>
        /// Normal frame buffer.
        /// </summary>
        private readonly Bitmap lcdBuffer;

        private readonly ILcdStatusRegister lcdStatusRegister;

        private readonly object disposingContext = new object();

        private int currentTimings;

        private bool isEnabled;

        private byte[] lastTileMapBytes, lastTileSetBytes, lastSpriteBytes, lastSpriteTileSetBytes;

        private RenderSettings lastRenderSettings;

        private TaskCompletionSource<bool> paintingTaskCompletionSource;
        private bool disposed;
        

        public Gpu(IGameBoyConfig gameBoyConfig, IInterruptFlagsRegister interruptFlagsRegister, IGpuRegisters gpuRegisters, IRenderHandler renderhandler, IInstructionTimer timer)
        {
            this.interruptFlagsRegister = interruptFlagsRegister;
            this.gpuRegisters = gpuRegisters;
            this.renderhandler = renderhandler;
            this.gameBoyConfig = gameBoyConfig;
            this.lcdStatusRegister = gpuRegisters.LcdStatusRegister;


            this.spriteRam = new ArrayBackedMemoryBank(SpriteRamConfig);
            this.tileRam = new ArrayBackedMemoryBank(MapRamConfig);

            this.isEnabled = false;
            this.lcdStatusRegister.GpuMode = GpuMode.VerticalBlank;
            this.currentTimings = 0;
            
            if (gameBoyConfig.RunGpu)
            {
                timer.TimingSync += Sync;
            }
            
            lcdBuffer = new Bitmap(LcdWidth, LcdHeight, PixelFormat.Format8bppIndexed);
            paintingTaskCompletionSource = new TaskCompletionSource<bool>();
            disposed = false;

            Task.Factory.StartNew(() => PaintLoop().Wait(), TaskCreationOptions.LongRunning);
        }

        public IEnumerable<IAddressSegment> AddressSegments => new[] { spriteRam, tileRam };

        public void Halt()
        {
            
        }

        public void Resume()
        {
            
        }

        private void Sync(InstructionTimings instructionTimings)
        {
            if (!gpuRegisters.LcdControlRegister.LcdOperation)
            {
                if (isEnabled)
                {
                    lcdStatusRegister.GpuMode = GpuMode.VerticalBlank;
                    currentTimings = 0;
                    gpuRegisters.CurrentScanlineRegister.Register = 0x00;
                }
                return;
            }

            this.isEnabled = true;
            
            var timings = instructionTimings.MachineCycles;
            currentTimings += timings;
            
            switch (lcdStatusRegister.GpuMode)
            {
                case GpuMode.HorizonalBlank:
                    if (currentTimings >= HorizontalBlankClocks)
                    {
                        this.gpuRegisters.CurrentScanlineRegister.IncrementScanline();
                        lcdStatusRegister.GpuMode = this.gpuRegisters.CurrentScanlineRegister.Scanline == Scanlines ? GpuMode.VerticalBlank : GpuMode.ReadingOam;
                        currentTimings -= HorizontalBlankClocks;
                    }
                    break;
                case GpuMode.VerticalBlank:
                    if (currentTimings >= VerticalBlankClocks)
                    {
                        if (this.gpuRegisters.CurrentScanlineRegister.Scanline == VertaicalBlankScanlines)
                        {
                            // Reset
                            this.gpuRegisters.CurrentScanlineRegister.Register = 0x00;
                            lcdStatusRegister.GpuMode = GpuMode.ReadingOam;
                            currentTimings -= VerticalBlankClocks;
                            this.interruptFlagsRegister.UpdateInterrupts(InterruptFlag.VerticalBlank);
                            break;
                        }

                        this.gpuRegisters.CurrentScanlineRegister.IncrementScanline();
                        currentTimings -= VerticalBlankClocks;
                    }
                    break;
                case GpuMode.ReadingOam:
                    if (currentTimings >= ReadingOamClocks)
                    {
                        lcdStatusRegister.GpuMode = GpuMode.ReadingVram;
                        currentTimings -= ReadingOamClocks;
                    }
                    break;
                case GpuMode.ReadingVram:
                    if (currentTimings >= ReadingVramClocks)
                    {
                        paintingTaskCompletionSource?.TrySetResult(true);

                        lcdStatusRegister.GpuMode = GpuMode.HorizonalBlank;
                        currentTimings -= ReadingVramClocks;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task PaintLoop()
        {
            while (!disposed)
            {
                var result = await paintingTaskCompletionSource.Task.ConfigureAwait(false);
                if (!result)
                {
                    break;
                }

                paintingTaskCompletionSource = null;

                Paint();

                paintingTaskCompletionSource = new TaskCompletionSource<bool>();
            }
        }
        
        private void Paint()
        {
            var renderSettings = this.gpuRegisters.LcdControlRegister.BackgroundTileMap
                ? new RenderSettings(0x1c00,
                                     0x800,
                                     true,
                                     gpuRegisters.ScrollXRegister.Register,
                                     gpuRegisters.ScrollYRegister.Register,
                                     gpuRegisters.LcdControlRegister.SpriteSize,
                                     gpuRegisters.LcdControlRegister.SpriteDisplayEnable)
                : new RenderSettings(0x1800,
                                     0x0,
                                     false,
                                     gpuRegisters.ScrollXRegister.Register,
                                     gpuRegisters.ScrollYRegister.Register,
                                     gpuRegisters.LcdControlRegister.SpriteSize,
                                     gpuRegisters.LcdControlRegister.SpriteDisplayEnable);

            var tileMapBytes = this.tileRam.ReadBytes(renderSettings.TileMapAddress, 0x400);
            var tileSetBytes = tileRam.ReadBytes(renderSettings.TileSetAddress, 0x1000);
            var spriteBytes = spriteRam.ReadBytes(0x0, 0xa0);
            var spriteTileSetBytes = renderSettings.TileSetAddress == 0 ? tileSetBytes : tileRam.ReadBytes(0x0, 0x1000);

            if (renderSettings.Equals(lastRenderSettings) && tileMapBytes.SequenceEquals(lastTileMapBytes) &&
                tileSetBytes.SequenceEquals(lastTileSetBytes) && spriteBytes.SequenceEquals(lastSpriteBytes) &&
                spriteTileSetBytes.SequenceEquals(lastSpriteTileSetBytes))
            {
                // No need to render the same frame twice.
                return;
            }

            lastRenderSettings = renderSettings;
            lastTileMapBytes = tileMapBytes;
            lastTileSetBytes = tileSetBytes;
            lastSpriteBytes = spriteBytes;
            lastSpriteTileSetBytes = spriteTileSetBytes;

            var tileMap = new TileMap(renderSettings, tileSetBytes, tileMapBytes, spriteBytes, spriteTileSetBytes);
            var frameBounds = new Rectangle(0, 0, LcdWidth, LcdHeight);

            var bitmapPalette = lcdBuffer.Palette;
            foreach (var palette in gpuRegisters.LcdMonochromePaletteRegister.Pallette)
            {
                bitmapPalette.Entries[(int) palette.Key] = gameBoyConfig.MonocromePalette[palette.Value];
            }
            lcdBuffer.Palette = bitmapPalette;

            var frameData = lcdBuffer.LockBits(frameBounds, ImageLockMode.ReadOnly, lcdBuffer.PixelFormat);

            var buffer = new byte[frameData.Stride];
            var ptr = frameData.Scan0;
            for (var y = 0; y < LcdHeight; y++)
            {
                for (var x = 0; x < LcdWidth; x++)
                {
                    buffer[x] = (byte) tileMap.Get;

                    if (x + 1 < LcdWidth)
                    {
                        tileMap.NextColumn();
                    }
                }
                
                Marshal.Copy(buffer, 0, ptr, buffer.Length);

                if (y + 1 < LcdHeight)
                {
                    tileMap.NextRow();
                    ptr += frameData.Stride;
                }
            }

            lcdBuffer.UnlockBits(frameData);
            this.renderhandler.Paint(lcdBuffer);
        }

        private class TileMap
        {
            private readonly byte[] tileSetBytes;
            private readonly byte[] tileMapBytes;
            private readonly Sprite[] allSprites;
            private readonly byte[] spriteTileSetBytes;
            private readonly IDictionary<int, Tile> tileCache;
            private readonly IDictionary<byte, Tile> spriteTileCache;

            private Tile currentTile;
            private Sprite[] currentSprites;

            private readonly RenderSettings renderSettings;

            private int column;
            private int tileMapColumn;
            private int tileColumn;
            private int row;
            private int tileMapRow;
            private int tileRow;

            public TileMap(RenderSettings renderSettings, byte[] tileSetBytes, byte[] tileMapBytes, byte[] spriteBytes, byte[] spriteTileSetBytes)
            {
                this.renderSettings = renderSettings;

                column = renderSettings.ScrollX;
                tileMapColumn = renderSettings.ScrollX / 8;
                tileColumn = renderSettings.ScrollX % 8;

                row = renderSettings.ScrollY;
                tileMapRow = renderSettings.ScrollY / 8;
                tileRow = renderSettings.ScrollY % 8;

                // TODO: tile sets and caches can be shared when sprite and background set to same set.
                this.tileSetBytes = tileSetBytes;
                this.tileMapBytes = tileMapBytes;
                this.spriteTileSetBytes = spriteTileSetBytes;
                this.allSprites = renderSettings.SpritesEnabled ? GetAllSprites(spriteBytes).ToArray() : new Sprite[0];
                this.tileCache = new Dictionary<int, Tile>();
                this.spriteTileCache = new Dictionary<byte, Tile>();

                UpdateCurrentTile();
                UpdateRowSprites();
            }

            public Palette Get
            {
                get
                {
                    var background = currentTile.Get(tileRow, tileColumn);
                    if (!renderSettings.SpritesEnabled)
                    {
                        return background;
                    }

                    foreach (var sprite in currentSprites.Where(s => column >= s.X && column < s.X + 8))
                    {
                        // TODO: 8x16 sprites.
                        // TODO: Background priority sprites.
                        Tile spriteTile;
                        if (spriteTileCache.ContainsKey(sprite.TileNumber))
                        {
                            spriteTile = spriteTileCache[sprite.TileNumber];
                        }
                        else
                        {
                            spriteTileCache[sprite.TileNumber] = spriteTile = GetTile(spriteTileSetBytes, sprite.TileNumber * 16);
                        }

                        return spriteTile.Get(row - sprite.Y, column - sprite.X);
                    }

                    return background;
                }
            }
            
            public void NextColumn()
            {
                column++;
                tileColumn = (tileColumn + 1) % 8;
                if (tileColumn == 0)
                {
                    tileMapColumn++;
                    UpdateCurrentTile();
                }
            }

            public void NextRow()
            {
                row++;
                tileRow = (tileRow + 1) % 8;
                if (tileRow == 0)
                {
                    tileMapRow++;
                }

                UpdateRowSprites();

                // Reset column.
                column = renderSettings.ScrollX;
                tileMapColumn = renderSettings.ScrollX / 8;
                tileColumn = renderSettings.ScrollX % 8;
                UpdateCurrentTile();
            }

            private void UpdateRowSprites()
            {
                // TODO: Multiple sprite priority.
                // TODO: Max 10 sprites per scan.
                // TODO: 8x16 sprites.
                currentSprites = allSprites.Where(s => row >= s.Y && row < s.Y + 8).ToArray();
            }

            private void UpdateCurrentTile()
            {
                var tileMapIndex = tileMapRow * 32 + tileMapColumn;
                var tileMapValue = tileMapBytes[tileMapIndex];
                var tileSetIndex = renderSettings.TileSetIsSigned ? (sbyte)tileMapValue + 128 : tileMapValue;
                if (tileCache.ContainsKey(tileSetIndex))
                {
                    currentTile = tileCache[tileSetIndex];
                }
                else
                {
                    tileCache[tileSetIndex] = currentTile = GetTile(tileSetBytes, tileSetIndex * 16);
                }
            }

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
                        palette[col + baseIndex] = (Palette)(((low & mask) > 0 ? 1 : 0) + ((high & mask) > 0 ? 2 : 0));
                    }
                }

                return new Tile(palette);
            }

            private static IEnumerable<Sprite> GetAllSprites(byte[] buffer)
            {
                using (var stream = new MemoryStream(buffer))
                {
                    for (var i = 0; i < 40; i++)
                    {
                        var y = stream.ReadByte() - 16;
                        var x = stream.ReadByte() - 8;
                        var n = (byte)stream.ReadByte();
                        var flags = stream.ReadByte();

                        if (x <= 0 || x >= LcdWidth || y <= 0 || y >= LcdHeight)
                        {
                            // Off screen sprite.
                            continue;
                        }

                        yield return
                            new Sprite((byte)x,
                                       (byte)y,
                                       n,
                                       (flags & 0x08) > 0,
                                       (flags & 0x04) > 0,
                                       (flags & 0x02) > 0,
                                       (flags & 0x01) > 0);
                    }
                }
            }
        }
        

        private struct RenderSettings
        {
            public readonly ushort TileMapAddress;
            public readonly ushort TileSetAddress;
            public readonly bool TileSetIsSigned;
            public readonly byte ScrollX;
            public readonly byte ScrollY;
            public readonly byte SpriteHeight;
            public readonly bool SpritesEnabled;

            public RenderSettings(ushort tileMapAddress, ushort tileSetAddress, bool tileSetIsSigned, byte scrollX, byte scrollY, bool bigSprites, bool spritesEnabled) : this()
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
            /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false. 
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            lock (disposingContext)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
            }
            
            paintingTaskCompletionSource?.TrySetResult(false);
        }
    }
}
