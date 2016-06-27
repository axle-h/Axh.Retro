namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    public interface IRuntimeConfig
    {
        bool DebugMode { get; }

        CoreMode CoreMode { get; }
    }
}