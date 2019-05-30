using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;

namespace HtmlCleanup
{
    class PdfFormatter : ITagFormatter
    {
        private MemoryStream _content;
        private Document _document;
        private PdfFont _font;
        private List _list;
        private bool _listItem;
        private bool _preItem;
        private bool _paragraph;
        private PdfWriter _writer;

        public MemoryStream GetOutputStream()
        {
            return _content;
        }

        public void CloseDocument()
        {
            _document.Close();
        }

        public PdfFormatter()
        {
            _content = new MemoryStream();

            _writer = new PdfWriter(_content);
            // Allows subsequent access to content.
            _writer.SetCloseStream(false);

            var pdf = new PdfDocument(_writer);
            _document = new Document(pdf);

            // Creates font object.
            _font = PdfFontFactory.CreateFont(fontProgram: FontConstants.TIMES_ROMAN);
        }

        public string InitializeTagFormatting(BaseHtmlCleaner.Tag tag, string innerText, out bool callFinalize)
        {
            callFinalize = false;
            switch (tag.StartTag)
            {
                case ("<ul"):
                    //  Creates list object.
                    _list = new List()
                        .SetSymbolIndent(12)
                        .SetListSymbol("*")
                        .SetFont(_font);
                    callFinalize = true;
                    break;

                case ("<li"):
                    //  Creates list item.
                    if (_list != null) {
                        _listItem = true;
                        callFinalize = true;
                    }
                    break;

                case ("<p"):
                    _paragraph = true;
                    callFinalize = true;
                    break;

                case ("<pre"):
                    _preItem = true;
                    callFinalize = true;
                    break;
            }
            //  Text is always returned as is for subsequent parsing.
            //  This type of formatter doesn't touch it!
            return innerText;
        }

        public void FinalizeTagFormatting(string finalText)
        {
            if (_list != null)
            {   //  Finalizes list parsing.
                if (_listItem)
                {
                    //  Adds new list item.
                    _list.Add(new ListItem(finalText));
                    _listItem = false;
                }
                else
                {
                    //  Finalizes the list.
                    _document.Add(_list);
                    _list = null;
                }
            }
            if (_preItem)
            {
                var paragraph = new Paragraph();
                paragraph.Add(finalText);
                _document.Add(paragraph);
                _preItem = false;
            }
            if (_paragraph)
            {
                var paragraph = new Paragraph();
                paragraph.Add(finalText);
                _document.Add(paragraph);
                _paragraph = false;
            }
        }

        public string GetResultingFileExtension()
        {
            return "pdf";
        }
    }
}
