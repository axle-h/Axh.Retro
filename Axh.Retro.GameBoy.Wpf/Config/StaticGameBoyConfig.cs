namespace Axh.Retro.GameBoy.Console.Config
{
    using Axh.Retro.GameBoy.Contracts;
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Tests.Util;

    public class StaticGameBoyConfig : IGameBoyConfig
    {
        public StaticGameBoyConfig(byte[] cartridge)
        {
            this.CartridgeData = cartridge;
        }

        public byte[] CartridgeData { get; }

        public GameBoyType GameBoyType => GameBoyType.GameBoy;

        public bool RunGpu => true;

        public bool UseGameBoyTimings => false;
    }
}
