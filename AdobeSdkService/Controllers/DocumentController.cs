using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdobeSdkService.Controllers
{
    [ApiController]
    [Route("pdf/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public DocumentController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private string GetBaseUrl()
        {
            var request = this.Request;
            var host = request.Host.ToUriComponent();
            var pathBase = request.PathBase.ToUriComponent();

            return $"{request.Scheme}://{host}{pathBase}";
        }

        private string GetStaticUrl(string staticFileName)
        {
            return GetBaseUrl() + (new PathString("/" + staticFileName)).ToUriComponent();
        }

        [HttpGet]
        public HtmlToPdfResult Get(string url)
        {
            try
            {
                if (url != null && url != "")
                {
                    var pdfFileName = FileNameController.UrlToFileName(url, ".pdf");
                    var pdfFilePath = Path.Combine(Directory.GetCurrentDirectory(), pdfFileName);

                    var htmlToPdfConverter = new HtmlToPdfConverter(url, pdfFilePath);

                    try
                    {
                        // Configure the logging.
                        htmlToPdfConverter.ConfigureLogging();

                        // Read HTML, convert and write PDF.
                        htmlToPdfConverter.ConvertFileToPdf();
                    }
                    finally
                    {
                        //htmlToPdfConverter.CleanUp();
                    }

                    return new HtmlToPdfResult { UrlToPdf = GetStaticUrl(pdfFileName), FileName = pdfFileName, Message = "Converted successfully." };
                }
                else
                {
                    return new HtmlToPdfResult { Message = "Provide URL for conversion via \"url\" parameter: https://<host>/pdf/?url=<url>." };
                }
            }
            catch (Exception e)
            {
                return new HtmlToPdfResult { Message = "Exception: " + e.Message };
            }
        }
    }
}
