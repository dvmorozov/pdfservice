using EnterpriseServices.Controllers;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace EnterpriseServices.HtmlToPdf
{
    public class HtmlToPdfByAdobeSdk : HtmlToPdf
    {
        private readonly PdfController _pdfController;

        public HtmlToPdfByAdobeSdk(PdfController pdfController)
        {
            _pdfController = pdfController;
        }

        public string GetUrlToPdf(string url)
        {
            string fileName = RequestConvertingService(url);
            GetFileFromConvertingService(fileName);
            return _pdfController.GetContentUri(fileName);
        }

        /// <summary>
        /// Requests external service for conversion original URL to PDF.
        /// </summary>
        /// <param name="url">Original URL.</param>
        /// <returns>File name.</returns>
        private string RequestConvertingService(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            UriBuilder uriBuilder = new UriBuilder()
            {
                Scheme = "https",
#if DEBUG
                Host = "localhost",
                Port = 44379,
#else
                Host = "adobesdk.azurewebsites.net",
                Port = 443,
#endif
                Path = "pdf/document",
                Query = "url=" + url
            };

            string request = uriBuilder.ToString();
            HttpWebRequest req = WebRequest.CreateHttp(request);
            req.Timeout = 30000;
            req.ContentType = "application/json";
            req.Method = "GET";

            using (WebResponse res = req.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(res.GetResponseStream()))
                {
                    JObject jObject = JObject.Parse(streamReader.ReadToEnd());

                    if (jObject["urlToPdf"].Type == JTokenType.Null)
                    {
                        if (jObject["message"].Type != JTokenType.Null)
                        {
                            //  Propagates an exception.
                            throw new Exception(jObject["message"].ToString());
                        }
                        else
                        {
                            throw new Exception("Converting service had returned empty URL to PDF.");
                        }
                    }

                    return jObject["fileName"].ToString();
                }
            }
        }

        /// <summary>
        /// Copies file from converting service to local folder and deletes it.
        /// </summary>
        /// <param name="fileName">Name of converted file.</param>
        private void GetFileFromConvertingService(string fileName)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            UriBuilder uriBuilder = new UriBuilder()
            {
                Scheme = "https",
#if DEBUG
                Host = "localhost",
                Port = 44379,
#else
                Host = "adobesdk.azurewebsites.net",
                Port = 443,
#endif
                Path = "pdf/file",
                Query = "fileName=" + fileName + "&delete=true"
            };

            string request = uriBuilder.ToString();
            HttpWebRequest req = WebRequest.CreateHttp(request);
            req.Timeout = 30000;
            req.ContentType = "application/pdf";
            req.Method = "GET";

            using (WebResponse res = req.GetResponse())
            {
                using (Stream dataStream = res.GetResponseStream())
                {
                    string pdfFilePath = _pdfController.GetContentPath(fileName);
                    CopyStreamToFile(dataStream, pdfFilePath);
                }
            }
        }

        /// <summary>
        /// Copies data from stream to local file.
        /// </summary>
        /// <param name="dataStream">Data stream.</param>
        /// <param name="filePath">Path to local file.</param>
        private void CopyStreamToFile(Stream dataStream, string filePath)
        {
            if (dataStream != null)
            {
                using (FileStream fileStream = System.IO.File.Create(filePath))
                {
                    dataStream.CopyTo(fileStream);
                }
            }
        }
    }
}