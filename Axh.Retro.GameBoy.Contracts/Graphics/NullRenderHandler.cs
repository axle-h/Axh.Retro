using System.Drawing;

namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    public class NullRenderHandler : IRenderHandler
    {
        public void Paint(Bitmap frame)
        {
        }
    }
}