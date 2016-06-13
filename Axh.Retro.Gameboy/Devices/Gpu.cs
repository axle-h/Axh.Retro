using System.Diagnostics;
using System.Threading.Tasks;

namespace Axh.Retro.GameBoy.Devices
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Axh.Retro.CPU.Common.Config;
    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Common.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Contracts.Graphics;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Registers.Interfaces;
    using Axh.Retro.GameBoy.Util;

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

        private readonly IGameBoyInterruptManager interruptManager;

        private readonly IGpuRegisters gpuRegisters;

        private readonly IRenderHandler renderhandler;

        private readonly IGameBoyConfig gameBoyConfig;
        
        /// <summary>
        /// Normal frame buffer.
        /// </summary>
        private readonly Bitmap lcdBuffer;

        private GpuMode mode;
        
        private int currentTimings;

        private bool isEnabled;

        private byte[] lastTileMapBytes;

        private byte[] lastTileSetBytes;

        private RenderSettings lastRenderSettings;

        private TaskCompletionSource<bool> paintingTaskCompletionSource;
        private bool disposed;

        public Gpu(IGameBoyConfig gameBoyConfig, IGameBoyInterruptManager interruptManager, IGpuRegisters gpuRegisters, IRenderHandler renderhandler, IInstructionTimer timer)
        {
            this.interruptManager = interruptManager;
            this.gpuRegisters = gpuRegisters;
            this.renderhandler = renderhandler;
            this.gameBoyConfig = gameBoyConfig;
            
            this.spriteRam = new ArrayBackedMemoryBank(SpriteRamConfig);
            this.tileRam = new ArrayBackedMemoryBank(MapRamConfig);

            this.isEnabled = false;
            this.mode = GpuMode.VerticalBlank;
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

        private void Sync(object sender, TimingSyncEventArgs args)
        {
            if (!this.gpuRegisters.LcdControlRegister.LcdOperation)
            {
                if (this.isEnabled)
                {
                    this.mode = GpuMode.VerticalBlank;
                    this.currentTimings = 0;
                    this.gpuRegisters.CurrentScanlineRegister.Register = 0x00;
                }
                return;
            }

            this.isEnabled = true;
            
            var timings = args.InstructionTimings.MachineCycles;
            currentTimings += timings;
            
            switch (mode)
            {
                case GpuMode.HorizonalBlank:
                    if (currentTimings >= HorizontalBlankClocks)
                    {
                        this.gpuRegisters.CurrentScanlineRegister.IncrementScanline();
                        mode = this.gpuRegisters.CurrentScanlineRegister.Scanline == Scanlines ? GpuMode.VerticalBlank : GpuMode.ReadingOam;
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
                            mode = GpuMode.ReadingOam;
                            currentTimings -= VerticalBlankClocks;
                            this.interruptManager.UpdateInterrupts(InterruptFlag.VerticalBlank);
                            break;
                        }

                        this.gpuRegisters.CurrentScanlineRegister.IncrementScanline();
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
                        paintingTaskCompletionSource?.TrySetResult(true);
                        
                        mode = GpuMode.HorizonalBlank;
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
                var result = await paintingTaskCompletionSource.Task;
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
                ? new RenderSettings(0x1c00, 0x800, true, this.gpuRegisters.ScrollXRegister.Register, this.gpuRegisters.ScrollYRegister.Register)
                : new RenderSettings(0x1800, 0x0, false, this.gpuRegisters.ScrollXRegister.Register, this.gpuRegisters.ScrollYRegister.Register);

            var tileMapBytes = this.tileRam.ReadBytes(renderSettings.TileMapAddress, 0x400);
            var tileSetBytes = tileRam.ReadBytes(renderSettings.TileSetAddress, 0x1000);

            
            if (renderSettings.Equals(lastRenderSettings) && tileMapBytes.SequenceEquals(lastTileMapBytes) && tileSetBytes.SequenceEquals(lastTileSetBytes))
            {
                // TODO: record this.
                return;
            }

            lastRenderSettings = renderSettings;
            lastTileMapBytes = tileMapBytes;
            lastTileSetBytes = tileSetBytes;
            
            var tileMap = new TileMap(renderSettings, tileSetBytes, tileMapBytes);
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

                // TODO: Sprites.

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
            private readonly bool isSigned;
            private readonly IDictionary<int, Tile> tileCache;

            private Tile currentTile;

            private readonly int scrollX;

            private int tileMapColumn;
            private int tileColumn;
            private int tileMapRow;
            private int tileRow;

            public TileMap(RenderSettings renderSettings, byte[] tileSetBytes, byte[] tileMapBytes)
            {
                this.scrollX = renderSettings.ScrollX;
                tileMapColumn = renderSettings.ScrollX / 8;
                tileColumn = renderSettings.ScrollX % 8;
                tileMapRow = renderSettings.ScrollY / 8;
                tileRow = renderSettings.ScrollY % 8;

                this.tileSetBytes = tileSetBytes;
                this.tileMapBytes = tileMapBytes;
                this.isSigned = renderSettings.TileSetIsSigned;
                this.tileCache = new Dictionary<int, Tile>();
                UpdateCurrentTile();
            }

            public Palette Get => currentTile.Get(tileRow, tileColumn);
            
            public void NextColumn()
            {
                tileColumn = (tileColumn + 1) % 8;
                if (tileColumn == 0)
                {
                    tileMapColumn++;
                    UpdateCurrentTile();
                }
            }

            public void NextRow()
            {
                tileRow = (tileRow + 1) % 8;
                if (tileRow == 0)
                {
                    tileMapRow++;
                    UpdateCurrentTile();
                }

                // Reset column.
                tileMapColumn = scrollX / 8;
                tileColumn = scrollX % 8;
                UpdateCurrentTile();
            }

            private void UpdateCurrentTile()
            {
                var tileMapIndex = tileMapRow * 32 + tileMapColumn;
                var tileMapValue = tileMapBytes[tileMapIndex];
                var tileSetIndex = isSigned ? (sbyte)tileMapValue + 128 : tileMapValue;
                if (tileCache.ContainsKey(tileSetIndex))
                {
                    currentTile = tileCache[tileSetIndex];
                }
                else
                {
                    tileCache[tileSetIndex] = currentTile = GetTile(tileSetBytes, tileSetIndex * 16);
                }
            }
        }
        
        private static Tile GetTile(byte[] buffer, int offset = 0)
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

        private struct RenderSettings
        {
            public readonly ushort TileMapAddress;
            public readonly ushort TileSetAddress;
            public readonly bool TileSetIsSigned;
            public readonly byte ScrollX;
            public readonly byte ScrollY;

            public RenderSettings(ushort tileMapAddress, ushort tileSetAddress, bool tileSetIsSigned, byte scrollX, byte scrollY) : this()
            {
                TileMapAddress = tileMapAddress;
                TileSetAddress = tileSetAddress;
                TileSetIsSigned = tileSetIsSigned;
                ScrollX = scrollX;
                ScrollY = scrollY;
            }

            public bool Equals(RenderSettings other)
            {
                return TileMapAddress == other.TileMapAddress && TileSetAddress == other.TileSetAddress && TileSetIsSigned == other.TileSetIsSigned && ScrollX == other.ScrollX && ScrollY == other.ScrollY;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                return obj is RenderSettings && Equals((RenderSettings)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = TileMapAddress.GetHashCode();
                    hashCode = (hashCode * 397) ^ TileSetAddress.GetHashCode();
                    hashCode = (hashCode * 397) ^ TileSetIsSigned.GetHashCode();
                    hashCode = (hashCode * 397) ^ ScrollX.GetHashCode();
                    hashCode = (hashCode * 397) ^ ScrollY.GetHashCode();
                    return hashCode;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            disposed = true;
            paintingTaskCompletionSource?.TrySetResult(false);
        }
    }
}
