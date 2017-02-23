using System.Web.Http;
using DryIoc;
using DryIoc.WebApi;
using DryIoc.WebApi.Owin;
using Owin;

namespace Axh.Retro.GameBoy.Web
{
    public partial class Startup
    {
        private static void ConfigureWebApi(IAppBuilder app, IContainer container)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(name: "DefaultApi",
                                       routeTemplate: "api/{controller}/{id}",
                                       defaults: new { id = RouteParameter.Optional });

            container.WithWebApi(config);

            app.UseWebApi(config);
        }
    }
}