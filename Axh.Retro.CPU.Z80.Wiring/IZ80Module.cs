using DryIoc;

namespace Axh.Retro.CPU.Z80.Wiring
{
    /// <summary>
    /// Registrations for Z80 hardware.
    /// </summary>
    public interface IZ80Module
    {
        /// <summary>
        /// Registers all hardware in this module.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="scope">The reuse scope.</param>
        void Register(IContainer container, IReuse scope);
    }
}
