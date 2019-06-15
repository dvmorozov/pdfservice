using System.Collections.Generic;
using System.IO;

namespace HtmlCleanup
{
    class UniversalHtmlCleaner : BaseHtmlCleaner
    {
        public UniversalHtmlCleaner(ICleanerConfigSerializer configSerializer) : base(configSerializer) { }

        protected override TagRemover GetTagRemover(TextProcessor next, ITagFormatter formatter)
        {
            var result = new TagRemover(next, formatter)
            {
                Tags = new List<HtmlTag>(new HtmlTag[] {
                    new HtmlTag( "<script", "</script>" ),
                    new HtmlTag( "<style", "</style>" ),
                    new HtmlTag( "<link", "" ),
                    new HtmlTag( "<path", "</path>" ),
                    new HtmlTag( "<meta", "" ),
                    new HtmlTag( "<iframe", "</iframe>" ),
                    new HtmlTag( "<svg", "</svg>" ),
                    new HtmlTag( "<sup", "</sup>" ),
                    new HtmlTag( "<input", "" ),
                    new HtmlTag( "<label", "</label>" ),
                    new HtmlTag( "<form", "</form>" ),
                    new HtmlTag( "<noscript", "</noscript>" ),
                    new HtmlTag( "<nav", "</nav>" ),
                    new HtmlTag( "<!DOCTYPE", "" ),
                    new HtmlTag( "<button", "</button>" ),
                    new HtmlTag( "<aside", "</aside>" ),
                    new HtmlTag( "<!--[if", "<![endif]-->" ),
                    new HtmlTag( "<!--", "" )
                })
            };
            return result;
        }

        protected override InnerTextProcessor GetInnerTextProcessor(TextProcessor next, ITagFormatter formatter)
        {
            var result = new InnerTextProcessor(next, formatter)
            {
                Tags = new List<HtmlTag>(new HtmlTag[] {
                    new HtmlTag( "<ul", "</ul>" ),
                    new HtmlTag( "<title", "</title>" ),
                    new HtmlTag( "<strong", "</strong>" ),
                    new HtmlTag( "<span", "</span>" ),
                    new HtmlTag( "<small", "</small>" ),
                    new HtmlTag( "<pre", "</pre>" ),
                    new HtmlTag( "<p", "</p>" ),
                    new HtmlTag( "<main", "</main>" ),
                    new HtmlTag( "<li", "</li>" ),
                    new HtmlTag( "<html", "</html>" ),
                    new HtmlTag( "<header", "</header>" ),
                    new HtmlTag( "<head", "</head>" ),
                    new HtmlTag( "<h4", "</h4>" ),
                    new HtmlTag( "<h3", "</h3>" ),
                    new HtmlTag( "<h3", "</h3>" ),
                    new HtmlTag( "<h2", "</h2>" ),
                    new HtmlTag( "<h1", "</h1>" ),
                    new HtmlTag( "<footer", "</footer>" ),
                    new HtmlTag( "<em", "</em>" ),
                    new HtmlTag( "<div", "</div>" ),
                    new HtmlTag( "<code", "</code>" ),
                    new HtmlTag( "<body", "</body>" ),
                    new HtmlTag( "<article", "</article>" )
                })
            };
            return result;
        }

        protected override ParagraphExtractor GetParagraphExtractor(TextProcessor next, ITagFormatter formatter)
        {
            return new ParagraphExtractor(next, formatter);
        }

        protected override string GetConfigurationFileName()
        {
            return _configSerializer.GetConfigurationFilePath() + "\\" + "UniversalHTMLCleanerConfig.xml";
        }
    }
}
