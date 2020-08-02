# Adobe Services SDK converting API

Two API endpoints "document" and "filename" are available under "pdf" route. Only GET method has been implemented for endpoints.
The first endpoint is responsible for conversion URLs into PDF documents, the second endpoint provides way of converting URL into file name.

## Building

Open the solution by Microsoft Visual Studio Community 2019 and build AdobeSdkService project.

## Usage

Run application by means of Visual Studio. The application will be opened in local browser at https://localhost:44379/pdf.

To get PDF document from given URL use "url" request parameter.

## Examples

To get PDF document from URL use
https://localhost:44379/pdf/document?url=https://auto.mail.ru/article/78150-vintazhnyi_pikap_dodge_v_bezumnoi_raskraske_vystavili_na_prodazhu/

To view generated document use
https://localhost:44379/auto_mail_ru_article_78150-vintazhnyi_pikap_dodge_v_bezumnoi_raskraske_vystavili_na_prodazhu.pdf

To get file name from URL use
https://localhost:44379/pdf/filename?url=https://auto.mail.ru/article/78150-vintazhnyi_pikap_dodge_v_bezumnoi_raskraske_vystavili_na_prodazhu/

## Deployment

The service is ready for deployment into Azure.
Actually now it is available at https://adobesdk.azurewebsites.net/

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

## Examples

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
