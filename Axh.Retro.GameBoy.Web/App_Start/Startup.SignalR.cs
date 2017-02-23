using System.Reflection;
using System.Web.Routing;
using DryIoc;
using DryIoc.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Owin;

namespace Axh.Retro.GameBoy.Web
{
    public partial class Startup
    {
        private static void ConfigureSignalR(IAppBuilder app, IContainer container)
        {
            container.RegisterHubs(Assembly.GetExecutingAssembly());
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => new DryIocHubActivator(container));
            
#if DEBUG
            const bool DebugEnabled = true;
#else
            const bool DebugEnabled = false;
#endif

            var signalRConfig = new HubConfiguration { EnableDetailedErrors = DebugEnabled, EnableJavaScriptProxies = false };
            app.MapSignalR(signalRConfig);
        }
    }
}