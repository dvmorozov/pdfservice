using System;

namespace HtmlCleanup
{
    class HtmlCleanerInjector
    {
        private IInjectorConfig _config;

        public HtmlCleanerInjector(IInjectorConfig config)
        {
            _config = config;
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
                    var cleaner = Activator.CreateInstance(cleanerType) as IHtmlCleaner;
                    cleaner.SetFormatter(formatter);
                    return cleaner;
                }
            }
            //  Default HTML parser.
            return new UniversalHtmlCleaner();
        }
    }
}
