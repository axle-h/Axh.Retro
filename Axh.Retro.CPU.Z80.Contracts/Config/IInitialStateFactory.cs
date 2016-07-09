using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    /// <summary>
    /// A factory for creating the initial state of the CPU.
    /// </summary>
    public interface IInitialStateFactory
    {
        /// <summary>
        /// Gets the initial state of the CPU registers.
        /// </summary>
        /// <returns></returns>
        Z80RegisterState GetInitialRegisterState();
    }
}