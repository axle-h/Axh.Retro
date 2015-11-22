namespace Axh.Retro.Z80Console
{
    using Axh.Retro.CPU.Z80.Binding;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.GameBoy.Binding;
    using Axh.Retro.GameBoy.Console.Config;

    using Ninject;

    class Program
    {
        private const string ScopeName = "GameBoy-Console";

        static void Main(string[] args)
        {
            using (var kernel = new StandardKernel(new GameBoyConsoleModule(ScopeName), new GameBoyModule(ScopeName), new Z80Module<IIntel8080Registers, Intel8080RegisterState>(ScopeName)))
            {
                var core = kernel.Get<ICpuCore<IIntel8080Registers, Intel8080RegisterState>>();
                
                core.StartCoreProcessAsync().Wait();
            }
        }
    }
}
