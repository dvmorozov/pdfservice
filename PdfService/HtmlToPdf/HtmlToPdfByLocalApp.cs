/*
 “Commons Clause” License Condition v1.0
The Software is provided to you by the Licensor under the License, as defined below, subject to the following condition.
Without limiting other conditions in the License, the grant of rights under the License will not include, and the License
does not grant to you, right to Sell the Software. For purposes of the foregoing, “Sell” means practicing any or all of
the rights granted to you under the License to provide to third parties, for a fee or other consideration (including
without limitation fees for hosting or consulting/ support services related to the Software), a product or service whose
value derives, entirely or substantially, from the functionality of the Software.  Any license notice or attribution
required by the License must also include this Commons Cause License Condition notice.

Software: PdfService

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
using EnterpriseServices.Controllers;
using System;
using System.Diagnostics;

namespace EnterpriseServices.HtmlToPdf
{
    public class HtmlToPdfByLocalApp
    {
        private readonly PdfController _pdfController;
        private readonly string _localAppDirectory;

        public HtmlToPdfByLocalApp(PdfController pdfController, string localAppDirectory)
        {
            _pdfController = pdfController;
            _localAppDirectory = localAppDirectory;
        }

        public string GetUrlToPdf(string url)
        {
            string pdfFileName = _pdfController.UrlToFileName(url);
            string converterPath = _pdfController.Server.MapPath("~") + _localAppDirectory + "\\create_pdf_from_html.bat";
            string pdfFilePath = _pdfController.Server.MapPath("~") + "Content\\" + pdfFileName;
            string arguments = url + " " + pdfFilePath;

            ProcessStartInfo startInfo = new ProcessStartInfo(converterPath, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _pdfController.Server.MapPath("~") + _localAppDirectory + "\\"
            };

            Process process = Process.Start(startInfo);
            process.WaitForExit();

            UriBuilder urlBuilder = new UriBuilder(_pdfController.Request.Url.AbsoluteUri)
            {
                Path = _pdfController.Url.Content("~/Content/" + pdfFileName),
                Query = null,
            };

            return urlBuilder.ToString();
        }
    }
}