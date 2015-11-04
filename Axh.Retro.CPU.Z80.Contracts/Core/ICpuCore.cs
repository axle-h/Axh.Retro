namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    using System.Threading.Tasks;

    public interface ICpuCore
    {
        Task StartCoreProcessAsync();
    }
}