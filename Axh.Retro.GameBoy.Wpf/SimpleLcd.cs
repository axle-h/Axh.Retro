using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;

namespace Axh.Retro.GameBoy.Wpf
{
    /// <summary>
    /// A brutally naive WPF window that renders the GameBoy GPU.
    /// </summary>
    /// <seealso cref="Axh.Retro.GameBoy.Contracts.Graphics.IRenderHandler" />
    internal class SimpleLcd : IRenderHandler
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Image _image;
        private Window _window;
        private WriteableBitmap _writeableBitmap;
        private Label _metricsLabel;

        public SimpleLcd(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
            StartStaTask(Init);
        }

        /// <summary>
        /// Called every time the GB LCD is updated.
        /// Don't hang on to frame instances.
        /// </summary>
        /// <param name="frame"></param>
        public void Paint(Bitmap frame) => _window.Dispatcher.Invoke(() => UpdateUi(frame));

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

        private static Color GetColor(System.Drawing.Color color) => Color.FromRgb(color.R, color.G, color.B);

        public void UpdateUi(Bitmap frame)
        {
            var palette = frame.Palette.Entries.Select(GetColor).ToList();

            if (_image.Source == null || !IsPaletteOk(palette))
            {
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

            // Reserve the back buffer for updates.
            _writeableBitmap.Lock();
            var srcData = frame.LockBits(new Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.ReadOnly, frame.PixelFormat);
            var srcPtr = srcData.Scan0;
            var targetPtr = _writeableBitmap.BackBuffer;

            var buffer = new byte[srcData.Stride * frame.Height];
            Marshal.Copy(srcPtr, buffer, 0, buffer.Length);
            Marshal.Copy(buffer, 0, targetPtr, buffer.Length);

            // Specify the area of the bitmap that changed.
            _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, frame.Width, frame.Height));

            // Release the back buffer and make it available for display.
            _writeableBitmap.Unlock();
            frame.UnlockBits(srcData);
        }

        private bool IsPaletteOk(IEnumerable<Color> expected)
            => _writeableBitmap?.Palette?.Colors.SequenceEqual(expected) ?? false;
    }
}