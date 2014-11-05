using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Caspian.Startup))]
namespace Caspian
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
