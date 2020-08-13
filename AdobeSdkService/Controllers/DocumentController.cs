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
        public IWebHostEnvironment Env { get; }

        public DocumentController(IWebHostEnvironment env)
        {
            Env = env;
        }

        private string GetBaseUrl()
        {
            string host = Request.Host.ToUriComponent();
            string pathBase = Request.PathBase.ToUriComponent();

            return $"{Request.Scheme}://{host}{pathBase}";
        }

        private string GetStaticUrl(string staticFileName)
        {
            return GetBaseUrl() + new PathString("/" + staticFileName).ToUriComponent();
        }

        [HttpGet]
        public HtmlToPdfResult Get(string url)
        {
            try
            {
                if (url != null && url != "")
                {
                    string pdfFileName = FileNameController.UrlToFileName(url, ".pdf");
                    string pdfFilePath = Path.Combine(Directory.GetCurrentDirectory(), pdfFileName);

                    HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter(url, pdfFilePath);

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
                return new HtmlToPdfResult { Message = e.Message };
            }
        }
    }
}
