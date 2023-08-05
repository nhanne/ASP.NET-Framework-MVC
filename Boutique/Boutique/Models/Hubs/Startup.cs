using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(Boutique.Models.Hubs.Startup))]

namespace Boutique.Models.Hubs
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}