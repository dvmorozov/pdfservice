using System.Collections.Generic;

namespace HtmlCleanup
{
    class BaseInjectorConfig : IInjectorConfig
    {
        public List<HtmlCleanerConfigItem> GetCleanerList()
        {
            return new List<HtmlCleanerConfigItem>() {
                new HtmlCleanerConfigItem() {
                    urlPrefix = "https://rationalcity.wordpress.com/",
                    htmlCleanerType = "HtmlCleanup.WordPressHtmlCleaner"
                }
            };
        }

        public string GetFormatterType()
        {
            return "HtmlCleanup.PdfFormatter";
        }
    }
}
