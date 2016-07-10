using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Wiring
{
    /// <summary>
    /// Wiring for a Z80 platform.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Wiring.Intel8080WiringBase{Axh.Retro.CPU.Z80.Contracts.Registers.IZ80Registers}" />
    public class Z80Wiring : Intel8080WiringBase<IZ80Registers>
    {
    }
}