using System.Collections.Generic;

namespace HTMLCleanup
{
    class BaseInjectorConfig : IInjectorConfig
    {
        public List<HtmlCleanerConfigItem> GetCleanerList()
        {
            return new List<HtmlCleanerConfigItem>() {
                new HtmlCleanerConfigItem() { urlPrefix = "https://rationalcity.wordpress.com/", htmlCleanerType = "WordPressHtmlCleaner" },
                new HtmlCleanerConfigItem() { urlPrefix = "", htmlCleanerType = "UniversalHtmlCleaner" }
            };
        }
    }
}
