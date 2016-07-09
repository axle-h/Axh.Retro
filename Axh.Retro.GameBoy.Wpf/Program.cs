using System;
using System.Threading;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.Tests.Util;
using Axh.Retro.GameBoy.Wiring;
using Axh.Retro.GameBoy.Wpf.Config;

namespace Axh.Retro.GameBoy.Wpf
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            using (
                var z80 =
                    new Z80<IRegisters>().With<GameBoyHardware>()
                                         .With(new GameBoyWpf(cancellationTokenSource, Resources.Tetris_W_Gb_Zip.UnZip()))
                                         .Init())
            {
                using (var core = z80.GetNewCore())
                {
                    core.StartCoreProcessAsync(cancellationTokenSource.Token).Wait();
                }
            }
        }
    }
}