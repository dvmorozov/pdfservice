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
using System.Collections.Generic;
using System.IO;

namespace HtmlCleanup
{
    public class UniversalHtmlCleaner : BaseHtmlCleaner
    {
        public UniversalHtmlCleaner(ICleanerConfigSerializer configSerializer) : base(configSerializer) { }

        protected override TagRemover GetTagRemover(TextProcessor next, ITagFormatter formatter)
        {
            TagRemover result = new TagRemover(next, formatter)
            {
                Tags = new List<HtmlTag>(new HtmlTag[] {
                    new HtmlTag( "<script", "</script>" ),
                    new HtmlTag( "<style", "</style>" ),
                    new HtmlTag( "<link", "" ),
                    new HtmlTag( "<path", "</path>" ),
                    new HtmlTag( "<meta", "" ),
                    new HtmlTag( "<iframe", "</iframe>" ),
                    new HtmlTag( "<svg", "</svg>" ),
                    new HtmlTag( "<sup", "</sup>" ),
                    new HtmlTag( "<input", "" ),
                    new HtmlTag( "<label", "</label>" ),
                    new HtmlTag( "<form", "</form>" ),
                    new HtmlTag( "<noscript", "</noscript>" ),
                    new HtmlTag( "<nav", "</nav>" ),
                    new HtmlTag( "<!DOCTYPE", "" ),
                    new HtmlTag( "<button", "</button>" ),
                    new HtmlTag( "<aside", "</aside>" ),
                    new HtmlTag( "<!--[if", "<![endif]-->" ),
                    new HtmlTag( "<!--", "" )
                })
            };
            return result;
        }

        protected override InnerTextProcessor GetInnerTextProcessor(TextProcessor next, ITagFormatter formatter)
        {
            InnerTextProcessor result = new InnerTextProcessor(next, formatter)
            {
                Tags = new List<HtmlTag>(new HtmlTag[] {
                    new HtmlTag( "<ul", "</ul>" ),
                    new HtmlTag( "<title", "</title>" ),
                    new HtmlTag( "<strong", "</strong>" ),
                    new HtmlTag( "<span", "</span>" ),
                    new HtmlTag( "<small", "</small>" ),
                    new HtmlTag( "<pre", "</pre>" ),
                    new HtmlTag( "<p", "</p>" ),
                    new HtmlTag( "<main", "</main>" ),
                    new HtmlTag( "<li", "</li>" ),
                    new HtmlTag( "<html", "</html>" ),
                    new HtmlTag( "<header", "</header>" ),
                    new HtmlTag( "<head", "</head>" ),
                    new HtmlTag( "<h4", "</h4>" ),
                    new HtmlTag( "<h3", "</h3>" ),
                    new HtmlTag( "<h3", "</h3>" ),
                    new HtmlTag( "<h2", "</h2>" ),
                    new HtmlTag( "<h1", "</h1>" ),
                    new HtmlTag( "<footer", "</footer>" ),
                    new HtmlTag( "<em", "</em>" ),
                    new HtmlTag( "<div", "</div>" ),
                    new HtmlTag( "<code", "</code>" ),
                    new HtmlTag( "<body", "</body>" ),
                    new HtmlTag( "<article", "</article>" )
                })
            };
            return result;
        }

        protected override ParagraphExtractor GetParagraphExtractor(TextProcessor next, ITagFormatter formatter)
        {
            return new ParagraphExtractor(next, formatter);
        }

        protected override string GetConfigurationFileName()
        {
            return _configSerializer.GetConfigurationFilePath() + "\\" + "UniversalHTMLCleanerConfig.xml";
        }
    }
}
