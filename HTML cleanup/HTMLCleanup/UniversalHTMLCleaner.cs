using System.Collections.Generic;
using System.IO;

namespace HtmlCleanup
{
    class UniversalHtmlCleaner : BaseHtmlCleaner
    {
        protected override TagWithTextRemover GetTagWithTextRemover(TextProcessor next)
        {
            var result = new TagWithTextRemover(next)
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

        protected override InnerTagRemover GetInnerTagRemover(TextProcessor next)
        {
            var result = new InnerTagRemover(next)
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

        protected override ParagraphExtractor GetParagraphExtractor(TextProcessor next)
        {
            return new ParagraphExtractor(next);
        }

        protected override string GetConfigurationFileName()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "UniversalHTMLCleanerConfig.xml";
        }
    }
}
