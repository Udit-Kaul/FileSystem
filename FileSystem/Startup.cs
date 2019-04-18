using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FileSystem.Startup))]
namespace FileSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
