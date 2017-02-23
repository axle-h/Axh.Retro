using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Axh.Retro.GameBoy.Wpf.Config;
using Frame = Axh.Retro.GameBoy.Contracts.Graphics.Frame;

namespace Axh.Retro.GameBoy.Wpf
{
    /// <summary>
    /// A brutally naive WPF window that renders the GameBoy GPU.
    /// </summary>
    /// <seealso cref="Axh.Retro.GameBoy.Contracts.Graphics.IRenderHandler" />
    internal class SimpleLcd : IRenderHandler
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IHardwareRegisters _hardware;
        private readonly IWpfConfig _config;
        private Image _image;
        private Window _window;
        private WriteableBitmap _writeableBitmap;
        private Label _metricsLabel;

        private static readonly IDictionary<byte, Color> _monocromePalette = new Dictionary<byte, Color>
                {
                    [(byte) MonochromeShade.White] = Color.FromRgb(255, 255, 255),
                    [(byte) MonochromeShade.LightGray] = Color.FromRgb(192, 192, 192),
                    [(byte) MonochromeShade.DarkGray] = Color.FromRgb(96, 96, 96),
                    [(byte) MonochromeShade.Black] = Color.FromRgb(0, 0, 0)
                };

        public SimpleLcd(CancellationTokenSource cancellationTokenSource, IHardwareRegisters hardware, IWpfConfig config)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _hardware = hardware;
            _config = config;
            StartStaTask(Init);
        }

        /// <summary>
        /// Called every time the GB LCD is updated.
        /// Don't hang on to frame instances.
        /// </summary>
        /// <param name="frame"></param>
        public void Paint(Frame frame) => _window.Dispatcher.Invoke(() => UpdateUi(frame));

        /// <summary>
        /// Updates the rendering metrics.
        /// The render handler can choose to display this if required.
        /// </summary>
        /// <param name="fps">The total frames rendered in the last second.</param>
        /// <param name="skippedFrames">The skipped frames.</param>
        public void UpdateMetrics(int fps, int skippedFrames)
        {
            _window.Dispatcher.Invoke(() => _metricsLabel.Content = $"fps: {fps}, frame skip: {skippedFrames}");
        }

        private void Init()
        {
            _image = new Image
                     {
                         Stretch = Stretch.Fill,
                         HorizontalAlignment = HorizontalAlignment.Left,
                         VerticalAlignment = VerticalAlignment.Top
                     };

            _metricsLabel = new Label();

            var panel = new StackPanel();
            
            panel.Children.Add(_metricsLabel);
            panel.Children.Add(_image);

            _window = new Window { Content = panel };
            var behavior = new KeyboardJoyPadBehavior(_hardware.JoyPad, _config);
            Interaction.GetBehaviors(_window).Add(behavior);

            RenderOptions.SetBitmapScalingMode(_image, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(_image, EdgeMode.Aliased);

            _window.Show();

            var app = new Application();
            app.Exit += (sender, args) => _cancellationTokenSource.Cancel();

            app.Run(_window);
        }

        private static Task StartStaTask(Action func)
        {
            var tcs = new TaskCompletionSource<bool>();
            var thread = new Thread(() =>
                                    {
                                        try
                                        {
                                            func();
                                            tcs.SetResult(true);
                                        }
                                        catch (Exception e)
                                        {
                                            tcs.SetException(e);
                                        }
                                    });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

        public void UpdateUi(Frame frame)
        {
            if (_image.Source == null)
            {
                var palette = _monocromePalette.Values.ToList();
                _writeableBitmap = new WriteableBitmap(frame.Width,
                                                       frame.Height,
                                                       96,
                                                       96,
                                                       PixelFormats.Indexed8,
                                                       new BitmapPalette(palette));

                _image.Width = frame.Width * 4;
                _image.Height = frame.Height * 4;
                _image.Source = _writeableBitmap;
            }
            
            // Copy managed source buffer to unmanaged back buffer.
            _writeableBitmap.Lock();
            var targetPtr = _writeableBitmap.BackBuffer;

            for (var y = 0; y < frame.Height; y++)
            {
                var row = frame.GetRow(y);
                Marshal.Copy(row, 0, targetPtr, row.Length);
                if (y + 1 < frame.Height)
                {
                    targetPtr = targetPtr + frame.Width;
                }
            }
            
            // Specify the area of the bitmap that changed.
            _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, frame.Width, frame.Height));

            // Release the back buffer and make it available for display.
            _writeableBitmap.Unlock();
            
        }

        private bool IsPaletteOk(IEnumerable<Color> expected)
            => _writeableBitmap?.Palette?.Colors.SequenceEqual(expected) ?? false;
    }
}