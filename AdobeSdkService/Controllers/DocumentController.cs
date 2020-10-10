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
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HTMLToPDFConvertingDLL;

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
                        // Configures the logging.
                        htmlToPdfConverter.ConfigureLogging();

                        // Reads HTML, converts and writes PDF.
                        htmlToPdfConverter.ConvertFileToPdf();
                    }
                    finally
                    {
                        // Removes temporary files.
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
