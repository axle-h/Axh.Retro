using Axh.Retro.CPU.Z80.Contracts.Config;

namespace Axh.Retro.Z80Console.Config
{
    public class RuntimeConfig : IRuntimeConfig
    {
        public bool DebugMode => true;

        public CoreMode CoreMode => CoreMode.DynaRec;
    }
}