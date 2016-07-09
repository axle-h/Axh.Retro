using System.Threading;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.Z80Console.Config;

namespace Axh.Retro.Z80Console
{
    /// <summary>
    /// Z80Console
    /// Simple implementation of Axh.Retro.CPU.Z80.
    /// Only has Console peripherals and a single bank of 64k RAM.
    /// Execution starts at 0x0000.
    /// See z80-hello for how to compile C hat will work on this emulator.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var z80 = new Z80<IZ80Registers>().With<Z80ConsoleModule>().Init())
            {
                using (var core = z80.GetNewCore())
                {
                    core.StartCoreProcessAsync(CancellationToken.None).Wait();
                }
            }
        }
    }
}