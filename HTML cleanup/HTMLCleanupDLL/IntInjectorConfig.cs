using System.Collections.Generic;

namespace HtmlCleanup
{
    public class HtmlCleanerConfigItem
    {
        public string urlPrefix;
        public string htmlCleanerType;
    }

    public interface IInjectorConfig
    {
        /// <summary>
        /// Returns list of cleaner type names.
        /// </summary>
        /// <returns>List of cleaner type names.</returns>
        List<HtmlCleanerConfigItem> GetCleanerList();
        /// <summary>
        /// Returns name of formatter type.
        /// </summary>
        /// <returns>Formatter type name.</returns>
        string GetFormatterType();
    }
}
