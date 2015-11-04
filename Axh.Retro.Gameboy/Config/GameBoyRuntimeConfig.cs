namespace Axh.Retro.GameBoy.Config
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Config;

    public class GameBoyRuntimeConfig : IRuntimeConfig
    {
        public TimeSpan? InstructionCacheSlidingExpiration => TimeSpan.FromSeconds(30);
    }
}
