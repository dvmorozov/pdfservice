using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseServices.HtmlToPdf
{
    interface HtmlToPdf
    {
        /// <summary>
        /// Convert HTML-page to local PDF-file.
        /// </summary>
        /// <param name="url">Original URL to HTML-page.</param>
        /// <returns>URL to PDF-file.</returns>
        string GetUrlToPdf(string url);
    }
}
