namespace Axh.Retro.GameBoy.Wpf
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Axh.Retro.GameBoy.Contracts.Graphics;
    using Image = System.Windows.Controls.Image;

    internal class SimpleLcd : IRenderHandler
    {
        private WriteableBitmap writeableBitmap;
        private Window window;

        public SimpleLcd()
        {
            StartStaTask(Init);
        }

        private void Init()
        {
            var image = new Image
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            window = new Window { Content = image };

            writeableBitmap = new WriteableBitmap(160, 144, 96, 96, PixelFormats.Bgr32, null);


            image.Source = writeableBitmap;

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

            window.Show();

            var app = new Application();
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
        
        /// <summary>
        /// Called every time the GB LCD is updated.
        /// Don't hang on to frame instances.
        /// </summary>
        /// <param name="frame"></param>
        public void Paint(Bitmap frame)
        {
            window.Dispatcher.Invoke(() => UpdateUi(frame));
        }

        public void UpdateUi(Bitmap frame)
        {
            // Reserve the back buffer for updates.
            writeableBitmap.Lock();
            var srcData = frame.LockBits(new Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.ReadOnly, frame.PixelFormat);

            unsafe
            {
                // Get a pointer to the back buffer.
                var pBackBuffer = (int*)writeableBitmap.BackBuffer;
                var srcScan0 = (int*)srcData.Scan0;

                var numPixels = srcData.Stride / 4 * srcData.Height;
                for (var p = 0; p < numPixels; p++)
                {
                    pBackBuffer[p] = srcScan0[p];
                }
            }

            // Specify the area of the bitmap that changed.
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, frame.Width, frame.Height));

            // Release the back buffer and make it available for display.
            writeableBitmap.Unlock();
            frame.UnlockBits(srcData);
        }
    }
}
