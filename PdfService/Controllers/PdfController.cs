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
        private string UrlToFileName(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            UriBuilder uriBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Scheme = "https",
                Host = "adobesdk.azurewebsites.net",    //  "localhost"
                Port = 443,                             //  44379
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
        private string GetContentUri(string fileName)
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
        private string GetContentPath(string fileName)
        {
            return Server.MapPath("~") + "Content\\" + fileName;
        }

        /// <summary>
        /// Cleans and converts URL to PDF.
        /// </summary>
        /// <param name="url">Original URL.</param>
        /// <returns>URL to PDF file.</returns>
        private string ConvertByCleaner(string url)
        {
            HtmlCleanerInjector injector = new HtmlCleanerInjector(new BaseInjectorConfig(), new WebCleanerConfigSerializer(Server));
            //  Creating cleaner instance based on URL.
            IHtmlCleaner processChain = injector.CreateHtmlCleaner(url);

            //  Performs request.
            string s = HtmlCleanerApp.MakeRequest(url);

            _ = processChain.Process(s);

            ITagFormatter formatter = processChain.GetFormatter();

            //  Finishes processing.
            formatter.CloseDocument();
            using (MemoryStream dataStream = formatter.GetOutputStream())
            {
                string pdfFileName = UrlToFileName(url);
                string pdfFilePath = GetContentPath(pdfFileName);

                if (dataStream != null)
                {
                    using (FileStream fileStream = System.IO.File.Create(pdfFilePath))
                    {
                        dataStream.Seek(0, SeekOrigin.Begin);
                        dataStream.CopyTo(fileStream);
                    }
                }

                return GetContentUri(pdfFileName);
            }
        }

        [AllowAnonymous]
        public ActionResult UrlToPdf(string url, string adobeViewMode, bool cleanHtml)
        {
            try
            {
                string urlToPdf = string.Empty;
                if (url != null)
                {
                    urlToPdf = cleanHtml ? ConvertByCleaner(url) : ConvertByAdobeSdk(url);
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
        /// <returns>File name.</returns>
        private string RequestConvertingService(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            UriBuilder uriBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Scheme = "https",
                Host = "adobesdk.azurewebsites.net",    //  "localhost"
                Port = 443,                             //  44379
                Path = "pdf/document",
                Query = "url=" + url
            };

            string request = uriBuilder.ToString();
            HttpWebRequest req = WebRequest.CreateHttp(request);
            req.Timeout = 30000;
            req.ContentType = "application/json";
            req.Method = "GET";

            using (WebResponse res = req.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(res.GetResponseStream()))
                {
                    return JObject.Parse(streamReader.ReadToEnd())["fileName"].ToString();
                }
            }
        }

        /// <summary>
        /// Copies data from stream to local file.
        /// </summary>
        /// <param name="dataStream">Data stream.</param>
        /// <param name="filePath">Path to local file.</param>
        private void CopyStreamToFile(Stream dataStream, string filePath)
        {
            if (dataStream != null)
            {
                using (FileStream fileStream = System.IO.File.Create(filePath))
                {
                    dataStream.CopyTo(fileStream);
                }
            }
        }

        /// <summary>
        /// Copies file from converting service to local folder and deletes it.
        /// </summary>
        /// <param name="fileName">Name of converted file.</param>
        private void GetFileFromConvertingService(string fileName)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            UriBuilder uriBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Scheme = "https",
                Host = "adobesdk.azurewebsites.net",    //  "localhost"
                Port = 443,                             //  44379
                Path = "pdf/file",
                Query = "fileName=" + fileName + "&delete=true"
            };

            string request = uriBuilder.ToString();
            HttpWebRequest req = WebRequest.CreateHttp(request);
            req.Timeout = 30000;
            req.ContentType = "application/pdf";
            req.Method = "GET";

            using (WebResponse res = req.GetResponse())
            {
                using (Stream dataStream = res.GetResponseStream())
                {
                    string pdfFilePath = GetContentPath(fileName);
                    CopyStreamToFile(dataStream, pdfFilePath);
                }
            }
        }

        /// <summary>
        /// Returns URL to PDF file.
        /// </summary>
        /// <param name="url">Original URL</param>
        /// <returns>URL to PDF.</returns>
        private string ConvertByAdobeSdk(string url)
        {
            string fileName = RequestConvertingService(url);
            GetFileFromConvertingService(fileName);
            return GetContentUri(fileName);
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View("Error");
        }
    }
}
