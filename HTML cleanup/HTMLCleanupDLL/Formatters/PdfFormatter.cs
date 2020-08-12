using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Layout;
using iText.Layout.Element;
using iText.StyledXmlParser.Resolver.Font;
using System.IO;

namespace HtmlCleanup
{
    class PdfFormatter : ITagFormatter
    {
        enum ParagraphType {
            Simple,
            H1,
            H2,
            H3,
            H4,
            Header,
            Hyperlink,
            Time
        }

        private readonly MemoryStream _content;
        private readonly Document _document;
        private readonly PdfFont _font;
        private List _list;
        private bool _listItem;
        private bool _paragraph;
        private bool _hyperlink;        //  Hyperlink is a "state". All nested paragraph should be hyperlinked.
        private bool _preformatted;     //  Add "preformatted" style.
        private readonly PdfWriter _writer;
        private readonly float _defaultPadding = 10;
        private readonly float _defaultFontSize = 14;
        private ParagraphType _paragraphType = ParagraphType.Simple;
        private string _href;

        public object BaseColor { get; private set; }

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

            PdfDocument pdf = new PdfDocument(_writer);
            _document = new Document(pdf);
            _document.SetFontProvider(new BasicFontProvider());

            // Creates font object.
            _font = PdfFontFactory.CreateFont(fontProgram: iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN);
        }

        public string InitializeTagFormatting(BaseHtmlCleaner.HtmlElement htmlElement, string innerText, out bool callFinalize)
        {
            callFinalize = false;
            switch (htmlElement.StartTag)
            {
                case "<ul":
                    //  Creates list object.
                    _list = new List()
                        .SetSymbolIndent(12)
                        .SetListSymbol("\u2022")
                        .SetFont(_font)
                        .SetFontSize(_defaultFontSize);
                    callFinalize = true;
                    break;

                case "<li":
                    //  Creates list item.
                    if (_list != null)
                    {
                        _listItem = true;
                        callFinalize = true;
                    }
                    break;

                case "<pre":
                    _preformatted = true;
                    _paragraph = true;
                    callFinalize = true;
                    break;

                case "<h1":
                    _paragraphType = ParagraphType.H1;
                    _paragraph = true;
                    callFinalize = true;
                    break;

                case "<h2":
                    _paragraphType = ParagraphType.H2;
                    _paragraph = true;
                    callFinalize = true;
                    break;

                case "<h3":
                    _paragraphType = ParagraphType.H3;
                    _paragraph = true;
                    callFinalize = true;
                    break;

                case "<h4":
                    _paragraphType = ParagraphType.H4;
                    _paragraph = true;
                    callFinalize = true;
                    break;

                case "<p":
                    _paragraphType = ParagraphType.Simple;
                    _paragraph = true;
                    callFinalize = true;
                    break;

                case "<a":
                    _href = htmlElement.GetAttribute("href");
                    _paragraphType = ParagraphType.Hyperlink;
                    _paragraph = true;
                    _hyperlink = true;
                    callFinalize = true;
                    break;

                case "<header":
                    _paragraphType = ParagraphType.Header;
                    _paragraph = true;
                    callFinalize = true;
                    break;

                case "<time":
                    _paragraphType = ParagraphType.Time;
                    _paragraph = true;
                    callFinalize = true;
                    break;

                default:
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
                Paragraph paragraph = new Paragraph().SetFont(_font);
                switch (_paragraphType)
                {
                    case ParagraphType.Simple:
                        paragraph.SetFontSize(_defaultFontSize);
                        paragraph.Add(finalText);
                        break;

                    case ParagraphType.H1:
                        paragraph.SetFontSize(_defaultFontSize + 8);
                        paragraph.Add(finalText);
                        break;

                    case ParagraphType.H2:
                        paragraph.SetFontSize(_defaultFontSize + 6);
                        paragraph.Add(finalText);
                        break;

                    case ParagraphType.H3:
                        paragraph.SetFontSize(_defaultFontSize + 4);
                        paragraph.Add(finalText);
                        break;

                    case ParagraphType.Header:
                        paragraph.SetFontSize(_defaultFontSize + 10);
                        paragraph.Add(finalText);
                        break;

                    case ParagraphType.Hyperlink:
                        //  Finalizes "simple" hyperlinked paragraph without nested paragraphs.
                        paragraph.Add(new Link(finalText, PdfAction.CreateURI(_href)));
                        break;

                    case ParagraphType.Time:
                        if (_hyperlink)
                        {   //  Finalizes "nested" hyperlinked paragraph. The only single is possible now.
                            paragraph.Add(new Link(finalText, PdfAction.CreateURI(_href)));
                            _hyperlink = false;
                        }
                        else
                            paragraph.Add(finalText);
                        break;

                    case ParagraphType.H4:
                        break;

                    default:
                        break;
                }

                if (_preformatted)
                {
                    _ = paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    _ = paragraph.SetFontFamily(new string[] { iText.IO.Font.Constants.StandardFonts.COURIER });
                    _ = paragraph.SetFontSize(10);
                    _ = paragraph.SetPadding(_defaultPadding);
                    _ = paragraph.SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.GRAY, 1));
                    _preformatted = false;
                }

                _ = _document.Add(paragraph);
                _paragraph = false;
            }
        }

        public string GetResultingFileExtension()
        {
            return "pdf";
        }
    }
}
