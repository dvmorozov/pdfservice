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

        [HttpGet]
        public string Get(string url)
        {
            return UrlToFileName(url, ".pdf");
        }
    }
}
