using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

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
                return View("Index", new UrlToPdfData { Url = url, UrlToPdf = urlToPdf, AdobeViewMode = adobeViewMode, FileName = UrlToFileName(url, ".pdf") });
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
        }

        /// <summary>
        /// Prepares file name from URL.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="fileExtension">File extension including dot.</param>
        /// <returns></returns>
        private string UrlToFileName(string url, string fileExtension)
        {
            if (url != null)
            {
                var fileName = url;
                var prefixIndex = fileName.IndexOf("://");
                if (prefixIndex != -1)
                    fileName = fileName.Substring(prefixIndex + 3);

                fileName = fileName.Replace('/', '_');
                fileName = fileName.Replace('.', '_');
                fileName = fileName.TrimEnd('_');
                //  Adds file extension.
                fileName += fileExtension;
                return fileName;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Writes response data.
        /// </summary>
        /// <param name="dataStream">File data.</param>
        /// <param name="contentType">MIME content type.</param>
        /// <param name="fileName">Full file name including extension.</param>
        private void SendFileResponse(MemoryStream dataStream, string contentType, string fileName)
        {
            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", "inline;filename=" + fileName);
            Response.BinaryWrite(dataStream.ToArray());
            Response.Flush();
            Response.End();
        }

        [AllowAnonymous]
        public ActionResult UrlToPdf(string url, string adobeViewMode)
        {
            try
            {
                var urlToPdf = string.Empty;
                if (url != null)
                {
                    urlToPdf = GetUrlToPdf(url);
                }
                return RedirectToAction("Index", "Pdf", new UrlToPdfData { Url = url, UrlToPdf = urlToPdf, AdobeViewMode = adobeViewMode, FileName = UrlToFileName(url, ".pdf") });
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
            var uriBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Scheme = "https",
                Host = "localhost", //  "adobesdk.azurewebsites.net"
                Port = 44379,       //  443
                Path = "converthtmltopdf",
                Query = "url=" + url
            };

            var request = uriBuilder.ToString();
            var req = WebRequest.CreateHttp(request);
            req.Timeout = 60000;
            req.ContentType = "application/json";
            req.Method = "GET";

            var res = req.GetResponse();

            var streamReader = new StreamReader(res.GetResponseStream());
            var convertHtmlToPdf = JObject.Parse(streamReader.ReadToEnd());
            return convertHtmlToPdf["urlToPdf"].ToString();
        }

        /// <summary>
        /// Executes local converting utility.
        /// </summary>
        /// <param name="url">Original URL.</param>
        /// <returns>URL to PDF file.</returns>
        private string ExecuteLocalConverter(string url)
        {
            var pdfFileName = UrlToFileName(url, ".pdf");
            var converterPath = Server.MapPath("~") + "PdfCreator\\create_pdf_from_html.bat";
            var pdfFilePath = Server.MapPath("~") + "Content\\" + pdfFileName;
            var arguments = url + " " + pdfFilePath;

            var startInfo = new ProcessStartInfo(converterPath, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WorkingDirectory = Server.MapPath("~") + "PdfCreator\\"
            };

            var process = Process.Start(startInfo);
            process.WaitForExit();
            if (0 != process.ExitCode)
            {
                throw new Exception(process.StandardOutput.ReadToEnd());
            }

            var urlBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Path = Url.Content("~/Content/" + pdfFileName),
                Query = null,
            };
            return urlBuilder.ToString();
        }

        /// <summary>
        /// Returns URL to PDF file.
        /// </summary>
        /// <param name="url">Original URL</param>
        /// <returns></returns>
        private string GetUrlToPdf(string url)
        {
            return ExecuteLocalConverter(url);
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View("Error");
        }
    }
}
