using System.Drawing;
using System.Drawing.Imaging;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.Console.Config
{
    public class RenderHandler : IRenderHandler
    {
        private int frameId;

        public void Paint(Bitmap frame)
        {
            frame.Save($"frame-{frameId++}.png", ImageFormat.Png);
        }
    }
}