namespace HtmlCleanup
{
    public interface IHtmlCleaner
    {
        string Process(string html);
        string GetResultingFileData();
        void SetFormatter(ITagFormatter formatter);
        ITagFormatter GetFormatter();
    }
}
