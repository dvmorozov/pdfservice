using System.Collections.Generic;
using System.IO;

namespace HtmlCleanup
{
    class WordPressHtmlCleaner : BaseHtmlCleaner
    {
        protected override TagWithTextRemover GetTagWithTextRemover(TextProcessor next)
        {
            var result = new TagWithTextRemover(next)
            {
                Tags = new List<TagToRemove>(new TagToRemove[] {
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
                    new TagToRemove( "<footer", "</footer>" ),
                    new TagToRemove( "<form", "</form>" ),
                    new TagToRemove( "<noscript", "</noscript>" ),
                    new TagToRemove( "<nav", "</nav>" ),
                    new TagToRemove( "<!DOCTYPE", "" ),
                    //  Advertising block and internal divs.
                    //  Items should be in the order reverse
                    //  to the nesting of divs (best possible
                    //  option for this primitive parser).
                    new TagToRemove( "<div id=\"atatags", "</div>"),
                    new TagToRemove( "<div style=\"", "</div>"),
                    new TagToRemove( "<div class=\"wpa-notice", "</div>"),
                    new TagToRemove( "<div class=\"u", "</div>"),
                    new TagToRemove( "<div class=\"wpa", "</div>"),
                    //  Sharing buttons (by groups of tags).
                    new TagToRemove( "<div class=\"sd-content", "</div>"),
                    new TagToRemove( "<div class=\"robots-nocontent", "</div>"),
                    new TagToRemove( "<div class=\"sharedaddy", "</div>"),

                    new TagToRemove( "<div class=\'likes-", "</div>"),
                    new TagToRemove( "<div class=\'sharedaddy", "</div>"),

                    new TagToRemove( "<div id=\'jp-relatedposts", "</div>"),
                    new TagToRemove( "<div id=\"jp-post-flair", "</div>"),

                    new TagToRemove( "<div class=\"wpcnt", "</div>"),
                    //  Other tags.
                    new TagToRemove( "<button", "</button>" ),
                    new TagToRemove( "<aside", "</aside>" ),
                    new TagToRemove( "<!--[if", "<![endif]-->" ),
                    new TagToRemove( "<!--", "" )
                })
            };
            return result;
        }

        protected override InnerTagRemover GetInnerTagRemover(TextProcessor next)
        {
            var result = new InnerTagRemover(next)
            {
                Tags = new List<TagToRemove>(new TagToRemove[] {
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
                new TagToRemove( "<body", "</body>" )
            })
            };
            return result;
        }

        protected override TextProcessor CreateProcessingChain()
        {
            return  
                //  Создает последовательность обработки (имеет значение).
                //  At first extracts content of the <article> tag.
                GetParagraphExtractor(
                    GetTagWithTextRemover(
                        new SpecialHTMLRemover(
                            new UrlFormatter(
                                GetInnerTagRemover(
                                    new TextFormatter(null)
                                )))));
        }

        protected override ParagraphExtractor GetParagraphExtractor(TextProcessor next)
        {
            return new ParagraphExtractor(next)
            {
                Skipped = false,
                Tag = new HtmlTag("<article", "</article>")
            };
        }

        protected override string GetConfigurationFileName()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "WordPressHTMLCleanerConfig.xml";
        }
    }
}
