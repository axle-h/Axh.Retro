using Axh.Retro.CPU.Z80.Contracts.Config;

namespace Axh.Retro.GameBoy.Config
{
    public class GameBoyRuntimeConfig : IRuntimeConfig
    {
        public bool DebugMode => true;

        public CoreMode CoreMode => CoreMode.DynaRec;
    }
}