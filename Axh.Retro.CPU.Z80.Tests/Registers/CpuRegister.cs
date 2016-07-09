using System.Collections.Generic;
using System.Linq;

namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    public enum CpuRegister
    {
        A, B, C, D, E, H, L
    }

    public static class RegisterHelpers
    {
        public static IEnumerable<CpuRegister> Registers8Bit => new[] { CpuRegister.A, CpuRegister.B, CpuRegister.C, CpuRegister.D, CpuRegister.E, CpuRegister.H, CpuRegister.L };

        public static IEnumerable<CpuRegister> Other8BitRegisters(this CpuRegister cpuRegister) => Registers8Bit.Except(new [] { cpuRegister });
    }
}