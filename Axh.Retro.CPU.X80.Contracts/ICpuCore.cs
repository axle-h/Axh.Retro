namespace Axh.Retro.CPU.X80.Contracts
{
    using System.Threading.Tasks;

    public interface ICpuCore
    {
        Task StartCoreProcessAsync();

        void StartCoreProcess();
    }
}