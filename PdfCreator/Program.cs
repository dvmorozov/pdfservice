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
using System.IO;
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
        private const string usageMessage = "Usage: pdf_creator <path-to-the-HTML-zip-or-URL> <path-to-the-created-pdf>";

        static void Main(string[] args)
        {
            try
            {
                //  Reads names of input and output files.
                ParseArguments(args);

                // Configure the logging.
                ConfigureLogging();

                // Read HTML, convert and write PDF.
                ConvertHtmlToPdf();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void ParseArguments(string[] args)
        {
            if (args.Count() != 2)
            {
                throw new ArgumentException(usageMessage);
            }

            inputFileNameOrUrl = args[0];
            outputFileName = args[1];
        }

        private static string MakeRequest(string url)
        {
            //  Defines code page and convert it to UTF-8.
            var req = WebRequest.Create(url);
            var res = req.GetResponse();
            //  Searches for code page name.
            string charset = String.Empty;
            if (res.ContentType.IndexOf("1251", 0, StringComparison.OrdinalIgnoreCase) != -1) charset = "windows-1251";
            else
                if (res.ContentType.IndexOf("utf-8", 0, StringComparison.OrdinalIgnoreCase) != -1) charset = "utf-8";

            StreamReader streamReader = null;
            string text = String.Empty;

            //  If charset wasn't recognized UTF-8 is used by default.
            if (charset == "utf-8" || string.IsNullOrEmpty(charset))
            {
                streamReader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                text = streamReader.ReadToEnd();
            }

            if (charset == "windows-1251")
            {
                streamReader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(1251));
                text = streamReader.ReadToEnd();
                //  Convert to UTF-8.
                var bIn = Encoding.GetEncoding(1251).GetBytes(text);
                var bOut = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.UTF8, bIn);
                text = Encoding.UTF8.GetString(bOut);
            }

            return text;
        }

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
            ZipFile.CreateFromDirectory(tempDirectoryName, zipFileName);
            Console.WriteLine(zipFileName);

            File.Delete(tempFileName);
            Directory.Delete(tempDirectoryName);

            return zipFileName;
        }

        private static void ConvertHtmlToPdf()
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

                FileRef source;
                bool urlIsProcessed = inputFileNameOrUrl.Contains("://");
                string temporaryFileName = "";

                if (urlIsProcessed)
                {
                    var content = MakeRequest(inputFileNameOrUrl);
                    temporaryFileName = CreateTemporaryFile(content);
                    source = FileRef.CreateFromLocalFile(temporaryFileName);
                }
                else
                {
                    source = FileRef.CreateFromLocalFile(inputFileNameOrUrl);
                }

                // Set operation input from a source file.
                htmlToPDFOperation.SetInput(source);

                // Provide any custom configuration options for the operation.
                SetCustomOptions(htmlToPDFOperation);

                // Execute the operation.
                FileRef result = htmlToPDFOperation.Execute(executionContext);

                // Save the result to the specified location.
                result.SaveAs(outputFileName);

                if (urlIsProcessed)
                {
                    File.Delete(temporaryFileName);
                }
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
