namespace HtmlCleanup
{
    class PlainTextFormatter : ITagFormatter
    {
        public PlainTextFormatter()
        {
        }

        public string Process(BaseHtmlCleaner.Tag tag, string innerText)
        {
            return innerText;
        }
    }
}
