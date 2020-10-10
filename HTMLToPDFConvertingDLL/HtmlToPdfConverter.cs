/*
 “Commons Clause” License Condition v1.0
The Software is provided to you by the Licensor under the License, as defined below, subject to the following condition.
Without limiting other conditions in the License, the grant of rights under the License will not include, and the License
does not grant to you, right to Sell the Software. For purposes of the foregoing, “Sell” means practicing any or all of
the rights granted to you under the License to provide to third parties, for a fee or other consideration (including
without limitation fees for hosting or consulting/ support services related to the Software), a product or service whose
value derives, entirely or substantially, from the functionality of the Software.  Any license notice or attribution
required by the License must also include this Commons Cause License Condition notice.

Software: HTMLToPDFConvertingDLL

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
using log4net.Repository;
using log4net;
using log4net.Config;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using Adobe.DocumentCloud.Services.auth;
using Adobe.DocumentCloud.Services.pdfops;
using Adobe.DocumentCloud.Services.io;
using Adobe.DocumentCloud.Services.exception;
using Adobe.DocumentCloud.Services.options.createpdf;
using System.Threading.Tasks;

namespace HTMLToPDFConvertingDLL
{
    public class HtmlToPdfConverter
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HtmlToPdfConverter));
        private readonly string inputFileNameOrUrl;
        private readonly string outputFileName;
        private string temporaryZipFileName;
        private readonly bool urlIsProcessed;

        public HtmlToPdfConverter(string _inputFileNameOrUrl, string _outputFileName)
        {
            inputFileNameOrUrl = _inputFileNameOrUrl;
            outputFileName = _outputFileName;

            urlIsProcessed = inputFileNameOrUrl.Contains("://");
        }

        /// <summary>
        /// Performs HTTP-request.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Response object.</returns>
        private WebResponse MakeRequest(string url)
        {
            //  Defines code page and convert it to UTF-8.
            WebRequest req = WebRequest.Create(url);
            return req.GetResponse();
        }

        /// <summary>
        /// Creates temporary zip-archive with content of page.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Filename of zip-archive.</returns>
        private string CreateTemporaryZipFile(Stream content)
        {
            string tempPath = Path.GetTempPath();
            string tempDirectoryName = Path.Combine(tempPath, ReplaceInvalidCharacters(Path.GetRandomFileName()));
            _ = Directory.CreateDirectory(tempDirectoryName);

            //  File must be named as "index.html".
            string tempFileName = Path.Combine(tempDirectoryName, "index.html");
            using (FileStream fileStream = File.Create(tempFileName))
            {
                content.CopyTo(fileStream);
            }

            string zipFileName = Path.Combine(tempPath, ReplaceInvalidCharacters(Path.GetRandomFileName()) + ".zip");
            ZipFile.CreateFromDirectory(tempDirectoryName, zipFileName);

            File.Delete(tempFileName);
            Directory.Delete(tempDirectoryName, true);

            return zipFileName;
        }

        /// <summary>
        /// Removes temporary files.
        /// </summary>
        public void CleanUp()
        {
            if (urlIsProcessed && temporaryZipFileName != null)
            {
                File.Delete(temporaryZipFileName);
            }
        }

        /// <summary>
        /// Prepares source of content.
        /// </summary>
        /// <returns>File for conversion to PDF.</returns>
        private FileRef GetSource()
        {
            if (urlIsProcessed)
            {
                using (WebResponse response = MakeRequest(inputFileNameOrUrl))
                {
                    temporaryZipFileName = CreateTemporaryZipFile(response.GetResponseStream());
                }
                return FileRef.CreateFromLocalFile(temporaryZipFileName);
            }
            else
            {
                return FileRef.CreateFromLocalFile(inputFileNameOrUrl);
            }
        }

        /// <summary>
        /// Converts zip-file or page located by provided URL to PDF.
        /// </summary>
        public void ConvertFileToPdf()
        {
            try
            {
                // Initial setup, create credentials instance.
                Credentials credentials = Credentials.ServiceAccountCredentialsBuilder()
                                .FromFile(Directory.GetCurrentDirectory() + "/dc-services-sdk-credentials.json")
                                .Build();

                // Create an ExecutionContext using credentials and create a new operation instance.
                Adobe.DocumentCloud.Services.ExecutionContext executionContext = Adobe.DocumentCloud.Services.ExecutionContext.Create(credentials);
                CreatePDFOperation htmlToPDFOperation = CreatePDFOperation.CreateNew();

                FileRef source = GetSource();

                // Set operation input from a source file.
                htmlToPDFOperation.SetInput(source);

                // Provide any custom configuration options for the operation.
                SetCustomOptions(htmlToPDFOperation);

                // Execute the operation.
                FileRef result = htmlToPDFOperation.Execute(executionContext);

                // Save the result to the specified location.
                File.Delete(outputFileName);
                result.SaveAs(outputFileName);
            }
            catch (ServiceUsageException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
                throw;
            }
            catch (ServiceApiException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
                throw;
            }
            catch (SDKException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
                throw;
            }
            catch (IOException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error("Exception encountered while executing operation", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets any custom options for the operation.
        /// </summary>
        /// <param name="htmlToPDFOperation">operation instance for which the options are provided.</param>
        private void SetCustomOptions(CreatePDFOperation htmlToPDFOperation)
        {
            // Define the page layout, in this case an 8 x 11.5 inch page (effectively portrait orientation).
            PageLayout pageLayout = new PageLayout();
            pageLayout.SetPageSize(8, 11.5);

            // Set the desired HTML-to-PDF conversion options.
            CreatePDFOptions htmlToPdfOptions = CreatePDFOptions.HtmlOptionsBuilder()
                    .IncludeHeaderFooter(true)
                    .WithPageLayout(pageLayout)
                    .Build();
            htmlToPDFOperation.SetOptions(htmlToPdfOptions);
        }

        public void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        /// <summary>
        /// Replaced inadmissible characters. Should necessarily replace '.' for proper naming of ZIP-files.
        /// </summary>
        /// <param name="fileName">Original file name.</param>
        /// <returns>Transformed file name.</returns>
        public static string ReplaceInvalidCharacters(string fileName)
        {
            char[] replacedCharacters = { '<', '>', ':', '"', '/', '\\', '|', '?', '*', '&', '#', '=', '.', '-' };

            foreach (char character in replacedCharacters)
            {
                fileName = fileName.Replace(character, '_');
            }

            return fileName.Trim('_');
        }
    }
}
