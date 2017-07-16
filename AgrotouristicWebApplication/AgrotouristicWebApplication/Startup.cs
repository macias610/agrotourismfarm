using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AgrotouristicWebApplication.Startup))]
namespace AgrotouristicWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
