using EnterpriseServices.Controllers;
using System;
using System.Diagnostics;

namespace EnterpriseServices.HtmlToPdf
{
    public class HtmlToPdfByPdfCreator
    {
        private readonly PdfController _pdfController;

        public HtmlToPdfByPdfCreator(PdfController pdfController)
        {
            _pdfController = pdfController;
        }

        public string GetUrlToPdf(string url)
        {
            string pdfFileName = _pdfController.UrlToFileName(url);
            string converterPath = _pdfController.Server.MapPath("~") + "PdfCreator\\create_pdf_from_html.bat";
            string pdfFilePath = _pdfController.Server.MapPath("~") + "Content\\" + pdfFileName;
            string arguments = url + " " + pdfFilePath;

            ProcessStartInfo startInfo = new ProcessStartInfo(converterPath, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _pdfController.Server.MapPath("~") + "PdfCreator\\"
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