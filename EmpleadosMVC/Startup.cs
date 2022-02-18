using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmpleadosMVC.Startup))]
namespace EmpleadosMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
