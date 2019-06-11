using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.StyledXmlParser.Resolver.Font;
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
        private bool _paragraph;
        private bool _preformatted;     //  Add "preformatted" style.
        private PdfWriter _writer;
        private float _defaultPadding = 10;
        private float _defaultFontSize = 14;

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
            _document.SetFontProvider(new BasicFontProvider());

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
                        .SetListSymbol("\u2022")
                        .SetFont(_font)
                        .SetFontSize(_defaultFontSize);
                    callFinalize = true;
                    break;

                case ("<li"):
                    //  Creates list item.
                    if (_list != null) {
                        _listItem = true;
                        callFinalize = true;
                    }
                    break;

                case ("<pre"):
                    _preformatted = true;
                    _paragraph = true;
                    callFinalize = true;
                    break;

                case ("<p"):
                case ("<h1"):
                case ("<h2"):
                case ("<h3"):
                case ("<h4"):
                    _paragraph = true;
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
                    _list.SetPadding(_defaultPadding);
                    _document.Add(_list);
                    _list = null;
                }
            }
            if (_paragraph)
            {
                var paragraph = new Paragraph().SetFont(_font);
                paragraph.Add(finalText);
                paragraph.SetFontSize(_defaultFontSize);

                if (_preformatted)
                {
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetFontFamily(new string[] { iText.IO.Font.Constants.StandardFonts.COURIER });
                    paragraph.SetFontSize(10);
                    paragraph.SetPadding(_defaultPadding);
                    paragraph.SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.GRAY, 1));
                    _preformatted = false;
                }

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
