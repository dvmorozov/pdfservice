/*
 “Commons Clause” License Condition v1.0
The Software is provided to you by the Licensor under the License, as defined below, subject to the following condition.
Without limiting other conditions in the License, the grant of rights under the License will not include, and the License
does not grant to you, right to Sell the Software. For purposes of the foregoing, “Sell” means practicing any or all of
the rights granted to you under the License to provide to third parties, for a fee or other consideration (including
without limitation fees for hosting or consulting/ support services related to the Software), a product or service whose
value derives, entirely or substantially, from the functionality of the Software.  Any license notice or attribution
required by the License must also include this Commons Cause License Condition notice.

Software: AdobeSdkService

License: 
Copyright 2020 Dmitry Morozov

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Licensor: Dmitry Morozov
 */
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
