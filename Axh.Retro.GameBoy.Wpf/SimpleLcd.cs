using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;

namespace Axh.Retro.GameBoy.Wpf
{
    internal class SimpleLcd : IRenderHandler
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private Image image;
        private Window window;
        private WriteableBitmap writeableBitmap;

        public SimpleLcd(CancellationTokenSource cancellationTokenSource)
        {
            this.cancellationTokenSource = cancellationTokenSource;
            StartStaTask(Init);
        }

        /// <summary>
        ///     Called every time the GB LCD is updated.
        ///     Don't hang on to frame instances.
        /// </summary>
        /// <param name="frame"></param>
        public void Paint(Bitmap frame) => window.Dispatcher.Invoke(() => UpdateUi(frame));

        private void Init()
        {
            image = new Image
            {
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            window = new Window {Content = image};

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
            
            window.Show();

            var app = new Application();
            app.Exit += (sender, args) => cancellationTokenSource.Cancel();

            app.Run(window);
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

            if (image.Source == null || !IsPaletteOk(palette))
            {
                writeableBitmap = new WriteableBitmap(frame.Width,
                                                      frame.Height,
                                                      96,
                                                      96,
                                                      PixelFormats.Indexed8,
                                                      new BitmapPalette(palette));
                image.Width = frame.Width * 4;
                image.Height = frame.Height * 4;
                image.Source = writeableBitmap;
            }

            // Reserve the back buffer for updates.
            writeableBitmap.Lock();
            var srcData = frame.LockBits(new Rectangle(0, 0, frame.Width, frame.Height),
                                         ImageLockMode.ReadOnly,
                                         frame.PixelFormat);
            var srcPtr = srcData.Scan0;
            var targetPtr = writeableBitmap.BackBuffer;

            var buffer = new byte[srcData.Stride * frame.Height];
            Marshal.Copy(srcPtr, buffer, 0, buffer.Length);
            Marshal.Copy(buffer, 0, targetPtr, buffer.Length);

            // Specify the area of the bitmap that changed.
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, frame.Width, frame.Height));

            // Release the back buffer and make it available for display.
            writeableBitmap.Unlock();
            frame.UnlockBits(srcData);
        }

        private bool IsPaletteOk(IEnumerable<Color> expected)
            => writeableBitmap?.Palette?.Colors.SequenceEqual(expected) ?? false;
    }
}