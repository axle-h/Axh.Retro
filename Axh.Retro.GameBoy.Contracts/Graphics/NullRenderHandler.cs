using System.Drawing;

namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    /// <summary>
    /// A render handler that does nothing.
    /// </summary>
    /// <seealso cref="Axh.Retro.GameBoy.Contracts.Graphics.IRenderHandler" />
    public class NullRenderHandler : IRenderHandler
    {
        /// <summary>
        /// Called every time the GB LCD is updated.
        /// The frame is updated and locked for each GPU cycle. So implementations should block as long as they are accessing pixels/buffers.
        /// Obviously the longer you block, the more frames will be skipped.
        /// </summary>
        /// <param name="frame"></param>
        public void Paint(Bitmap frame)
        {
        }
    }
}