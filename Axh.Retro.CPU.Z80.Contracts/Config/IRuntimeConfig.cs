namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    using System;

    public interface IRuntimeConfig
    {
        bool DebugMode { get; }

        CoreMode CoreMode { get; }
    }
}
