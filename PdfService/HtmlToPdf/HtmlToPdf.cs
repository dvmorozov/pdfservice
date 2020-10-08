namespace EnterpriseServices.HtmlToPdf
{
    interface IHtmlToPdf
    {
        /// <summary>
        /// Convert HTML-page to local PDF-file.
        /// </summary>
        /// <param name="url">Original URL to HTML-page.</param>
        /// <returns>URL to PDF-file.</returns>
        string GetUrlToPdf(string url);
    }
}
