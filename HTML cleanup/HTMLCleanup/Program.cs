/*
 “Commons Clause” License Condition v1.0
The Software is provided to you by the Licensor under the License, as defined below, subject to the following condition.
Without limiting other conditions in the License, the grant of rights under the License will not include, and the License
does not grant to you, right to Sell the Software. For purposes of the foregoing, “Sell” means practicing any or all of
the rights granted to you under the License to provide to third parties, for a fee or other consideration (including
without limitation fees for hosting or consulting/ support services related to the Software), a product or service whose
value derives, entirely or substantially, from the functionality of the Software.  Any license notice or attribution
required by the License must also include this Commons Cause License Condition notice.

Software: HTMLCleanup

License: 
Copyright 2020 Dmitry Morozov

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Licensor: Dmitry Morozov
 */
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

                var injector = new HtmlCleanerInjector(new BaseInjectorConfig(), new ConsoleCleanerConfigSerializer());
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
                var processChain = new WordPressHtmlCleaner(new ConsoleCleanerConfigSerializer());
                //  Writes template of configuration file.
                processChain.WriteConfiguration();
            }
        }
    }
}
