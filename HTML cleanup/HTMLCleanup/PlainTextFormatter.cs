namespace HtmlCleanup
{
    class PlainTextFormatter : ITagFormatter
    {
        public PlainTextFormatter()
        {
        }

        public string Process(BaseHtmlCleaner.Tag tag, string innerText)
        {
            switch (tag.StartTag)
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

        public string GetResultingFileExtension()
        {
            return "txt";
        }
    }
}
