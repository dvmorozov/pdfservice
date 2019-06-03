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
                Tags = new List<Tag>(new Tag[] {
                    new Tag( "<script", "</script>" ),
                    new Tag( "<style", "</style>" ),
                    new Tag( "<link", "" ),
                    new Tag( "<path", "</path>" ),
                    new Tag( "<meta", "" ),
                    new Tag( "<iframe", "</iframe>" ),
                    new Tag( "<svg", "</svg>" ),
                    new Tag( "<sup", "</sup>" ),
                    new Tag( "<input", "" ),
                    new Tag( "<label", "</label>" ),
                    new Tag( "<form", "</form>" ),
                    new Tag( "<noscript", "</noscript>" ),
                    new Tag( "<nav", "</nav>" ),
                    new Tag( "<!DOCTYPE", "" ),
                    new Tag( "<button", "</button>" ),
                    new Tag( "<aside", "</aside>" ),
                    new Tag( "<!--[if", "<![endif]-->" ),
                    new Tag( "<!--", "" )
                })
            };
            return result;
        }

        protected override InnerTextProcessor GetInnerTextProcessor(TextProcessor next, ITagFormatter formatter)
        {
            var result = new InnerTextProcessor(next, formatter)
            {
                Tags = new List<Tag>(new Tag[] {
                    new Tag( "<ul", "</ul>" ),
                    new Tag( "<title", "</title>" ),
                    new Tag( "<strong", "</strong>" ),
                    new Tag( "<span", "</span>" ),
                    new Tag( "<small", "</small>" ),
                    new Tag( "<pre", "</pre>" ),
                    new Tag( "<p", "</p>" ),
                    new Tag( "<main", "</main>" ),
                    new Tag( "<li", "</li>" ),
                    new Tag( "<html", "</html>" ),
                    new Tag( "<header", "</header>" ),
                    new Tag( "<head", "</head>" ),
                    new Tag( "<h4", "</h4>" ),
                    new Tag( "<h3", "</h3>" ),
                    new Tag( "<h3", "</h3>" ),
                    new Tag( "<h2", "</h2>" ),
                    new Tag( "<h1", "</h1>" ),
                    new Tag( "<footer", "</footer>" ),
                    new Tag( "<em", "</em>" ),
                    new Tag( "<div", "</div>" ),
                    new Tag( "<code", "</code>" ),
                    new Tag( "<body", "</body>" ),
                    new Tag( "<article", "</article>" )
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
