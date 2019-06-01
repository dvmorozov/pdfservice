namespace HtmlCleanup
{
    public interface IHtmlCleaner
    {
        string Process(string html);
        void SetFormatter(ITagFormatter formatter);
        ITagFormatter GetFormatter();
    }
}
