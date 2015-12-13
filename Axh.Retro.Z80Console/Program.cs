namespace Axh.Retro.Z80Console
{
    using System.Threading;

    using Axh.Retro.CPU.Z80.Binding;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.Z80Console.Infrastructure;

    using Ninject;

    /// <summary>
    /// Z80Console
    /// Simple implementation of Axh.Retro.CPU.Z80.
    /// Only has Console peripherals and a single bank of 64k RAM.
    /// Execution starts at 0x0000.
    /// See z80-hello for how to compile C hat will work on this emulator.
    /// </summary>
    class Program
    {
        private const string ScopeName = "Z80-Console";

        static void Main(string[] args)
        {
            using (var kernel = new StandardKernel(new Z80ConsoleModule(ScopeName), new Z80Module<IZ80Registers, Z80RegisterState>(ScopeName)))
            {
                var core = kernel.Get<ICpuCore<IZ80Registers, Z80RegisterState>>();

                core.StartCoreProcessAsync(CancellationToken.None).Wait();
            }
        }
    }
}
