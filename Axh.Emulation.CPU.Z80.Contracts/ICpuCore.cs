namespace Axh.Emulation.CPU.Z80.Contracts
{
    using System.Threading.Tasks;

    public interface ICpuCore
    {
        Task StartCoreProcess();
    }
}