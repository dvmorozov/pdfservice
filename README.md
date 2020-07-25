# PdfCreator - Adobe Services SDK CLI application

## Building

Open the solution by Microsoft Visual Studio Community 2019 and build PdfCreator project.
Publish the project by using FolderProfile.
Put files private.key and dc-services-sdk-credentials.json into the directory containing
published binaries ("PdfCreator\bin\Release\netcoreapp2.1\publish").

## Usage

Open the directory containing published binaries in terminal and execute

> pdf_creator &lt;path-to-HTML-zip-or-URL&gt; &lt;path-to-created-pdf&gt;

You could use URL or path to ZIP file containing compressed HTML page as the first parameter.

Examples

> pdf_creator "https://www.adobe.io/apis/documentcloud/dcsdk/pdf-embed.html" pdf-embed.pdf

> pdf_creator example_page.zip example_page.pdf

# PdfService - Adobe View SDK demo application

## Building

Open the solution by Microsoft Visual Studio Community 2019 and build PdfService project.

## Usage

Put files private.key and dc-services-sdk-credentials.json into the directory containing
PdfCreator binaries ("PdfService\PdfCreator").
Run application by means of Visual Studio. The application will be opened in local browser
at https://localhost:44301/.
Paste URL to page into the input field and press "Get PDF" button.

## Video
https://youtu.be/n40oFOq6I1o
