using System.IO;

namespace HtmlCleanup
{
    public class ConsoleCleanerConfigSerializer : CleanerConfigSerializer
    {
        public override string GetConfigurationFilePath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
    }
}
