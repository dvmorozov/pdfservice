using System.Collections.Generic;
using System.IO;

namespace HTMLCleanup
{
    class WordPressHTMLCleaner : BaseHTMLCleaner
    {
        protected override TagWithTextRemover GetTagWithTextRemover(TextProcessor next)
        {
            var result = new TagWithTextRemover(next);
            result.Tags = new List<TagToRemove>(new TagToRemove[] {
                new TagToRemove( "<script", "</script>" ),
                new TagToRemove( "<style", "</style>" ),
                new TagToRemove( "<link", "" ),
                new TagToRemove( "<path", "</path>" ),
                new TagToRemove( "<meta", "" ),
                new TagToRemove( "<iframe", "</iframe>" ),
                new TagToRemove( "<svg", "</svg>" ),
                new TagToRemove( "<sup", "</sup>" ),
                new TagToRemove( "<input", "" ),
                new TagToRemove( "<label", "</label>" ),
                new TagToRemove( "<form", "</form>" ),
                new TagToRemove( "<noscript", "</noscript>" ),
                new TagToRemove( "<nav", "</nav>" ),
                new TagToRemove( "<!DOCTYPE", "" ),
                new TagToRemove( "<button", "</button>" ),
                new TagToRemove( "<aside", "</aside>" ),
                new TagToRemove( "<!--[if", "<![endif]-->" ),
                new TagToRemove( "<!--", "" )
            });
            return result;
        }

        protected override InnerTagRemover GetInnerTagRemover(TextProcessor next)
        {
            var result = new InnerTagRemover(next);
            result.Tags = new List<TagToRemove>(new TagToRemove[] {
                new TagToRemove( "<ul", "</ul>" ),
                new TagToRemove( "<title", "</title>" ),
                new TagToRemove( "<strong", "</strong>" ),
                new TagToRemove( "<span", "</span>" ),
                new TagToRemove( "<small", "</small>" ),
                new TagToRemove( "<pre", "</pre>" ),
                new TagToRemove( "<p", "</p>" ),
                new TagToRemove( "<main", "</main>" ),
                new TagToRemove( "<li", "</li>" ),
                new TagToRemove( "<html", "</html>" ),
                new TagToRemove( "<header", "</header>" ),
                new TagToRemove( "<head", "</head>" ),
                new TagToRemove( "<h3", "</h3>" ),
                new TagToRemove( "<h3", "</h3>" ),
                new TagToRemove( "<h2", "</h2>" ),
                new TagToRemove( "<h1", "</h1>" ),
                new TagToRemove( "<footer", "</footer>" ),
                new TagToRemove( "<em", "</em>" ),
                new TagToRemove( "<div", "</div>" ),
                new TagToRemove( "<code", "</code>" ),
                new TagToRemove( "<body", "</body>" ),
                new TagToRemove( "<article", "</article>" )
            });
            return result;
        }

        protected override string GetConfigurationFileName()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "WordPressHTMLCleanerConfig.xml";
        }
    }
}
