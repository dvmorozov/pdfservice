using System.IO;

namespace HTMLCleanup
{
    class WordPressHTMLCleaner : BaseHTMLCleaner
    {
        protected override string GetConfigurationFileName()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "WordPressHTMLCleanerConfig.xml";
        }
    }
}
