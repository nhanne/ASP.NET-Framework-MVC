using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using System.Configuration;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using Microsoft.AspNet.Identity;

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