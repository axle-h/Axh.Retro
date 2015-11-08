namespace Axh.Retro.GameBoy.Config
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Config;

    public class GameBoyRuntimeConfig : IRuntimeConfig
    {
        public TimeSpan? InstructionCacheSlidingExpiration => TimeSpan.FromSeconds(30);

        public bool DebugMode => true;

        public CoreMode CoreMode => CoreMode.DynaRec;
    }
}
