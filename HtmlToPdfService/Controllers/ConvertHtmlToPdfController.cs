using System;
using Microsoft.AspNetCore.Hosting;
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

        [HttpGet]
        public ConvertHtmlToPdf Get(string url)
        {
            var pdfFileName = UrlToFileName(url, ".pdf");
            var contentRoot = System.IO.Path.Combine(_env.ContentRootPath, "Content");
            var pdfFilePath = System.IO.Path.Combine(contentRoot, pdfFileName);

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
                htmlToPdfConverter.CleanUp();
            }

            return new ConvertHtmlToPdf { UrlToPdf = "Content/" + pdfFileName, FileName = pdfFileName };
        }
    }
}
