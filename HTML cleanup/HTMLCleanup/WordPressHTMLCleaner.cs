﻿using System.Collections.Generic;
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
                    new TagToRemove( "<svg", "</svg>" ),
                    new TagToRemove( "<sup", "</sup>" ),
                    new TagToRemove( "<label", "</label>" ),
                    new TagToRemove( "<input", "" ),
                    new TagToRemove( "<img", "" ),
                    new TagToRemove( "<iframe", "</iframe>" ),
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
                    new TagToRemove( "<br", "" ),
                    new TagToRemove( "<aside", "</aside>" ),
                    //  Hyperlinks are removed.
                    new TagToRemove( "<a", "</a>" ),
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
                new TagToRemove( "<u", "</u>" ),
                //  Removing tables.
                new TagToRemove( "<td", "</td>" ),
                new TagToRemove( "<tr", "</tr>" ),
                new TagToRemove( "<tbody", "</tbody>" ),
                new TagToRemove( "<table", "</table>" ),
                //  Other tags.
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
                new TagToRemove( "<h4", "</h4>" ),
                new TagToRemove( "<h3", "</h3>" ),
                new TagToRemove( "<h3", "</h3>" ),
                new TagToRemove( "<h2", "</h2>" ),
                new TagToRemove( "<h1", "</h1>" ),
                new TagToRemove( "<footer", "</footer>" ),
                new TagToRemove( "<em", "</em>" ),
                new TagToRemove( "<div", "</div>" ),
                new TagToRemove( "<code", "</code>" ),
                new TagToRemove( "<body", "</body>" ),
                new TagToRemove( "<blockquote", "</blockquote>")
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
                    GetTagWithTextRemover(
                        //  Replacing tags with text is done before replacing
                        //  special characters to avoid interpreting text as HTML tags.
                        GetInnerTagRemover(
                            new UrlFormatter(
                                new SpecialHTMLRemover(
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
