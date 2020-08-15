using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdobeSdkService.Controllers
{
    [Route("pdf/[controller]")]
    [ApiController]
    public class FileNameController : ControllerBase
    {

        /// <summary>
        /// Prepares file name from URL.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="fileExtension">File extension including dot.</param>
        /// <returns>File name.</returns>
        public static string UrlToFileName(string url, string fileExtension)
        {
            if (url != null)
            {
                int prefixIndex = url.IndexOf("://");
                string fileName = url;
                if (prefixIndex != -1)
                {
                    fileName = url.Substring(prefixIndex + 3);
                }

                fileName = HtmlToPdfConverter.ReplaceInvalidCharacters(fileName);

                //  Adds file extension.
                fileName += fileExtension;
                return fileName;
            }
            else
            {
                return "";
            }
        }

        [HttpGet]
        public string Get(string url)
        {
            return UrlToFileName(url, ".pdf");
        }
    }
}
