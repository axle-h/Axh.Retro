namespace Axh.Retro.GameBoy.Console.Config
{
    using Axh.Retro.GameBoy.Contracts;
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Tests.Util;

    public class GameBoyConfig : IGameBoyConfig
    {
        public byte[] CartridgeData => Resources.cpu_instrs;

        public GameBoyType GameBoyType => GameBoyType.GameBoy;
    }
}
