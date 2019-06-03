using System.Collections.Generic;
using System.IO;

namespace HtmlCleanup
{
    public class WordPressHtmlCleaner : BaseHtmlCleaner
    {
        public WordPressHtmlCleaner(ICleanerConfigSerializer configSerializer) : base(configSerializer) { }

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
                    new Tag( "<svg", "</svg>" ),
                    new Tag( "<sup", "</sup>" ),
                    new Tag( "<label", "</label>" ),
                    new Tag( "<input", "" ),
                    new Tag( "<img", "" ),
                    new Tag( "<iframe", "</iframe>" ),
                    new Tag( "<footer", "</footer>" ),
                    new Tag( "<form", "</form>" ),
                    new Tag( "<noscript", "</noscript>" ),
                    new Tag( "<nav", "</nav>" ),
                    new Tag( "<!DOCTYPE", "" ),
                    //  Advertising block and internal divs.
                    //  Items should be in the order reverse
                    //  to the nesting of divs (best possible
                    //  option for this primitive parser).
                    new Tag( "<div id=\"atatags", "</div>"),
                    new Tag( "<div style=\"", "</div>"),
                    new Tag( "<div class=\"wpa-notice", "</div>"),
                    new Tag( "<div class=\"u", "</div>"),
                    new Tag( "<div class=\"wpa", "</div>"),
                    //  Sharing buttons (by groups of tags).
                    new Tag( "<div class=\"sd-content", "</div>"),
                    new Tag( "<div class=\"robots-nocontent", "</div>"),
                    new Tag( "<div class=\"sharedaddy", "</div>"),

                    new Tag( "<div class=\'likes-", "</div>"),
                    new Tag( "<div class=\'sharedaddy", "</div>"),

                    new Tag( "<div id=\'jp-relatedposts", "</div>"),
                    new Tag( "<div id=\"jp-post-flair", "</div>"),

                    new Tag( "<div class=\"wpcnt", "</div>"),
                    //  Other tags.
                    new Tag( "<button", "</button>" ),
                    new Tag( "<br", "" ),
                    new Tag( "<aside", "</aside>" ),
                    //  Hyperlinks are removed.
                    new Tag( "<a", "</a>" ),
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
                    new Tag( "<u", "</u>" ),
                    //  Removing tables.
                    new Tag( "<td", "</td>" ),
                    new Tag( "<tr", "</tr>" ),
                    new Tag( "<tbody", "</tbody>" ),
                    new Tag( "<table", "</table>" ),
                    //  Other tags.
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
                    new Tag( "<blockquote", "</blockquote>")
                })
            };
            return result;
        }

        protected override TextProcessor CreateProcessingChain()
        {
            return
                //  Creates processing chain (nesting is important).
                //  At first extracts content of the <article> tag.
                GetParagraphExtractor(
                    GetTagRemover(
                        //  Replacing tags with text is done before replacing
                        //  special characters to avoid interpreting text as HTML tags.
                        GetInnerTextProcessor(
                            new UrlFormatter(
                                new TextFormatter(null,
                                _formatter),
                            _formatter),
                        _formatter),
                    _formatter),
                _formatter);
        }

        protected override ParagraphExtractor GetParagraphExtractor(TextProcessor next, ITagFormatter formatter)
        {
            return new ParagraphExtractor(next, formatter)
            {
                Skipped = false,
                Tag = new HtmlTag("<article", "</article>")
            };
        }

        protected override string GetConfigurationFileName()
        {
            return _configSerializer.GetConfigurationFilePath() + "\\" + "WordPressHTMLCleanerConfig.xml";
        }
    }
}
