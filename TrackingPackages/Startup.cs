using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrackingPackages.Startup))]
namespace TrackingPackages
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
