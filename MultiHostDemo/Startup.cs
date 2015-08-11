using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MultiHostDemo.Startup))]
namespace MultiHostDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
