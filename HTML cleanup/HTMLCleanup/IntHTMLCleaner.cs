namespace HtmlCleanup
{
    interface IHtmlCleaner
    {
        string Process(string html);
        void SetFormatter(ITagFormatter formatter);
    }
}
