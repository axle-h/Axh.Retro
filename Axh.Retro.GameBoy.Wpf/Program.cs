using System;
using System.Threading;
using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.Wiring;
using Axh.Retro.GameBoy.Wpf.Config;

namespace Axh.Retro.GameBoy.Wpf
{
    public class Program
    {
        [STAThread]
        private static void Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            using (var gameBoy = new Z80Wiring().With<GameBoyHardware>()
                                                .With(new GameBoyWpf(cancellationTokenSource, Resources.Tetris_W_Gb_Zip.UnZip()))
                                                .Init())
            using (var core = gameBoy.GetNewCore())
            {
                core.StartCoreProcessAsync(cancellationTokenSource.Token).Wait();
            }
        }
    }
}