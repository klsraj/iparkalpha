using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(iParkBackend.Startup))]

namespace iParkBackend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}