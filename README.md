# AdobeSdkService 

Implements REST API providing access to Adobe Services SDK functionality for conversion HTML-pages into PDF-documents.

## API endpoints 

|Endpoint     |Methods         |Purpose                                                                        |
|-------------|----------------|-------------------------------------------------------------------------------|
|pdf/document |GET             |Accepts URL to original page and returns URL to PDF file.                      |
|pdf/filename |GET             |Accepts URL to original page and returns file name.                            |
|pdf/file     |GET             |Accepts file name and returns content of the file as "application/pdf" content.|
|pdf/file     |DELETE          |Deletes file.

## Building

Open the solution by Microsoft Visual Studio Community 2019 and build __AdobeSdkService__ project.

## Usage

Run application by means of Visual Studio. The application is opened in local browser at https://localhost:44379/pdf.

To get PDF document from given URL use "url" request parameter.

### Examples

To get PDF document from URL use
https://localhost:44379/pdf/document?url=https://auto.mail.ru/article/78150-vintazhnyi_pikap_dodge_v_bezumnoi_raskraske_vystavili_na_prodazhu/

To view generated document use
https://localhost:44379/auto_mail_ru_article_78150-vintazhnyi_pikap_dodge_v_bezumnoi_raskraske_vystavili_na_prodazhu.pdf

To get file name from URL use
https://localhost:44379/pdf/filename?url=https://auto.mail.ru/article/78150-vintazhnyi_pikap_dodge_v_bezumnoi_raskraske_vystavili_na_prodazhu/

## Deployment

The API has been deployed into Azure infrastructure.
You can try it at https://adobesdk.azurewebsites.net/

### Examples

https://adobesdk.azurewebsites.net/pdf/document/?url=https://hi-tech.mail.ru/review/ne_perevodi/
https://adobesdk.azurewebsites.net/pdf/filename/?url=https://hi-tech.mail.ru/review/ne_perevodi/

# HTMLCleanupDLL

Implements extracting of informative text from HTML-pages (cleaning up from advertising banners and so on).
Converts cleaned content into PDF by means of iText library.

# PdfCreator

Implements CLI application converting HTML-pages into PDF-documents by means of Adobe Services SDK.

## Building

Open the solution by Microsoft Visual Studio Community 2019 and build PdfCreator project.
Publish the project by using FolderProfile.
Put files private.key and dc-services-sdk-credentials.json into the directory containing
published binaries ("PdfCreator\bin\Release\netcoreapp2.1\publish").

## Usage

Open the directory containing published binaries in terminal and execute

> pdf_creator &lt;path-to-HTML-zip-or-URL&gt; &lt;path-to-created-pdf&gt;

You could use URL or path to ZIP file containing compressed HTML page as the first parameter.

### Examples

> pdf_creator "https://www.adobe.io/apis/documentcloud/dcsdk/pdf-embed.html" pdf-embed.pdf

> pdf_creator example_page.zip example_page.pdf

# HTMLToPDFConvertingDLL

Contains class providing access to Adobe SDK functionality common for AdobeSdkService and PdfCreator projects.

# PdfService

Implements Web UI for conversion HTML-pages into PDF-documents.

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
