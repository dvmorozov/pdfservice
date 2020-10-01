using EnterpriseServices.Controllers;
using HtmlCleanup;
using System.IO;

namespace EnterpriseServices.HtmlToPdf
{
    public class HtmlToPdfByITextCleaner : IHtmlToPdf
    {
        private readonly PdfController _pdfController;

        public HtmlToPdfByITextCleaner(PdfController pdfController)
        {
            _pdfController = pdfController;
        }

        public string GetUrlToPdf(string url)
        {
            HtmlCleanerInjector injector = new HtmlCleanerInjector(new BaseInjectorConfig(), new WebCleanerConfigSerializer(_pdfController.Server));
            //  Creating cleaner instance based on URL.
            IHtmlCleaner processChain = injector.CreateHtmlCleaner(url);

            //  Performs request.
            string s = HtmlCleanerApp.MakeRequest(url);

            _ = processChain.Process(s);

            ITagFormatter formatter = processChain.GetFormatter();

            //  Finishes processing.
            formatter.CloseDocument();
            using (MemoryStream dataStream = formatter.GetOutputStream())
            {
                string pdfFileName = _pdfController.UrlToFileName(url);
                string pdfFilePath = _pdfController.GetContentPath(pdfFileName);

                if (dataStream != null)
                {
                    using (FileStream fileStream = System.IO.File.Create(pdfFilePath))
                    {
                        dataStream.Seek(0, SeekOrigin.Begin);
                        dataStream.CopyTo(fileStream);
                    }
                }

                return _pdfController.GetContentUri(pdfFileName);
            }
        }
    }
}