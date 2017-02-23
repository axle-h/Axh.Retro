using System;
using System.IO;
using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.Web.Wiring;
using Axh.Retro.GameBoy.Wiring;
using DryIoc;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

[assembly: OwinStartup(typeof(Axh.Retro.GameBoy.Web.Startup))]

namespace Axh.Retro.GameBoy.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new Container();
            
            var gameBoyFactory = new Z80Wiring().With<GameBoyHardware>().With(new GameBoyWebModule(Resources.Tetris_W_Gb_Zip.UnZip())).Init();
            var context = new CpuCoreContext(gameBoyFactory);
            container.RegisterInstance<ICpuCoreContext>(context, Reuse.Singleton);


            var root = AppDomain.CurrentDomain.BaseDirectory;
            var physicalFileSystem = new PhysicalFileSystem(Path.Combine(root, "wwwroot"));
            var options = new FileServerOptions
            {
                RequestPath = PathString.Empty,
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = false;
            app.UseFileServer(options);
            ConfigureWebApi(app, container);
            ConfigureSignalR(app, container);
        }
    }
    
}
