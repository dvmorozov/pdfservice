namespace HtmlCleanup
{
    public interface ITagFormatter
    {
        string Process(BaseHtmlCleaner.Tag tag, string innerText);
    }
}
