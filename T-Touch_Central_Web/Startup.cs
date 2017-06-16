using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(T_Touch_Central_Web.Startup))]
namespace T_Touch_Central_Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
