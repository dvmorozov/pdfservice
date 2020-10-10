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
using Microsoft.AspNetCore.Mvc;
using HTMLToPDFConvertingDLL;

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
