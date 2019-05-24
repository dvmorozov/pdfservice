using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlCleanup
{
    class PdfFormatter : ITagFormatter
    {
        public string Process(BaseHtmlCleaner.Tag tag, string innerText)
        {
            switch (tag.StartTag)
            {
                case ("<ul"):
                    return innerText;

                case ("<li"):
                    return innerText;

                case ("<pre"):
                    return innerText;
            }
            return innerText;
        }

        public string GetResultingFileExtension()
        {
            return "pdf";
        }
    }
}
