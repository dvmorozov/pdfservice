using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using EnterpriseServices.HtmlToPdf;
using System.Configuration;
using System.Web.WebPages;

namespace EnterpriseServices.Controllers
{
    [Authorize]
    public partial class PdfController : ErrorHandlingController
    {
        [AllowAnonymous]
        public ActionResult Index(string url, string urlToPdf, string adobeViewMode)
        {
            try
            {
                return url != null && urlToPdf == null
                    ? RedirectToAction("UrlToPdf", "Pdf", new { url, adobeViewMode, cleanHtml = false })
                    : (ActionResult)View("Index", new UrlToPdfData { Url = url, UrlToPdf = urlToPdf, AdobeViewMode = adobeViewMode, FileName = UrlToFileName(url) });
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Pdf", "Index"));
            }
        }

        public class UrlToPdfData
        {
            [Required]
            public string Url { get; set; }
            public string AdobeViewMode { get; set; }
            public string UrlToPdf { get; set; }
            public string FileName { get; set; }
            public bool CleanHtml { get; set; }
        }

        /// <summary>
        /// Prepares file name from URL.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="fileExtension">File extension including dot.</param>
        /// <returns></returns>
        public string UrlToFileName(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            UriBuilder uriBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Scheme = "https",
#if DEBUG
                Host = "localhost",
                Port = 44379,
#else
                Host = "adobesdk.azurewebsites.net",
                Port = 443,
#endif
                Path = "pdf/filename",
                Query = "url=" + url
            };

            string request = uriBuilder.ToString();
            HttpWebRequest req = WebRequest.CreateHttp(request);
            req.Timeout = 30000;
            req.ContentType = "text/plain";
            req.Method = "GET";

            using (WebResponse res = req.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(res.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Returns URL to local content file.
        /// </summary>
        /// <param name="fileName">Content file name.</param>
        /// <returns>URL.</returns>
        public string GetContentUri(string fileName)
        {
            UriBuilder urlBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Path = Url.Content("~/Content/" + fileName),
                Query = null,
            };
            return urlBuilder.ToString();
        }

        /// <summary>
        /// Returns full path to content file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>Path.</returns>
        public string GetContentPath(string fileName)
        {
            return Path.Combine(Server.MapPath("~"), "Content", fileName);
        }

        /// <summary>
        /// Cleans and converts URL to PDF.
        /// </summary>
        /// <param name="url">Original URL.</param>
        /// <returns>URL to PDF file.</returns>
        private string ConvertByITextCleaner(string url)
        {
            HtmlToPdfByITextCleaner converter = new HtmlToPdfByITextCleaner(this);
            return converter.GetUrlToPdf(url);
        }

        [AllowAnonymous]
        public ActionResult UrlToPdf(string url, string adobeViewMode, bool cleanHtml)
        {
            try
            {
                bool convertByAdobeSdk = false;
#if DEBUG
                bool.TryParse(ConfigurationManager.AppSettings["ConvertByAdobeSdk"], out convertByAdobeSdk);
#else
                bool.TryParse(CloudConfigurationManager.GetSetting("ConvertByAdobeSdk"), out convertByAdobeSdk);
#endif
                string urlToPdf = string.Empty;
                if (url != null)
                {
                    urlToPdf = cleanHtml ? ConvertByITextCleaner(url) : (convertByAdobeSdk ? ConvertByAdobeSdk(url) : ConvertByLocalApp(url));
                }
                return RedirectToAction("Index", "Pdf", new UrlToPdfData { Url = url, UrlToPdf = urlToPdf, AdobeViewMode = adobeViewMode, FileName = UrlToFileName(url) });
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Pdf", "Index"));
            }
        }

        /// <summary>
        /// Returns URL to PDF file.
        /// </summary>
        /// <param name="url">Original URL</param>
        /// <returns>URL to PDF.</returns>
        private string ConvertByAdobeSdk(string url)
        {
            HtmlToPdfByAdobeSdk converter = new HtmlToPdfByAdobeSdk(this);
            return converter.GetUrlToPdf(url);
        }

        /// <summary>
        /// Returns URL to PDF file.
        /// </summary>
        /// <param name="url">Original URL</param>
        /// <returns>URL to PDF.</returns>
        private string ConvertByLocalApp(string url)
        {
#if DEBUG
            string localConvertingApp = ConfigurationManager.AppSettings["LocalConvertingApp"];
#else
            string localConvertingApp = CloudConfigurationManager.GetSetting("LocalConvertingApp");
#endif
            if (localConvertingApp.IsEmpty())
            {
                localConvertingApp = "Wkhtmltopdf";
            }
            HtmlToPdfByLocalApp converter = new HtmlToPdfByLocalApp(this, localConvertingApp);
            return converter.GetUrlToPdf(url);
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View("Error");
        }
    }
}
