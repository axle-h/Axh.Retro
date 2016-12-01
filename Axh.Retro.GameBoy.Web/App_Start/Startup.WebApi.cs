using System.Web.Http;
using Owin;

namespace Axh.Retro.GameBoy.Web
{
    public partial class Startup
    {
        private static void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(name: "DefaultApi",
                                       routeTemplate: "api/{controller}/{id}",
                                       defaults: new { id = RouteParameter.Optional });
            app.UseWebApi(config);
        }
    }
}