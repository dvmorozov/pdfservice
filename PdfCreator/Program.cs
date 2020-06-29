/*
 * Copyright 2019 Adobe
 * All Rights Reserved.
 *
 * NOTICE: Adobe permits you to use, modify, and distribute this file in 
 * accordance with the terms of the Adobe license agreement accompanying 
 * it. If you have received this file from a source other than Adobe, 
 * then your use, modification, or distribution of it requires the prior 
 * written permission of Adobe.
 */
using System;
using System.IO;
using log4net.Repository;
using log4net;
using log4net.Config;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Reflection;
using Adobe.DocumentCloud.Services;
using Adobe.DocumentCloud.Services.auth;
using Adobe.DocumentCloud.Services.pdfops;
using Adobe.DocumentCloud.Services.io;
using Adobe.DocumentCloud.Services.exception;
using Adobe.DocumentCloud.Services.options.createpdf;
using System.Linq;

namespace PdfCreator
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static string inputFileNameOrUrl;
        private static string outputFileName;
        private static string temporaryFileName;
        private static bool urlIsProcessed;
        private const string usageMessage = "Usage: pdf_creator <path-to-the-HTML-zip-or-URL> <path-to-the-created-pdf>";

        static void Main(string[] args)
        {
            try
            {
                try
                {
                    //  Read names of input and output files.
                    ParseArguments(args);

                    // Configure the logging.
                    ConfigureLogging();

                    // Read HTML, convert and write PDF.
                    ConvertFileToPdf();
                }
                finally
                {
                    CleanUp();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Extracts file names from provided arguments.
        /// </summary>
        /// <param name="args"></param>
        private static void ParseArguments(string[] args)
        {
            if (args.Count() != 2)
            {
                throw new ArgumentException(usageMessage);
            }

            inputFileNameOrUrl = args[0];
            outputFileName = args[1];

            urlIsProcessed = inputFileNameOrUrl.Contains("://");
        }

        /// <summary>
        /// Performs HTTP-request.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string MakeRequest(string url)
        {
            //  Defines code page and convert it to UTF-8.
            var req = WebRequest.Create(url);
            var res = req.GetResponse();

            StreamReader streamReader = new StreamReader(res.GetResponseStream());
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Creates temporary zip-archive with content of page.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string CreateTemporaryFile(string content)
        {
            string tempPath = Path.GetTempPath();
            string tempDirectoryName = tempPath + "/pdf_creator_page";
            Directory.CreateDirectory(tempDirectoryName);

            //  File must be named as "index.html".
            string tempFileName = Path.Combine(tempDirectoryName, "index.html");

            File.WriteAllText(tempFileName, content);
            Console.WriteLine(tempFileName);

            string zipFileName = Path.Combine(tempPath, "pdf_creator_page.zip");
            File.Delete(zipFileName);

            ZipFile.CreateFromDirectory(tempDirectoryName, zipFileName);
            Console.WriteLine(zipFileName);

            File.Delete(tempFileName);
            Directory.Delete(tempDirectoryName, true);

            return zipFileName;
        }

        /// <summary>
        /// Removes temporary files.
        /// </summary>
        private static void CleanUp()
        {
            if (urlIsProcessed)
            {
                File.Delete(temporaryFileName);
            }
        }

        /// <summary>
        /// Prepares source of content.
        /// </summary>
        /// <returns></returns>
        private static FileRef GetSource()
        {
            if (urlIsProcessed)
            {
                var content = MakeRequest(inputFileNameOrUrl);
                temporaryFileName = CreateTemporaryFile(content);
                return FileRef.CreateFromLocalFile(temporaryFileName);
            }
            else
            {
                return FileRef.CreateFromLocalFile(inputFileNameOrUrl);
            }
        }

        /// <summary>
        /// Converts zip-file or page located by provided URL to PDF.
        /// </summary>
        private static void ConvertFileToPdf()
        {
            try
            {
                // Initial setup, create credentials instance.
                Credentials credentials = Credentials.ServiceAccountCredentialsBuilder()
                                .FromFile(Directory.GetCurrentDirectory() + "/dc-services-sdk-credentials.json")
                                .Build();

                // Create an ExecutionContext using credentials and create a new operation instance.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);
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
            }
            catch (ServiceApiException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch (SDKException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch (IOException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch (Exception ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
        }

        /// <summary>
        /// Sets any custom options for the operation.
        /// </summary>
        /// <param name="htmlToPDFOperation">operation instance for which the options are provided.</param>
        private static void SetCustomOptions(CreatePDFOperation htmlToPDFOperation)
        {
            // Define the page layout, in this case an 8 x 11.5 inch page (effectively portrait orientation).
            PageLayout pageLayout = new PageLayout();
            pageLayout.SetPageSize(8, 11.5);

            // Set the desired HTML-to-PDF conversion options.
            CreatePDFOptions htmlToPdfOptions = CreatePDFOptions.HtmlOptionsBuilder()
                    .IncludeHeaderFooter(true)
                    .WithPageLayout(pageLayout)
                    . Build();
            htmlToPDFOperation.SetOptions(htmlToPdfOptions);
        }

        static void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}
