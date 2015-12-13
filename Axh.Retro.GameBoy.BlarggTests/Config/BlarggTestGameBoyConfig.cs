namespace Axh.Retro.GameBoy.BlarggTests.Config
{
    using Axh.Retro.GameBoy.Contracts;
    using Axh.Retro.GameBoy.Contracts.Config;

    public class BlarggTestGameBoyConfig : IGameBoyConfig
    {
        public byte[] CartridgeData { get; set; }

        public GameBoyType GameBoyType => GameBoyType.GameBoy;

        public bool RunGpu => false;

        public bool UseGameBoyTimings => false;
    }
}
