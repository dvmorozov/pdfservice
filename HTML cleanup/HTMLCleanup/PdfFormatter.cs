using iText.Kernel.Pdf;
using iText.Layout;
using System.IO;

namespace HtmlCleanup
{
    class PdfFormatter : ITagFormatter
    {
        private MemoryStream _content;
        private Document _document;

        public PdfFormatter()
        {
            _content = new MemoryStream();
            var writer = new PdfWriter(_content);
            var pdf = new PdfDocument(writer);
            _document = new Document(pdf);
        }

        public string Process(BaseHtmlCleaner.Tag tag, string innerText)
        {
            switch (tag.StartTag)
            {
                case ("<ul"):
                    return innerText;

                case ("<li"):
                    return innerText;

                case ("<pre"):
                    return innerText;
            }
            return innerText;
        }

        public string GetResultingFileExtension()
        {
            return "pdf";
        }
    }
}
