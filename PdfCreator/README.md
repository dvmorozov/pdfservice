# Adobe Services SDK CLI application

## Building

Open the solution by Microsoft Visual Studio Community 2019 and build PdfCreator project.
Publish the project by using FolderProfile.

## Usage

Open the directory containing published binaries ("PdfCreator\bin\Release\netcoreapp2.1\publish") in terminal and execute

> pdf_creator &lt;path-to-HTML-zip-or-URL&gt; &lt;path-to-created-pdf&gt;

You could use URL or path to ZIP file containing compressed HTML page as the first parameter.

Examples

> pdf_creator "https://www.adobe.io/apis/documentcloud/dcsdk/pdf-embed.html" pdf-embed.pdf

> pdf_creator example_page.zip example_page.pdf
