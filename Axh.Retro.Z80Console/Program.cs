namespace Axh.Retro.Z80Console
{
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.Z80Console.Infrastructure;

    using Ninject;

    /// <summary>
    /// Z80Console
    /// Simple Z80 implementation of Axh.Retro.CPU.X80.
    /// Only has Console peripherals and a single bank of 64k RAM.
    /// Execution starts at 0x0000.
    /// See z80-hello for how to compile C hat will work on this emulator.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            using (var kernel = new StandardKernel(new Z80Module()))
            {
                var core = kernel.Get<ICpuCore>();
                core.StartCoreProcessAsync().Wait();
            }
        }
    }
}
