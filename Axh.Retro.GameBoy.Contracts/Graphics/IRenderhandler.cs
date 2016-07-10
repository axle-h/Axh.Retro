using System.Drawing;

namespace Axh.Retro.GameBoy.Contracts.Graphics
{
    /// <summary>
    /// An external render handler.
    /// </summary>
    public interface IRenderHandler
    {
        /// <summary>
        /// Called every time the GB LCD is updated.
        /// The frame is updated and locked for each GPU cycle. So implementations should block as long as they are accessing pixels/buffers.
        /// Obviously the longer you block, the more frames will be skipped.
        /// </summary>
        /// <param name="frame"></param>
        void Paint(Bitmap frame);
    }
}