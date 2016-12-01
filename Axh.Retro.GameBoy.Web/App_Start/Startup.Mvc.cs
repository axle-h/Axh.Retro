using System.Web.Mvc;
using System.Web.Routing;

namespace Axh.Retro.GameBoy.Web
{
    public partial class Startup
    {
        private static void ConfigureMvc()
        {
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            RouteTable.Routes.MapRoute(name: "Default",
                                       url: "{controller}/{action}/{id}",
                                       defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
