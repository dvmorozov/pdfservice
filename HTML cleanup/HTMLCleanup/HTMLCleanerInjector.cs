using HtmlCleanup;
using System;

namespace HTMLCleanup
{
    class HTMLCleanerInjector
    {
        private IInjectorConfig _config;

        public HTMLCleanerInjector(IInjectorConfig config)
        {
            _config = config;
        }

        public IHtmlCleaner CreateHTMLCleaner(string url)
        {
            var list = _config.GetCleanerList();
            foreach (var item in list)
            {
                if (url.Contains(item.urlPrefix))
                {
                    var t = Type.GetType(item.htmlCleanerType);
                    return Activator.CreateInstance(t) as IHtmlCleaner;
                }
            }
            //  Default HTML parser.
            return new UniversalHtmlCleaner();
        }
    }
}
