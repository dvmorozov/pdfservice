using System.Linq;
using System.IO;
using HtmlCleanup;

namespace HtmlCleanupApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 0)
            {
                var url = args[0];

                var injector = new HtmlCleanerInjector(new BaseInjectorConfig());
                //  Creating cleaner instance based on URL.
                var processChain = injector.CreateHtmlCleaner(url);

                //  Performs request.
                var s = HtmlCleanerApp.MakeRequest(url);

                var output = processChain.Process(s);

                //  Creates directories for storing page content.
                var path = HtmlCleanerApp.CreateDirectories(url);

                var formatter = processChain.GetFormatter();

                //  Forms content file name.
                var fileName = path + "\\" + "content." + formatter.GetResultingFileExtension();

                //  Finishes processing.
                formatter.CloseDocument();
                var dataStream = formatter.GetOutputStream();

                if (dataStream != null)
                {
                    using (var fileStream = File.Create(fileName))
                    {
                        dataStream.Seek(0, SeekOrigin.Begin);
                        dataStream.CopyTo(fileStream);
                    }
                }
                else
                {
                    //  Saves text to file.
                    HtmlCleanerApp.WriteTextToFile(fileName, output);
                }
            }
            else
            {
                //  Default HTML cleaner for writing configutaion.
                var processChain = new WordPressHtmlCleaner();
                //  Writes template of configuration file.
                processChain.WriteConfiguration();
            }
        }
    }
}
