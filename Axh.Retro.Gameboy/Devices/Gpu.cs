using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using Axh.Retro.CPU.Common.Config;
using Axh.Retro.CPU.Common.Contracts.Config;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Common.Memory;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Devices
{
    /// <summary>
    /// The GameBoy GPU.
    /// </summary>
    /// <seealso cref="Axh.Retro.GameBoy.Contracts.Graphics.IGpu" />
    public class Gpu : IGpu
    {
        private const int ScanLines = 144;
        private const int VerticalBlankScanLines = 153;
        private const int VerticalBlankCycles = 4560 / (VerticalBlankScanLines - ScanLines + 1);
        private const int ReadingOamCycles = 80;
        private const int ReadingVramCycles = 172;
        private const int HorizontalBlankCycles = 204;

        public const int LcdWidth = 160;
        public const int LcdHeight = 144;

        private static readonly IMemoryBankConfig SpriteRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral,
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

        private readonly IRenderHandler _renderHandler;

        /// <summary>
        /// Normal frame buffer.
        /// </summary>
        private readonly Bitmap _lcdBuffer;

        /// <summary>
        /// $FE00-$FE9F	OAM - Object Attribute Memory
        /// </summary>
        private readonly ArrayBackedMemoryBank _spriteRam;

        /// <summary>
        /// $9C00-$9FFF	Tile map #1
        /// $9800-$9BFF Tile map #0 32*32
        /// $9000-$97FF Tile set #0: tiles 0-127
        /// $8800-$8FFF Tile set #1: tiles 128-255 & Tile set #0: tiles -128 to -1
        /// $8000-$87FF Tile set #1: tiles 0-127
        /// </summary>
        private readonly ArrayBackedMemoryBank _tileRam;

        private int _frameSkip;
        private int _framesRendered;
        private readonly Timer _metricsTimer;
        
        private int _currentTimings;
        private bool _disposed;
        

        // TODO: move to state object.
        private RenderState _lastRenderState;
        private TileMapPointer _tileMapPointer;

        private TaskCompletionSource<bool> _paintingTaskCompletionSource;

        private readonly Stopwatch _frameStopwatch;
        private const double ExpectedFrameTime = 1000 / 60.0; // 60 fps => ~16.6ms.

        /// <summary>
        /// Initializes a new instance of the <see cref="Gpu"/> class.
        /// </summary>
        /// <param name="gameBoyConfig">The game boy configuration.</param>
        /// <param name="interruptFlagsRegister">The interrupt flags register.</param>
        /// <param name="gpuRegisters">The gpu registers.</param>
        /// <param name="renderHandler">The renderhandler.</param>
        /// <param name="timer">The timer.</param>
        public Gpu(IGameBoyConfig gameBoyConfig,
            IInterruptFlagsRegister interruptFlagsRegister,
            IGpuRegisters gpuRegisters,
            IRenderHandler renderHandler,
            IInstructionTimer timer)
        {
            _interruptFlagsRegister = interruptFlagsRegister;
            _gpuRegisters = gpuRegisters;
            _renderHandler = renderHandler;
            _gameBoyConfig = gameBoyConfig;

            _spriteRam = new ArrayBackedMemoryBank(SpriteRamConfig);
            _tileRam = new ArrayBackedMemoryBank(MapRamConfig);
            
            _gpuRegisters.GpuMode = GpuMode.VerticalBlank;
            _currentTimings = 0;

            _gpuRegisters.GpuMode = GpuMode.VerticalBlank;
            _currentTimings = 0;
            _gpuRegisters.CurrentScanlineRegister.Scanline = 0x92;

            if (gameBoyConfig.RunGpu)
            {
                timer.TimingSync += Sync;
            }

            _lcdBuffer = new Bitmap(LcdWidth, LcdHeight, PixelFormat.Format8bppIndexed);
            _paintingTaskCompletionSource = new TaskCompletionSource<bool>();
            _disposed = false;

            Task.Factory.StartNew(() => PaintLoop().Wait(), TaskCreationOptions.LongRunning);

            _metricsTimer = new Timer(1000);
            _metricsTimer.Elapsed += (sender, args) =>
                                     {
                                         _renderHandler.UpdateMetrics(_framesRendered, _frameSkip);
                                         _framesRendered = _frameSkip = 0;
                                     };
            _metricsTimer.Start();
            _frameStopwatch = new Stopwatch();
        }

        /// <summary>
        /// Gets the address segments.
        /// </summary>
        /// <value>
        /// The address segments.
        /// </value>
        public IEnumerable<IAddressSegment> AddressSegments => new[] { _spriteRam, _tileRam };

        /// <summary>
        /// Halts the GPU thread.
        /// </summary>
        public void Halt()
        {
            // TODO.
        }

        /// <summary>
        /// Resumes the GPU thread.
        /// </summary>
        public void Resume()
        {
            // TODO.
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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

            _metricsTimer.Dispose();
            _paintingTaskCompletionSource?.TrySetResult(false);
        }

        /// <summary>
        /// Synchronizes the GPU thread and associated registers according to the specified instruction timings.
        /// </summary>
        /// <param name="instructionTimings">The instruction timings.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        private void Sync(InstructionTimings instructionTimings)
        {
            if (!_gpuRegisters.LcdControlRegister.LcdOperation)
            {
                return;
            }
            
            _currentTimings += instructionTimings.MachineCycles;

            switch (_gpuRegisters.GpuMode)
            {
                case GpuMode.HorizontalBlank:
                    if (_currentTimings >= HorizontalBlankCycles)
                    {
                        _gpuRegisters.IncrementScanline();
                        _gpuRegisters.GpuMode = _gpuRegisters.CurrentScanlineRegister.Scanline == ScanLines - 1
                                                    ? GpuMode.VerticalBlank
                                                    : GpuMode.ReadingOam;
                        _currentTimings -= HorizontalBlankCycles;
                    }
                    break;
                case GpuMode.VerticalBlank:
                    if (_currentTimings >= VerticalBlankCycles)
                    {
                        if (_gpuRegisters.CurrentScanlineRegister.Scanline == VerticalBlankScanLines)
                        {
                            // Paint.
                            var painted = _paintingTaskCompletionSource?.TrySetResult(true);
                            if (!painted.GetValueOrDefault())
                            {
                                _frameSkip++;
                            }

                            if (!_frameStopwatch.IsRunning)
                            {
                                _frameStopwatch.Start();
                            }
                            else
                            {
                                // Check frame time.
                                var overTime = ExpectedFrameTime - _frameStopwatch.ElapsedMilliseconds;
                                if (overTime > 0)
                                {
                                    Task.Delay(TimeSpan.FromMilliseconds(overTime * 0.2)).Wait();
                                }
                                _frameStopwatch.Restart();
                            }
                            
                            // Reset
                            _gpuRegisters.CurrentScanlineRegister.Scanline = 0x00;
                            _gpuRegisters.GpuMode = GpuMode.ReadingOam;
                            _interruptFlagsRegister.UpdateInterrupts(InterruptFlag.VerticalBlank);
                            _currentTimings -= VerticalBlankCycles;
                            break;
                        }

                        _gpuRegisters.IncrementScanline();
                        _currentTimings -= VerticalBlankCycles;
                    }
                    break;
                case GpuMode.ReadingOam:
                    if (_currentTimings >= ReadingOamCycles)
                    {
                        _gpuRegisters.GpuMode = GpuMode.ReadingVram;
                        _currentTimings -= ReadingOamCycles;
                    }
                    break;
                case GpuMode.ReadingVram:
                    if (_currentTimings >= ReadingVramCycles)
                    {
                        _gpuRegisters.GpuMode = GpuMode.HorizontalBlank;
                        _currentTimings -= ReadingVramCycles;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// The paint loop task.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Paints a new frame.
        /// </summary>
        private void Paint()
        {
            var renderSettings = new RenderSettings(_gpuRegisters);

            var backgroundTileMap = _tileRam.ReadBytes(renderSettings.BackgroundTileMapAddress, 0x400);
            var tileSet = _tileRam.ReadBytes(renderSettings.TileSetAddress, 0x1000);
            var windowTileMap = renderSettings.WindowEnabled ? _tileRam.ReadBytes(renderSettings.WindowTileMapAddress, 0x400) : new byte[0];

            byte[] spriteOam, spriteTileSet;
            if (renderSettings.SpritesEnabled)
            {
                // If the background tiles are read from the sprite pattern table then we can reuse the bytes.
                spriteTileSet = renderSettings.SpriteAndBackgroundTileSetShared ? tileSet : _tileRam.ReadBytes(0x0, 0x1000);
                spriteOam = _spriteRam.ReadBytes(0x0, 0xa0);
            }
            else
            {
                spriteOam = spriteTileSet = new byte[0];
            }

            var renderState = new RenderState(renderSettings, tileSet, backgroundTileMap, windowTileMap, spriteOam, spriteTileSet);

            var renderStateChange = renderState.GetRenderStateChange(_lastRenderState);
            if (renderStateChange == RenderStateChange.None)
            {
                // No need to render the same frame twice.
                _frameSkip = 0;
                _framesRendered++;
                return;
            }

            _lastRenderState = renderState;

            _tileMapPointer = _tileMapPointer == null ? new TileMapPointer(renderState) : _tileMapPointer.Reset(renderState, renderStateChange);
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
                    buffer[x] = (byte) _tileMapPointer.Pixel;

                    if (x + 1 < LcdWidth)
                    {
                        _tileMapPointer.NextColumn();
                    }
                }

                Marshal.Copy(buffer, 0, ptr, buffer.Length);

                if (y + 1 < LcdHeight)
                {
                    _tileMapPointer.NextRow();
                    ptr += frameData.Stride;
                }
            }
            
            _lcdBuffer.UnlockBits(frameData);
            _renderHandler.Paint(_lcdBuffer);

            _frameSkip = 0;
            _framesRendered++;
        }
    }
}