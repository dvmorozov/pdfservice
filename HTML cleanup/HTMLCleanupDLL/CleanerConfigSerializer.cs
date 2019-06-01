using System.IO;
using System.Xml.Serialization;
using HtmlCleanup.Config;

namespace HtmlCleanup
{
    /// <summary>
    /// Reads configuration from XML-file and creates objects.
    /// Decouples serialization/deserialization from definitions
    /// of data classes automatically generated from XSD-description.
    /// </summary>
    class CleanerConfigSerializer : ICleanerConfigSerializer
    {
        /// <summary>
        /// Reads configuration objects from file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <param name="chain">The first member of processing chain.</param>
        public void Deserialize(string fileName, BaseHtmlCleaner.TextProcessor chain)
        {
            //  Reads settings from file.
            var config = new HTMLCleanupConfig();
            using (var reader = new StreamReader(fileName))
            {
                var serializer = new XmlSerializer(typeof(HTMLCleanupConfig));
                config = (HTMLCleanupConfig)serializer.Deserialize(reader);
            }
            //  Updates objects in the chain.
            while (chain != null)
            {
                if (chain.GetType() == typeof(BaseHtmlCleaner.ParagraphExtractor))
                    ((BaseHtmlCleaner.ParagraphExtractor)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.SpecialHtmlRemover))
                    ((BaseHtmlCleaner.SpecialHtmlRemover)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.InnerTextProcessor))
                    ((BaseHtmlCleaner.InnerTextProcessor)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.TagRemover))
                    ((BaseHtmlCleaner.TagRemover)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.UrlFormatter))
                    ((BaseHtmlCleaner.UrlFormatter)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.TextFormatter))
                    ((BaseHtmlCleaner.TextFormatter)chain).LoadSettings(config);

                chain = chain.Next;
            }
        }

        /// <summary>
        /// Scans processing chain and copies object data into file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <param name="chain">The first member of processing chain.</param>
        public void Serialize(string fileName, BaseHtmlCleaner.TextProcessor chain)
        {
            var config = new HTMLCleanupConfig();
            //  Fills the settings container.
            while (chain != null)
            {
                if (chain.GetType() == typeof(BaseHtmlCleaner.ParagraphExtractor))
                    ((BaseHtmlCleaner.ParagraphExtractor)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.SpecialHtmlRemover))
                    ((BaseHtmlCleaner.SpecialHtmlRemover)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.InnerTextProcessor))
                    ((BaseHtmlCleaner.InnerTextProcessor)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.TagRemover))
                    ((BaseHtmlCleaner.TagRemover)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.UrlFormatter))
                    ((BaseHtmlCleaner.UrlFormatter)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.TextFormatter))
                    ((BaseHtmlCleaner.TextFormatter)chain).SaveSettings(config);

                chain = chain.Next;
            }
            //  Writes data to file.
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof(HTMLCleanupConfig));
                serializer.Serialize(writer, config);
            }
        }
    }
}