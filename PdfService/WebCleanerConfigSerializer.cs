using HtmlCleanup;
using System.Web;

namespace EnterpriseServices
{
    public class WebCleanerConfigSerializer : CleanerConfigSerializer
    {
        private HttpServerUtilityBase _server;

        public WebCleanerConfigSerializer(HttpServerUtilityBase server)
        {
            _server = server;
        }

        public override string GetConfigurationFilePath()
        {
            return _server.MapPath("~/App_Data/");
        }
    }
}