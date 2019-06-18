﻿using System.IO;

namespace HtmlCleanup
{
    public class PlainTextFormatter : ITagFormatter
    {
        public string InitializeTagFormatting(BaseHtmlCleaner.HtmlElement htmlElement, string innerText, out bool callFinalize)
        {
            callFinalize = false;
            switch (htmlElement.StartTag)
            {
                case ("<ul"):
                    //  Adds line break (line should not contain only white
                    //  spaces, otherwise it will be removed at next stages).
                    return "\\\n" + innerText + "\\\n";

                case ("<li"):
                    return "  * " + innerText;

                case ("<pre"):
                    var indent = "\\  ";
                    return indent + innerText.Replace("\n", "\n" + indent);
            }
            return innerText;
        }

        public void FinalizeTagFormatting(string finalText)
        {
        }

        public string GetResultingFileExtension()
        {
            return "txt";
        }

        public MemoryStream GetOutputStream()
        {
            return null;
        }

        public void CloseDocument()
        {
        }
    }
}
