namespace Axh.Emulation.CPU.X80.Contracts
{
    using System.Threading.Tasks;

    public interface ICpuCore
    {
        Task StartCoreProcess();
    }
}