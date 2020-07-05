using HtmlCleanup;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web.Mvc;

namespace EnterpriseServices.Controllers
{
    [Authorize]
    public partial class PdfController : ErrorHandlingController
    {
        [AllowAnonymous]
        public ActionResult Index(string url, string adobeViewMode)
        {
            try
            {
                return View("Index", new UrlToPdfData { Url = url, AdobeViewMode = adobeViewMode });
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

        [AllowAnonymous]
        public ActionResult UrlToPdf(string url, string adobeViewMode)
        {
            try
            {
                if (url != null)
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
                        return new EmptyResult();
                    }
                }
                return View("Index", new UrlToPdfData { Url = url, AdobeViewMode = /*adobeViewMode*/"IN_LINE" });
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Pdf", "Index"));
            }
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View("Error");
        }
    }
}
