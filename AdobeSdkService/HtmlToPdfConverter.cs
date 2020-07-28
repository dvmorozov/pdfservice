﻿using System;
using System.IO;
using log4net.Repository;
using log4net;
using log4net.Config;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using Adobe.DocumentCloud.Services;
using Adobe.DocumentCloud.Services.auth;
using Adobe.DocumentCloud.Services.pdfops;
using Adobe.DocumentCloud.Services.io;
using Adobe.DocumentCloud.Services.exception;
using Adobe.DocumentCloud.Services.options.createpdf;
using System.Threading;

namespace AdobeSdkService
{
    class HtmlToPdfConverter
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
        /// <returns>Content stream.</returns>
        private Stream MakeRequest(string url)
        {
            //  Defines code page and convert it to UTF-8.
            var req = WebRequest.Create(url);
            var res = req.GetResponse();

            return res.GetResponseStream();
        }

        /// <summary>
        /// Creates temporary zip-archive with content of page.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Filename of zip-archive.</returns>
        private string CreateTemporaryZipFile(Stream content)
        {
            string tempPath = Path.GetTempPath();
            string tempDirectoryName = tempPath + "/pdf_creator_page";
            Directory.CreateDirectory(tempDirectoryName);

            //  File must be named as "index.html".
            string tempFileName = Path.Combine(tempDirectoryName, "index.html");
            using (var fileStream = File.Create(tempFileName))
            {
                content.CopyTo(fileStream);
            }

            string zipFileName = Path.Combine(tempPath, Path.GetRandomFileName() + ".zip");
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
            if (urlIsProcessed)
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
                var content = MakeRequest(inputFileNameOrUrl);
                temporaryZipFileName = CreateTemporaryZipFile(content);
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
    }
}
