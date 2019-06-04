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
        public ActionResult Index()
        {
            try
            {
                return View("Index");
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
        }

        [AllowAnonymous]
        public ActionResult UrlToPdf(string url)
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

                        var contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                        return File(dataStream.ToArray(), contentType, "content.pdf");
                    }
                }
                return View("Index");
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
