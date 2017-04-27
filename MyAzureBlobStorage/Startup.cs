using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyAzureBlobStorage.Startup))]
namespace MyAzureBlobStorage
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
