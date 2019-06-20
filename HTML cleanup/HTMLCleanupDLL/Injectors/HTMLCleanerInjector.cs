using System;

namespace HtmlCleanup
{
    public class HtmlCleanerInjector
    {
        private IInjectorConfig _config;
        private ICleanerConfigSerializer _configSerializer;

        public HtmlCleanerInjector(IInjectorConfig config, ICleanerConfigSerializer configSerializer)
        {
            _config = config;
            _configSerializer = configSerializer;
        }

        public IHtmlCleaner CreateHtmlCleaner(string url)
        {
            var list = _config.GetCleanerList();
            var formatterType = Type.GetType(_config.GetFormatterType());

            foreach (var item in list)
            {
                if (url.Contains(item.urlPrefix))
                {
                    var cleanerType = Type.GetType(item.htmlCleanerType);
                    var formatter = Activator.CreateInstance(formatterType) as ITagFormatter;
                    var cleaner = Activator.CreateInstance(cleanerType, new object[] { _configSerializer }) as IHtmlCleaner;
                    cleaner.SetFormatter(formatter);
                    return cleaner;
                }
            }
            //  Default HTML parser.
            return new UniversalHtmlCleaner(_configSerializer);
        }
    }
}
