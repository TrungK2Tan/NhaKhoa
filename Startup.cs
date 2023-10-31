using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NhaKhoa.Startup))]
namespace NhaKhoa
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
