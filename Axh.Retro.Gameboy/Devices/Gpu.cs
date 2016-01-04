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
        private const int FrameBufferDimension = 32 * 8;


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
        
        /// <summary>
        /// We expect the GPU to reuse tiles often. Better to cache the result of parsing tiles than process them every frame.
        /// TODO: How big will this grow?
        /// </summary>
        private readonly IDictionary<Guid, Tile> tileCache;

        /// <summary>
        /// Normal frame buffer.
        /// </summary>
        private readonly Bitmap frameBuffer;

        /// <summary>
        /// Double width and height buffer for when scrolling overflows the frame buffer.
        /// </summary>
        private readonly Bitmap scrollOverflowFrameBuffer;

        private static readonly Rectangle FrameBounds = new Rectangle(0, 0, FrameBufferDimension, FrameBufferDimension);
        private static readonly Rectangle OverscanXBounds = new Rectangle(FrameBufferDimension, 0, FrameBufferDimension, FrameBufferDimension);
        private static readonly Rectangle OverscanYBounds = new Rectangle(0, FrameBufferDimension, FrameBufferDimension, FrameBufferDimension);
        private static readonly Rectangle OverscanXYBounds = new Rectangle(FrameBufferDimension, FrameBufferDimension, FrameBufferDimension, FrameBufferDimension);

        private GpuMode mode;
        
        private int currentTimings;

        private bool isEnabled;

        private byte[] lastTileMapBytes;

        private byte[] lastTileSetBytes;

        private RenderSettings lastRenderSettings;

        public Gpu(IGameBoyConfig gameBoyConfig, IGameBoyInterruptManager interruptManager, IGpuRegisters gpuRegisters, IRenderHandler renderhandler, IInstructionTimer timer)
        {
            this.interruptManager = interruptManager;
            this.gpuRegisters = gpuRegisters;
            this.renderhandler = renderhandler;
            
            this.spriteRam = new ArrayBackedMemoryBank(SpriteRamConfig);
            this.tileRam = new ArrayBackedMemoryBank(MapRamConfig);
            this.tileCache = new Dictionary<Guid, Tile>();

            this.isEnabled = false;
            this.mode = GpuMode.VerticalBlank;
            this.currentTimings = 0;
            
            if (gameBoyConfig.RunGpu)
            {
                timer.TimingSync += Sync;
            }

            frameBuffer = new Bitmap(FrameBufferDimension, FrameBufferDimension);
            scrollOverflowFrameBuffer = new Bitmap(FrameBufferDimension * 2, FrameBufferDimension * 2);
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
            var renderSettings = this.gpuRegisters.LcdControlRegister.BackgroundTileMap
                ? new RenderSettings(0x1c00, 0x800, true, this.gpuRegisters.ScrollXRegister.Register, this.gpuRegisters.ScrollYRegister.Register)
                : new RenderSettings(0x1800, 0x0, false, this.gpuRegisters.ScrollXRegister.Register, this.gpuRegisters.ScrollYRegister.Register);

            var tileMapBytes = this.tileRam.ReadBytes(renderSettings.TileMapAddress, 0x400);
            var tileSetBytes = tileRam.ReadBytes(renderSettings.TileSetAddress, 0x1000);
            
            if (renderSettings.Equals(lastRenderSettings) && tileMapBytes.SequenceEquals(lastTileMapBytes) && tileSetBytes.SequenceEquals(lastTileSetBytes))
            {
                return;
            }

            lastRenderSettings = renderSettings;
            lastTileMapBytes = tileMapBytes;
            lastTileSetBytes = tileSetBytes;

            var tileSet = GetTileSet(tileSetBytes).ToArray();
            var tileMap = GetTileMap(tileMapBytes, tileSet, renderSettings.TileSetIsSigned);
            
            // TODO get background palette register
            var colors = new Dictionary<Palette, Color>
                         {
                             { Palette.Colour0, Color.FromArgb(255, 255, 255) },
                             { Palette.Colour1, Color.FromArgb(192, 192, 192) },
                             { Palette.Colour2, Color.FromArgb(96, 96, 96) },
                             { Palette.Colour3, Color.FromArgb(0, 0, 0) }
                         };

            // Paint all 32 * 32 tiles for now
            for (var r = 0; r < 32; r++)
            {
                var y = r * 8;
                for (var c = 0; c < 32; c++)
                {
                    DrawTile(tileMap[r][c], c * 8, y, colors);
                }
            }

            // Detect and fix scroll overflow.
            var lcdBounds = new Rectangle(renderSettings.ScrollX, renderSettings.ScrollY, LcdWidth, LcdHeight);
            if (!FrameBounds.Contains(lcdBounds))
            {
                var overscanX = renderSettings.ScrollX > FrameBufferDimension - LcdWidth;
                var overscanY = renderSettings.ScrollY > FrameBufferDimension - LcdHeight;

                // Copy primary framebuffer to satisfy overlaps.
                DrawOverscan(overscanX, overscanY);
                
                this.renderhandler.Paint(scrollOverflowFrameBuffer.Clone(lcdBounds, scrollOverflowFrameBuffer.PixelFormat));
            }

            this.renderhandler.Paint(frameBuffer.Clone(lcdBounds, frameBuffer.PixelFormat));
        }

        /// <summary>
        /// Copies frameBuffer to scrollOverflowFrameBuffer and draws overscan where required.
        /// </summary>
        /// <param name="drawOverscanX"></param>
        /// <param name="drawOverscanY"></param>
        private void DrawOverscan(bool drawOverscanX, bool drawOverscanY)
        {
            var frameData = frameBuffer.LockBits(FrameBounds, ImageLockMode.ReadOnly, frameBuffer.PixelFormat);
            var bpp = Math.Abs(frameData.Stride) / frameBuffer.Width;
            var buffer = new byte[bpp * FrameBounds.Width];

            var rectangles = new List<Rectangle> {FrameBounds};
            if (drawOverscanX)
            {
                rectangles.Add(OverscanXBounds);
            }

            if (drawOverscanY)
            {
                rectangles.Add(OverscanYBounds);
            }

            if (drawOverscanX && drawOverscanY)
            {
                rectangles.Add(OverscanXYBounds);
            }

            foreach (var rectangle in rectangles)
            {
                var toData = scrollOverflowFrameBuffer.LockBits(rectangle, ImageLockMode.WriteOnly, frameBuffer.PixelFormat);
                var fromPtr = new IntPtr(frameData.Scan0.ToInt64());
                var toPtr = new IntPtr(toData.Scan0.ToInt64());

                try
                {
                    for (var y = 0; y < frameData.Height; y++, fromPtr += frameData.Stride, toPtr += toData.Stride)
                    {
                        Marshal.Copy(fromPtr, buffer, 0, buffer.Length);
                        Marshal.Copy(buffer, 0, toPtr, buffer.Length);
                    }
                }
                finally
                {
                    scrollOverflowFrameBuffer.UnlockBits(toData);
                }
            }

            frameBuffer.UnlockBits(frameData);
        }

        private IEnumerable<Tile> GetTileSet(byte[] tileSetBytes)
        {
            var address = 0;
            var buffer = new byte[16];
            for (var i = 0; i < 256; address += 16, i++)
            {
                Array.Copy(tileSetBytes, address, buffer, 0, 16);
                var id = new Guid(buffer); // So happnes that a Guid stores 16 bytes nicely

                if (this.tileCache.ContainsKey(id))
                {
                    yield return this.tileCache[id];
                }
                else
                {
                    var tile = GetTile(buffer);
                    yield return tile;
                    this.tileCache[id] = tile;
                }
            }
        }

        private void DrawTile(Tile tile, int x, int y, IDictionary<Palette, Color> colors)
        {
            for (var r = 0; r < 8; r++)
            {
                for (var c = 0; c < 8; c++)
                {
                    frameBuffer.SetPixel(x + c, y + r, colors[tile.Get(r, c)]);
                }
            }
        }

        private static Tile[][] GetTileMap(byte[] tileMapBytes, Tile[] tileSet, bool indexIsSigned)
        {
            var tileMap = new Tile[32][];
            var address = 0;
            for (var r = 0; r < 32; r++)
            {
                tileMap[r] = new Tile[32];
                for (var c = 0; c < 32; address += 1, c++)
                {
                    var index = tileMapBytes[address];
                    var tile = tileSet[indexIsSigned ? (sbyte)index + 128 : index];
                    tileMap[r][c] = tile;
                }
            }
            return tileMap;
        }
        
        private static Tile GetTile(byte[] buffer)
        {
            var palette = new Palette[64];
            var address = 0;
            for (var row = 0; row < 8; row++, address += 2)
            {
                var low = buffer[address];
                var high = buffer[address + 1];
                var baseIndex = 8 * row;

                for (var col = 0; col < 8; col++)
                {
                    // Each value is a 2bit number stored in matching positions across low and high bytes
                    var mask = 0x1 << (7- col);
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
    }
}
