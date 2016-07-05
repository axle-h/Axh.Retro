using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Config;
using Axh.Retro.CPU.Common.Contracts.Config;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Common.Memory;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Axh.Retro.GameBoy.Registers.Interfaces;
using Axh.Retro.GameBoy.Util;

namespace Axh.Retro.GameBoy.Devices
{
    public class Gpu : IGpu
    {
        private const int Scanlines = 144;
        private const int VertaicalBlankScanlines = 153;
        private const int VerticalBlankClocks = 456;
        private const int ReadingOamClocks = 80;
        private const int ReadingVramClocks = 172;
        private const int HorizontalBlankClocks = 204;

        private const int LcdWidth = 160;
        private const int LcdHeight = 144;

        private static readonly IMemoryBankConfig SpriteRamConfig = new SimpleMemoryBankConfig(
            MemoryBankType.Peripheral,
            null,
            0xfe00,
            0xa0);

        private static readonly IMemoryBankConfig MapRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral,
            null,
            0x8000,
            0x2000);

        private readonly object _disposingContext = new object();

        private readonly IGameBoyConfig _gameBoyConfig;

        private readonly IGpuRegisters _gpuRegisters;

        private readonly IInterruptFlagsRegister _interruptFlagsRegister;

        /// <summary>
        ///     Normal frame buffer.
        /// </summary>
        private readonly Bitmap _lcdBuffer;

        private readonly ILcdStatusRegister _lcdStatusRegister;

        private readonly IRenderHandler _renderhandler;

        /// <summary>
        ///     $FE00-$FE9F	OAM - Object Attribute Memory
        /// </summary>
        private readonly ArrayBackedMemoryBank _spriteRam;

        /// <summary>
        ///     $9C00-$9FFF	Tile map #1
        ///     $9800-$9BFF Tile map #0 32*32
        ///     $9000-$97FF Tile set #0: tiles 0-127
        ///     $8800-$8FFF Tile set #1: tiles 128-255 & Tile set #0: tiles -128 to -1
        ///     $8000-$87FF Tile set #1: tiles 0-127
        /// </summary>
        private readonly ArrayBackedMemoryBank _tileRam;

        private int _currentTimings;
        private bool _disposed;

        private bool _isEnabled;

        private RenderSettings _lastRenderSettings;

        // TODO: move to state object.
        private byte[] _lastTileMapBytes, _lastTileSetBytes, _lastSpriteBytes, _lastSpriteTileSetBytes;

        private TaskCompletionSource<bool> _paintingTaskCompletionSource;


        public Gpu(IGameBoyConfig gameBoyConfig,
            IInterruptFlagsRegister interruptFlagsRegister,
            IGpuRegisters gpuRegisters,
            IRenderHandler renderhandler,
            IInstructionTimer timer)
        {
            _interruptFlagsRegister = interruptFlagsRegister;
            _gpuRegisters = gpuRegisters;
            _renderhandler = renderhandler;
            _gameBoyConfig = gameBoyConfig;
            _lcdStatusRegister = gpuRegisters.LcdStatusRegister;


            _spriteRam = new ArrayBackedMemoryBank(SpriteRamConfig);
            _tileRam = new ArrayBackedMemoryBank(MapRamConfig);

            _isEnabled = false;
            _lcdStatusRegister.GpuMode = GpuMode.VerticalBlank;
            _currentTimings = 0;

            if (gameBoyConfig.RunGpu)
            {
                timer.TimingSync += Sync;
            }

            _lcdBuffer = new Bitmap(LcdWidth, LcdHeight, PixelFormat.Format8bppIndexed);
            _paintingTaskCompletionSource = new TaskCompletionSource<bool>();
            _disposed = false;

            Task.Factory.StartNew(() => PaintLoop().Wait(), TaskCreationOptions.LongRunning);
        }

        public IEnumerable<IAddressSegment> AddressSegments => new[] {_spriteRam, _tileRam};

        public void Halt()
        {
        }

        public void Resume()
        {
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            lock (_disposingContext)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
            }

            _paintingTaskCompletionSource?.TrySetResult(false);
        }

        private void Sync(InstructionTimings instructionTimings)
        {
            if (!_gpuRegisters.LcdControlRegister.LcdOperation)
            {
                if (_isEnabled)
                {
                    _lcdStatusRegister.GpuMode = GpuMode.VerticalBlank;
                    _currentTimings = 0;
                    _gpuRegisters.CurrentScanlineRegister.Register = 0x00;
                }
                return;
            }

            _isEnabled = true;

            var timings = instructionTimings.MachineCycles;
            _currentTimings += timings;

            switch (_lcdStatusRegister.GpuMode)
            {
                case GpuMode.HorizonalBlank:
                    if (_currentTimings >= HorizontalBlankClocks)
                    {
                        _gpuRegisters.CurrentScanlineRegister.IncrementScanline();
                        _lcdStatusRegister.GpuMode = _gpuRegisters.CurrentScanlineRegister.Scanline == Scanlines
                            ? GpuMode.VerticalBlank
                            : GpuMode.ReadingOam;
                        _currentTimings -= HorizontalBlankClocks;
                    }
                    break;
                case GpuMode.VerticalBlank:
                    if (_currentTimings >= VerticalBlankClocks)
                    {
                        if (_gpuRegisters.CurrentScanlineRegister.Scanline == VertaicalBlankScanlines)
                        {
                            // Reset
                            _gpuRegisters.CurrentScanlineRegister.Register = 0x00;
                            _lcdStatusRegister.GpuMode = GpuMode.ReadingOam;
                            _currentTimings -= VerticalBlankClocks;
                            _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.VerticalBlank);
                            break;
                        }

                        _gpuRegisters.CurrentScanlineRegister.IncrementScanline();
                        _currentTimings -= VerticalBlankClocks;
                    }
                    break;
                case GpuMode.ReadingOam:
                    if (_currentTimings >= ReadingOamClocks)
                    {
                        _lcdStatusRegister.GpuMode = GpuMode.ReadingVram;
                        _currentTimings -= ReadingOamClocks;
                    }
                    break;
                case GpuMode.ReadingVram:
                    if (_currentTimings >= ReadingVramClocks)
                    {
                        _paintingTaskCompletionSource?.TrySetResult(true);

                        _lcdStatusRegister.GpuMode = GpuMode.HorizonalBlank;
                        _currentTimings -= ReadingVramClocks;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task PaintLoop()
        {
            while (!_disposed)
            {
                var result = await _paintingTaskCompletionSource.Task.ConfigureAwait(false);
                if (!result)
                {
                    break;
                }

                _paintingTaskCompletionSource = null;

                Paint();

                _paintingTaskCompletionSource = new TaskCompletionSource<bool>();
            }
        }

        private void Paint()
        {
            var renderSettings = _gpuRegisters.LcdControlRegister.BackgroundTileMap
                ? new RenderSettings(0x1c00,
                    0x800,
                    true,
                    _gpuRegisters.ScrollXRegister.Register,
                    _gpuRegisters.ScrollYRegister.Register,
                    _gpuRegisters.LcdControlRegister.SpriteSize,
                    _gpuRegisters.LcdControlRegister.SpriteDisplayEnable)
                : new RenderSettings(0x1800,
                    0x0,
                    false,
                    _gpuRegisters.ScrollXRegister.Register,
                    _gpuRegisters.ScrollYRegister.Register,
                    _gpuRegisters.LcdControlRegister.SpriteSize,
                    _gpuRegisters.LcdControlRegister.SpriteDisplayEnable);

            var tileMapBytes = _tileRam.ReadBytes(renderSettings.TileMapAddress, 0x400);
            var tileSetBytes = _tileRam.ReadBytes(renderSettings.TileSetAddress, 0x1000);
            var spriteBytes = _spriteRam.ReadBytes(0x0, 0xa0);
            var spriteTileSetBytes = renderSettings.TileSetAddress == 0 ? tileSetBytes : _tileRam.ReadBytes(0x0, 0x1000);

            if (renderSettings.Equals(_lastRenderSettings) && tileMapBytes.SequenceEquals(_lastTileMapBytes) &&
                tileSetBytes.SequenceEquals(_lastTileSetBytes) && spriteBytes.SequenceEquals(_lastSpriteBytes) &&
                spriteTileSetBytes.SequenceEquals(_lastSpriteTileSetBytes))
            {
                // No need to render the same frame twice.
                return;
            }

            _lastRenderSettings = renderSettings;
            _lastTileMapBytes = tileMapBytes;
            _lastTileSetBytes = tileSetBytes;
            _lastSpriteBytes = spriteBytes;
            _lastSpriteTileSetBytes = spriteTileSetBytes;

            var tileMap = new TileMap(renderSettings, tileSetBytes, tileMapBytes, spriteBytes, spriteTileSetBytes);
            var frameBounds = new Rectangle(0, 0, LcdWidth, LcdHeight);

            var bitmapPalette = _lcdBuffer.Palette;
            foreach (var palette in _gpuRegisters.LcdMonochromePaletteRegister.Pallette)
            {
                bitmapPalette.Entries[(int) palette.Key] = _gameBoyConfig.MonocromePalette[palette.Value];
            }
            _lcdBuffer.Palette = bitmapPalette;

            var frameData = _lcdBuffer.LockBits(frameBounds, ImageLockMode.ReadOnly, _lcdBuffer.PixelFormat);

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

            _lcdBuffer.UnlockBits(frameData);
            _renderhandler.Paint(_lcdBuffer);
        }

        private class TileMap
        {
            private readonly Sprite[] _allSprites;

            private readonly RenderSettings _renderSettings;
            private readonly IDictionary<byte, Tile> _spriteTileCache;
            private readonly byte[] _spriteTileSetBytes;
            private readonly IDictionary<int, Tile> _tileCache;
            private readonly byte[] _tileMapBytes;
            private readonly byte[] _tileSetBytes;

            private int _column;
            private Sprite[] _currentSprites;

            private Tile _currentTile;
            private int _row;
            private int _tileColumn;
            private int _tileMapColumn;
            private int _tileMapRow;
            private int _tileRow;

            public TileMap(RenderSettings renderSettings,
                byte[] tileSetBytes,
                byte[] tileMapBytes,
                byte[] spriteBytes,
                byte[] spriteTileSetBytes)
            {
                _renderSettings = renderSettings;

                _column = renderSettings.ScrollX;
                _tileMapColumn = renderSettings.ScrollX / 8;
                _tileColumn = renderSettings.ScrollX % 8;

                _row = renderSettings.ScrollY;
                _tileMapRow = renderSettings.ScrollY / 8;
                _tileRow = renderSettings.ScrollY % 8;

                // TODO: tile sets and caches can be shared when sprite and background set to same set.
                _tileSetBytes = tileSetBytes;
                _tileMapBytes = tileMapBytes;
                _spriteTileSetBytes = spriteTileSetBytes;
                _allSprites = renderSettings.SpritesEnabled ? GetAllSprites(spriteBytes).ToArray() : new Sprite[0];
                _tileCache = new Dictionary<int, Tile>();
                _spriteTileCache = new Dictionary<byte, Tile>();

                UpdateCurrentTile();
                UpdateRowSprites();
            }

            public Palette Get
            {
                get
                {
                    var background = _currentTile.Get(_tileRow, _tileColumn);
                    if (!_renderSettings.SpritesEnabled)
                    {
                        return background;
                    }

                    foreach (var sprite in _currentSprites.Where(s => _column >= s.X && _column < s.X + 8))
                    {
                        // TODO: 8x16 sprites.
                        // TODO: Background priority sprites.
                        Tile spriteTile;
                        if (_spriteTileCache.ContainsKey(sprite.TileNumber))
                        {
                            spriteTile = _spriteTileCache[sprite.TileNumber];
                        }
                        else
                        {
                            _spriteTileCache[sprite.TileNumber] =
                                spriteTile = GetTile(_spriteTileSetBytes, sprite.TileNumber * 16);
                        }

                        return spriteTile.Get(_row - sprite.Y, _column - sprite.X);
                    }

                    return background;
                }
            }

            public void NextColumn()
            {
                _column++;
                _tileColumn = (_tileColumn + 1) % 8;
                if (_tileColumn == 0)
                {
                    _tileMapColumn++;
                    UpdateCurrentTile();
                }
            }

            public void NextRow()
            {
                _row++;
                _tileRow = (_tileRow + 1) % 8;
                if (_tileRow == 0)
                {
                    _tileMapRow++;
                }

                UpdateRowSprites();

                // Reset column.
                _column = _renderSettings.ScrollX;
                _tileMapColumn = _renderSettings.ScrollX / 8;
                _tileColumn = _renderSettings.ScrollX % 8;
                UpdateCurrentTile();
            }

            private void UpdateRowSprites()
            {
                // TODO: Multiple sprite priority.
                // TODO: Max 10 sprites per scan.
                // TODO: 8x16 sprites.
                _currentSprites = _allSprites.Where(s => _row >= s.Y && _row < s.Y + 8).ToArray();
            }

            private void UpdateCurrentTile()
            {
                var tileMapIndex = _tileMapRow * 32 + _tileMapColumn;
                var tileMapValue = _tileMapBytes[tileMapIndex];
                var tileSetIndex = _renderSettings.TileSetIsSigned ? (sbyte) tileMapValue + 128 : tileMapValue;
                if (_tileCache.ContainsKey(tileSetIndex))
                {
                    _currentTile = _tileCache[tileSetIndex];
                }
                else
                {
                    _tileCache[tileSetIndex] = _currentTile = GetTile(_tileSetBytes, tileSetIndex * 16);
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
                        palette[col + baseIndex] = (Palette) (((low & mask) > 0 ? 1 : 0) + ((high & mask) > 0 ? 2 : 0));
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
                        var n = (byte) stream.ReadByte();
                        var flags = stream.ReadByte();

                        if (x <= 0 || x >= LcdWidth || y <= 0 || y >= LcdHeight)
                        {
                            // Off screen sprite.
                            continue;
                        }

                        yield return
                            new Sprite((byte) x,
                                (byte) y,
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

            public RenderSettings(ushort tileMapAddress,
                ushort tileSetAddress,
                bool tileSetIsSigned,
                byte scrollX,
                byte scrollY,
                bool bigSprites,
                bool spritesEnabled) : this()
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
            ///     Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <returns>
            ///     true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
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
            ///     Returns the hash code for this instance.
            /// </summary>
            /// <returns>
            ///     A 32-bit signed integer that is the hash code for this instance.
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
    }
}