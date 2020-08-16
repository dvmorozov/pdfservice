using System;

namespace HtmlCleanup
{
    public class HtmlCleanerInjector
    {
        private readonly IInjectorConfig _config;
        private readonly ICleanerConfigSerializer _configSerializer;

        public HtmlCleanerInjector(IInjectorConfig config, ICleanerConfigSerializer configSerializer)
        {
            _config = config;
            _configSerializer = configSerializer;
        }

        public IHtmlCleaner CreateHtmlCleaner(string url)
        {
            System.Collections.Generic.List<HtmlCleanerConfigItem> list = _config.GetCleanerList();
            Type formatterType = Type.GetType(_config.GetFormatterType());

            foreach (HtmlCleanerConfigItem item in list)
            {
                if (url.Contains(item.urlPrefix))
                {
                    Type cleanerType = Type.GetType(item.htmlCleanerType);
                    ITagFormatter formatter = Activator.CreateInstance(formatterType) as ITagFormatter;
                    IHtmlCleaner cleaner = Activator.CreateInstance(cleanerType, new object[] { _configSerializer }) as IHtmlCleaner;
                    cleaner.SetFormatter(formatter);
                    return cleaner;
                }
            }
            //  Default HTML parser.
            return new UniversalHtmlCleaner(_configSerializer);
        }
    }
}
