using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DX.TED.IdentityTestWeb.Startup))]
namespace DX.TED.IdentityTestWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
