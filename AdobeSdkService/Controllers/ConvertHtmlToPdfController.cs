using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdobeSdkService.Controllers
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
        /// <returns>File name.</returns>
        private string UrlToFileName(string url, string fileExtension)
        {
            if (url != null)
            {
                var fileName = url;
                var prefixIndex = fileName.IndexOf("://");
                if (prefixIndex != -1)
                    fileName = fileName.Substring(prefixIndex + 3);

                char[] forbidden = { '<', '>', ':', '"', '/', '\\', '|', '?', '*', '&', '#', '=' };

                foreach (char character in forbidden)
                {
                    fileName = fileName.Replace(character, '_');
                }

                fileName = fileName.Trim('_');
                //  Adds file extension.
                fileName += fileExtension;
                return fileName;
            }
            else
            {
                return "";
            }
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
                    var pdfFileName = UrlToFileName(url, ".pdf");
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
                    return new HtmlToPdfResult { Message = "Provide URL for conversion via \"url\" parameter: https://<host>/converthtmltopdf/?url=<url>." };
                }
            }
            catch (Exception e)
            {
                return new HtmlToPdfResult { Message = "Exception: " + e.Message };
            }
        }
    }
}
