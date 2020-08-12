using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdobeSdkService.Controllers
{
    [Route("pdf/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        /// <summary>
        /// Writes response data.
        /// </summary>
        /// <param name="dataStream">File data.</param>
        /// <param name="contentType">MIME content type.</param>
        /// <param name="fileName">Full file name including extension.</param>
        private async System.Threading.Tasks.Task SendFileResponseAsync(MemoryStream dataStream, string contentType, string fileName)
        {
            Response.Clear();
            Response.ContentType = contentType;
            Response.Headers.Add("Content-Disposition", "inline;filename=" + fileName);
            byte[] pdfData = dataStream.ToArray();
            await Response.Body.WriteAsync(pdfData, 0, pdfData.Length);
        }

        [HttpGet]
        public ActionResult Get(string fileName, bool delete = false)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using MemoryStream memoryStream = new MemoryStream();
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.CopyTo(memoryStream);
                SendFileResponseAsync(memoryStream, "application/pdf", fileName).Wait();
            }

            if (delete)
            {
                DeleteFile(fileName);
            }
            return new EmptyResult();
        }

        [HttpDelete("{fileName}")]
        public void Delete(string fileName)
        {
            DeleteFile(fileName);
        }

        private void DeleteFile(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
