using System.Runtime.InteropServices;

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
            var srcScan0 = srcData.Scan0;            
            var buffer = new byte[srcData.Stride * srcData.Height];
            Marshal.Copy(srcScan0, buffer, 0, buffer.Length);
            Marshal.Copy(buffer, 0, writeableBitmap.BackBuffer, buffer.Length);

            // Specify the area of the bitmap that changed.
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, frame.Width, frame.Height));

            // Release the back buffer and make it available for display.
            writeableBitmap.Unlock();
            frame.UnlockBits(srcData);
        }
    }
}
