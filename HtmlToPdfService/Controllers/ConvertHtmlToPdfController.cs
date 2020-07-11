using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HtmlToPdfService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertHtmlToPdfController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ConvertHtmlToPdfController(IWebHostEnvironment env)
        {
            _env = env;
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

        private string GetBaseUrl()
        {
            var request = this.Request;
            var host = request.Host.ToUriComponent();
            var pathBase = request.PathBase.ToUriComponent();

            return $"{request.Scheme}://{host}{pathBase}";
        }

        private string GetStaticUrl(string staticFileName, string staticRoot)
        {
            return GetBaseUrl() + (new PathString("/" + staticRoot).Add("/" + staticFileName)).ToUriComponent();
        }

        [HttpGet]
        public ConvertHtmlToPdf Get(string url)
        {
            try
            {
                if (url != null && url != "")
                {
                    var pdfFileName = UrlToFileName(url, ".pdf");
                    var pdfFilePath = Path.Combine(_env.ContentRootPath, "wwwroot", "Content", pdfFileName);

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

                    return new ConvertHtmlToPdf { UrlToPdf = GetStaticUrl(pdfFileName, "Content"), FileName = pdfFileName, Message = "Converted successfully." };
                }
                else
                {
                    return new ConvertHtmlToPdf { Message = "Provide URL for conversion via \"url\" parameter: https://<host>/converthtmltopdf/?url=<url>." };
                }
            }
            catch (Exception e)
            {
                return new ConvertHtmlToPdf { Message = "Exception: " + e.Message };
            }
        }
    }
}
