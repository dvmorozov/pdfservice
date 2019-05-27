using System.IO;

namespace HtmlCleanup
{
    public interface ITagFormatter
    {
        string Process(BaseHtmlCleaner.Tag tag, string innerText);
        string GetResultingFileExtension();
        MemoryStream GetOutputStream();
        void CloseDocument();
    }
}
