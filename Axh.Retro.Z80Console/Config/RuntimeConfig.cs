namespace Axh.Retro.Z80Console.Config
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Config;

    public class RuntimeConfig : IRuntimeConfig
    {
        public TimeSpan? InstructionCacheSlidingExpiration => TimeSpan.FromSeconds(3);
    }
}
