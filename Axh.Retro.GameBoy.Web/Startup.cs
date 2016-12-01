using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Axh.Retro.GameBoy.Web.Startup))]

namespace Axh.Retro.GameBoy.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureWebApi(app);
            ConfigureMvc();
        }
    }
}
