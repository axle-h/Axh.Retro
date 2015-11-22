namespace Axh.Retro.GameBoy.Console.Config
{
    using Axh.Retro.GameBoy.Console.Properties;
    using Axh.Retro.GameBoy.Contracts;
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Tests.Util;

    public class GameBoyConfig : IGameBoyConfig
    {
        public byte[] CartridgeData => Axh.Retro.GameBoy.Resources.Resources.Tetris_W_Gb_Zip.UnZip();

        public GameBoyType GameBoyType => GameBoyType.GameBoy;
    }
}
