using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EnterpriseServices.Startup))]
namespace EnterpriseServices
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
