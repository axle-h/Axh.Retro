using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Axh.Retro.GameBoy.Web.Controllers.Hubs
{
    public class GameBoyHub : Hub<IGameBoyHub>
    {
        public Task JoinGroup(Guid id) => Groups.Add(Context.ConnectionId, id.ToString());

        public Task LeaveGroup(Guid id) => Groups.Remove(Context.ConnectionId, id.ToString());
    }

    public interface IGameBoyHub
    {
        void Render(byte[] frame);

        void UpdateMetrics(int fps, int skippedFrames);
    }
}