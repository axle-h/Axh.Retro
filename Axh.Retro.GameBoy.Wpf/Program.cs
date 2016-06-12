using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.Wiring;

namespace Axh.Retro.GameBoy.Wpf
{
    using System;
    using System.Threading;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.GameBoy.Tests.Util;
    using Axh.Retro.GameBoy.Wpf.Config;
    using Ninject;

    public class Program
    {
        private const string ScopeName = "GameBoy-Wpf";

        [STAThread]
        static void Main(string[] args)
        {
            using (var kernel = new StandardKernel(new GameBoyWpfModule(ScopeName, Resources.Tetris_W_Gb_Zip.UnZip()), new GameBoyModule(ScopeName), new Z80Module<IIntel8080Registers, Intel8080RegisterState>(ScopeName)))
            {
                var core = kernel.Get<ICpuCore<IIntel8080Registers, Intel8080RegisterState>>();
                
                core.StartCoreProcessAsync(CancellationToken.None).Wait();
            }
        }
    }
}
