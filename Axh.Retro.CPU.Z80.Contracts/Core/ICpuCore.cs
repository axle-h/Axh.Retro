namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    using System.Threading.Tasks;

    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public interface ICpuCore<TRegisters> where TRegisters : IRegisters
    {
        Task StartCoreProcessAsync(ICoreContext<TRegisters> context);

        ICoreContext<TRegisters> GetContext();
    }
}