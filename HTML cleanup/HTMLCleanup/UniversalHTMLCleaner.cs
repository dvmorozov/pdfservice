using System.IO;

namespace HTMLCleanup
{
    class UniversalHTMLCleaner : BaseHTMLCleaner
    {
        protected override string GetConfigurationFileName()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "UniversalHTMLCleanerConfig.xml";
        }
    }
}
