using System.IO;

namespace HtmlCleanup
{
    public interface ITagFormatter
    {
        //  Initialize state machine of tag formatting.
        string InitializeTagFormatting(BaseHtmlCleaner.Tag tag, string innerText, out bool callFinalize);
        //  Finalize state machine of tag formatting.
        void FinalizeTagFormatting();
        string GetResultingFileExtension();
        MemoryStream GetOutputStream();
        void CloseDocument();
    }
}
