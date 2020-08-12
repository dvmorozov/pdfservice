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
using System.Linq;

namespace PdfCreator
{
    class Program
    {
        private const string usageMessage = "Usage: pdf_creator <path-to-the-HTML-zip-or-URL> <path-to-the-created-pdf>";

        static void Main(string[] args)
        {
            try
            {
                //  Read names of input and output files.
                HtmlToPdfConverter htmlToPdfConverter = ParseArguments(args);
                try
                {
                    // Configure the logging.
                    htmlToPdfConverter.ConfigureLogging();

                    // Read HTML, convert and write PDF.
                    htmlToPdfConverter.ConvertFileToPdf();
                }
                finally
                {
                    htmlToPdfConverter.CleanUp();
                }

                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Environment.ExitCode = 1;
            }
        }

        /// <summary>
        /// Extracts file names from provided arguments.
        /// </summary>
        /// <param name="args"></param>
        private static HtmlToPdfConverter ParseArguments(string[] args)
        {
            if (args.Count() != 2)
            {
                throw new ArgumentException(usageMessage);
            }

            string inputFileNameOrUrl = args[0].Trim('"');
            string outputFileName = args[1].Trim('"');

            Console.WriteLine("Input file = " + inputFileNameOrUrl);
            Console.WriteLine("Output file = " + outputFileName);

            return new HtmlToPdfConverter(inputFileNameOrUrl, outputFileName);
        }
    }
}
