/*
 “Commons Clause” License Condition v1.0
The Software is provided to you by the Licensor under the License, as defined below, subject to the following condition.
Without limiting other conditions in the License, the grant of rights under the License will not include, and the License
does not grant to you, right to Sell the Software. For purposes of the foregoing, “Sell” means practicing any or all of
the rights granted to you under the License to provide to third parties, for a fee or other consideration (including
without limitation fees for hosting or consulting/ support services related to the Software), a product or service whose
value derives, entirely or substantially, from the functionality of the Software.  Any license notice or attribution
required by the License must also include this Commons Cause License Condition notice.

Software: HTMLCleanupDLL

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
using System;

namespace HtmlCleanup
{
    public class HtmlCleanerInjector
    {
        private readonly IInjectorConfig _config;
        private readonly ICleanerConfigSerializer _configSerializer;

        public HtmlCleanerInjector(IInjectorConfig config, ICleanerConfigSerializer configSerializer)
        {
            _config = config;
            _configSerializer = configSerializer;
        }

        public IHtmlCleaner CreateHtmlCleaner(string url)
        {
            System.Collections.Generic.List<HtmlCleanerConfigItem> list = _config.GetCleanerList();
            Type formatterType = Type.GetType(_config.GetFormatterType());

            foreach (HtmlCleanerConfigItem item in list)
            {
                if (url.Contains(item.urlPrefix))
                {
                    Type cleanerType = Type.GetType(item.htmlCleanerType);
                    ITagFormatter formatter = Activator.CreateInstance(formatterType) as ITagFormatter;
                    IHtmlCleaner cleaner = Activator.CreateInstance(cleanerType, new object[] { _configSerializer }) as IHtmlCleaner;
                    cleaner.SetFormatter(formatter);
                    return cleaner;
                }
            }
            //  Default HTML parser.
            return new UniversalHtmlCleaner(_configSerializer);
        }
    }
}
