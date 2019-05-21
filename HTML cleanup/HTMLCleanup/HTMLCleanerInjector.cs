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
