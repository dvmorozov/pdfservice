using HtmlCleanup;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                return View("Index", new UrlToPdfData { Url = url, UrlToPdf = urlToPdf, AdobeViewMode = adobeViewMode, FileName = UrlToFileName(url) });
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
        private string UrlToFileName(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            var uriBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Scheme = "https",
                Host = "adobesdk.azurewebsites.net",    //  "localhost"
                Port = 443,                             //  44379
                Path = "pdf/filename",
                Query = "url=" + url
            };

            var request = uriBuilder.ToString();
            var req = WebRequest.CreateHttp(request);
            req.Timeout = 30000;
            req.ContentType = "text/plain";
            req.Method = "GET";

            var res = req.GetResponse();

            var streamReader = new StreamReader(res.GetResponseStream());
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Cleans and converts URL to PDF.
        /// </summary>
        /// <param name="url">Original URL.</param>
        /// <returns>URL to PDF file.</returns>
        private string ConvertByCleaner(string url)
        {
            var injector = new HtmlCleanerInjector(new BaseInjectorConfig(), new WebCleanerConfigSerializer(Server));
            //  Creating cleaner instance based on URL.
            var processChain = injector.CreateHtmlCleaner(url);

            //  Performs request.
            var s = HtmlCleanerApp.MakeRequest(url);

            processChain.Process(s);

            var formatter = processChain.GetFormatter();

            //  Finishes processing.
            formatter.CloseDocument();
            var dataStream = formatter.GetOutputStream();

            var pdfFileName = UrlToFileName(url);
            var pdfFilePath = Server.MapPath("~") + "Content\\" + pdfFileName;

            if (dataStream != null)
            {
                using (var fileStream = System.IO.File.Create(pdfFilePath))
                {
                    dataStream.Seek(0, SeekOrigin.Begin);
                    dataStream.CopyTo(fileStream);
                }
            }

            var urlBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Path = Url.Content("~/Content/" + pdfFileName),
                Query = null,
            };
            return urlBuilder.ToString();
        }

        [AllowAnonymous]
        public ActionResult UrlToPdf(string url, string adobeViewMode, bool CleanHtml)
        {
            try
            {
                var urlToPdf = string.Empty;
                if (url != null)
                {
                    if (CleanHtml)
                    {
                        urlToPdf = ConvertByCleaner(url);
                    }
                    else
                    {
                        urlToPdf = ConvertByAdobeSdk(url);
                    }
                }
                return RedirectToAction("Index", "Pdf", new UrlToPdfData { Url = url, UrlToPdf = urlToPdf, AdobeViewMode = adobeViewMode, FileName = UrlToFileName(url) });
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Pdf", "Index"));
            }
        }

        /// <summary>
        /// Requests external service for conversion original URL to PDF.
        /// </summary>
        /// <param name="url">Original URL.</param>
        /// <returns>URL to PDF file.</returns>
        private string RequestConvertingService(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            var uriBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Scheme = "https",
                Host = "adobesdk.azurewebsites.net",    //  "localhost"
                Port = 443,                             //  44379
                Path = "pdf/document",
                Query = "url=" + url
            };

            var request = uriBuilder.ToString();
            var req = WebRequest.CreateHttp(request);
            req.Timeout = 30000;
            req.ContentType = "application/json";
            req.Method = "GET";

            var res = req.GetResponse();

            var streamReader = new StreamReader(res.GetResponseStream());
            var convertHtmlToPdf = JObject.Parse(streamReader.ReadToEnd());
            return convertHtmlToPdf["urlToPdf"].ToString();
        }

        /// <summary>
        /// Returns URL to PDF file.
        /// </summary>
        /// <param name="url">Original URL</param>
        /// <returns>URL to PDF.</returns>
        private string ConvertByAdobeSdk(string url)
        {
            return RequestConvertingService(url);
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View("Error");
        }
    }
}
