using System.Collections.Generic;

namespace HTMLCleanup
{
    public class HtmlCleanerConfigItem
    {
        public string urlPrefix;
        public string htmlCleanerType;
    }

    interface IInjectorConfig
    {
        /// <summary>
        /// Returns list of cleaner type names.
        /// </summary>
        /// <returns>List of cleaner type names.</returns>
        List<HtmlCleanerConfigItem> GetCleanerList();
    }
}
