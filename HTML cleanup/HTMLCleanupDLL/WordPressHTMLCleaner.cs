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
                Tags = new List<HtmlTag>(new HtmlTag[] {
                    new HtmlTag( "<script", "</script>" ),
                    new HtmlTag( "<style", "</style>" ),
                    new HtmlTag( "<link", "" ),
                    new HtmlTag( "<path", "</path>" ),
                    new HtmlTag( "<meta", "" ),
                    new HtmlTag( "<svg", "</svg>" ),
                    new HtmlTag( "<sup", "</sup>" ),
                    new HtmlTag( "<label", "</label>" ),
                    new HtmlTag( "<input", "" ),
                    new HtmlTag( "<img", "" ),
                    new HtmlTag( "<iframe", "</iframe>" ),
                    new HtmlTag( "<footer", "</footer>" ),
                    new HtmlTag( "<form", "</form>" ),
                    new HtmlTag( "<noscript", "</noscript>" ),
                    new HtmlTag( "<nav", "</nav>" ),
                    new HtmlTag( "<!DOCTYPE", "" ),
                    //  Advertising block and internal divs.
                    //  Items should be in the order reverse
                    //  to the nesting of divs (best possible
                    //  option for this primitive parser).
                    new HtmlTag( "<div id=\"atatags", "</div>"),
                    new HtmlTag( "<div style=\"", "</div>"),
                    new HtmlTag( "<div class=\"wpa-notice", "</div>"),
                    new HtmlTag( "<div class=\"u", "</div>"),
                    new HtmlTag( "<div class=\"wpa", "</div>"),
                    //  Sharing buttons (by groups of tags).
                    new HtmlTag( "<div class=\"sd-content", "</div>"),
                    new HtmlTag( "<div class=\"robots-nocontent", "</div>"),
                    new HtmlTag( "<div class=\"sharedaddy", "</div>"),

                    new HtmlTag( "<div class=\'likes-", "</div>"),
                    new HtmlTag( "<div class=\'sharedaddy", "</div>"),

                    new HtmlTag( "<div id=\'jp-relatedposts", "</div>"),
                    new HtmlTag( "<div id=\"jp-post-flair", "</div>"),

                    new HtmlTag( "<div class=\"wpcnt", "</div>"),
                    //  Other tags.
                    new HtmlTag( "<button", "</button>" ),
                    new HtmlTag( "<br", "" ),
                    new HtmlTag( "<aside", "</aside>" ),
                    //  Hyperlinks are removed.
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
                    new HtmlTag( "<u", "</u>" ),
                    //  Removing tables.
                    new HtmlTag( "<td", "</td>" ),
                    new HtmlTag( "<tr", "</tr>" ),
                    new HtmlTag( "<tbody", "</tbody>" ),
                    new HtmlTag( "<table", "</table>" ),
                    //  Other tags.
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
                    new HtmlTag( "<blockquote", "</blockquote>"),
                    new HtmlTag( "<a", "</a>", new string[] { "href" })
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
