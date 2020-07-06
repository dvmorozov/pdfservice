using HtmlCleanup;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;

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
                return View("Index", new UrlToPdfData { Url = url, UrlToPdf = urlToPdf, AdobeViewMode = adobeViewMode });
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
        }

        /// <summary>
        /// Prepares file name from URL.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="fileExtension">File extension including dot.</param>
        /// <returns></returns>
        private string UrlToFileName(string url, string fileExtension)
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

        private ActionResult SendPdfResponse(string url)
        {
            var injector = new HtmlCleanerInjector(new BaseInjectorConfig(), new WebCleanerConfigSerializer(Server));
            //  Creating cleaner instance based on URL.
            var processChain = injector.CreateHtmlCleaner(url);

            //  Performs request.
            var s = HtmlCleanerApp.MakeRequest(url);

            var output = processChain.Process(s);

            var formatter = processChain.GetFormatter();

            //  Finishes processing.
            formatter.CloseDocument();
            var dataStream = formatter.GetOutputStream();

            if (dataStream != null)
            {
                dataStream.Seek(0, SeekOrigin.Begin);
                SendFileResponse(dataStream, "application/pdf", UrlToFileName(url, ".pdf"));
            }
            return new EmptyResult();
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
                    //return SendPdfResponse(url);
                }
                return RedirectToAction("Index", "Pdf", new UrlToPdfData { Url = url, UrlToPdf = urlToPdf, AdobeViewMode = adobeViewMode});
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
        /// <returns></returns>
        private string GetUrlToPdf(string url)
        {
            var pdfFileName = UrlToFileName(url, ".pdf");
            var converterPath = Server.MapPath("~") + "PdfCreator\\create_pdf_from_html.bat";
            var pdfFilePath = Server.MapPath("~") + "Content\\" + pdfFileName;
            var arguments = url + " " + pdfFilePath;

            var startInfo = new ProcessStartInfo(converterPath, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Server.MapPath("~") + "PdfCreator\\"
            };

            var process = Process.Start(startInfo);
            process.WaitForExit();

            var urlBuilder = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Path = Url.Content("~/Content/" + pdfFileName),
                Query = null,
            };
            return urlBuilder.ToString();
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View("Error");
        }
    }
}
