using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Axh.Retro.GameBoy.Web.Controllers.Hubs;
using Microsoft.AspNet.SignalR;

namespace Axh.Retro.GameBoy.Web.Wiring
{
    public class SignalRRenderHandler : IRenderHandler
    {
        private readonly IHubContext<IGameBoyHub> _hubContext;
        private readonly ICpuCore _core;

        public SignalRRenderHandler(ICpuCore core)
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<GameBoyHub, IGameBoyHub>();
            _core = core;
        }

        /// <summary>
        /// Called every time the GB LCD is updated.
        /// The frame is updated and locked for each GPU cycle. So implementations should block as long as they are accessing pixels/buffers.
        /// Obviously the longer you block, the more frames will be skipped.
        /// </summary>
        /// <param name="frame"></param>
        public void Paint(Bitmap frame)
        {
            // TODO: compression.
            // TODO: palette cache.

            var srcData = frame.LockBits(new Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.ReadOnly, frame.PixelFormat);
            var srcPtr = srcData.Scan0;
            var buffer = new byte[srcData.Stride * frame.Height];
            Marshal.Copy(srcPtr, buffer, 0, buffer.Length);
            frame.UnlockBits(srcData);

            _hubContext.Clients.Group(_core.CoreId.ToString()).Render(buffer);
        }

        /// <summary>
        /// Updates the rendering metrics.
        /// The render handler can choose to display this if required.
        /// </summary>
        /// <param name="fps">The total frames rendered in the last second.</param>
        /// <param name="skippedFrames">The skipped frames.</param>
        public void UpdateMetrics(int fps, int skippedFrames)
        {
            _hubContext.Clients.Group(_core.CoreId.ToString()).UpdateMetrics(fps, skippedFrames);
        }
    }
}