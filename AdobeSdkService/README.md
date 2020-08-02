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

## Examples

https://adobesdk.azurewebsites.net/pdf/document/?url=https://hi-tech.mail.ru/review/ne_perevodi/
https://adobesdk.azurewebsites.net/pdf/filename/?url=https://hi-tech.mail.ru/review/ne_perevodi/