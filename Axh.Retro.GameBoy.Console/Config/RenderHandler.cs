namespace Axh.Retro.GameBoy.Console.Config
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using Axh.Retro.GameBoy.Contracts.Graphics;

    public class RenderHandler : IRenderHandler
    {
        private int frameId = 0;

        public void Paint(Bitmap frame)
        {
            frame.Save($"frame-{frameId++}.png", ImageFormat.Png);
        }
    }
}
