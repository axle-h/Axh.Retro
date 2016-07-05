using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        public const int LcdWidth = 160;
        public const int LcdHeight = 144;

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
                    buffer[x] = (byte) tileMap.Pixel;

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
    }
}