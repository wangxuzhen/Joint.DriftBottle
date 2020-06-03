using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Joint.Web.Startup))]
namespace Joint.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
