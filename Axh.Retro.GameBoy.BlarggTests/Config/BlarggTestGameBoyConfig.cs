using Axh.Retro.GameBoy.Contracts;
using Axh.Retro.GameBoy.Contracts.Config;

namespace Axh.Retro.GameBoy.BlarggTests.Config
{
    public class BlarggTestGameBoyConfig : IGameBoyConfig
    {
        public byte[] CartridgeData { get; set; }

        public GameBoyType GameBoyType => GameBoyType.GameBoy;

        public bool RunGpu => false;
    }
}