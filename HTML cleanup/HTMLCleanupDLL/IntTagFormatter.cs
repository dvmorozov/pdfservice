using System.IO;

namespace HtmlCleanup
{
    public interface ITagFormatter
    {
        //  Initialize state machine of tag formatting.
        string InitializeTagFormatting(BaseHtmlCleaner.HtmlElement htmlElement, string initialText, out bool callFinalize);
        //  Finalize state machine of tag formatting.
        void FinalizeTagFormatting(string finalText);
        string GetResultingFileExtension();
        MemoryStream GetOutputStream();
        void CloseDocument();
    }
}
