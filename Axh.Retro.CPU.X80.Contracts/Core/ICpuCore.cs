namespace Axh.Retro.CPU.X80.Contracts.Core
{
    using System.Threading.Tasks;

    public interface ICpuCore
    {
        Task StartCoreProcessAsync();

        void StartCoreProcess();
    }
}