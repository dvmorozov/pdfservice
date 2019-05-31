
using System.IO;
using System.Text;
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
                {
                    ((BaseHtmlCleaner.ParagraphExtractor)chain).Skipped = config.ParagraphExtractorConfig.Skipped;
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.SpecialHTMLRemover))
                {
                    ((BaseHtmlCleaner.SpecialHTMLRemover)chain).Skipped = config.SpecialHTMLRemoverConfig.Skipped;
                    ((BaseHtmlCleaner.SpecialHTMLRemover)chain).SpecialHTML.Clear();
                    foreach (var t in config.SpecialHTMLRemoverConfig.SpecialHTML)
                    {
                        ((BaseHtmlCleaner.SpecialHTMLRemover)chain).SpecialHTML.Add(new BaseHtmlCleaner.SpecialHTMLSymbol(t.SpecialHTML, t.Replacement));
                    }
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.InnerTextProcessor))
                {
                    ((BaseHtmlCleaner.InnerTextProcessor)chain).Skipped = config.InnerTagRemoverConfig.Skipped;
                    ((BaseHtmlCleaner.InnerTextProcessor)chain).Tags.Clear();
                    foreach (var t in config.InnerTagRemoverConfig.Tags)
                    {
                        ((BaseHtmlCleaner.InnerTextProcessor)chain).Tags.Add(new BaseHtmlCleaner.Tag(t.StartTagWithoutBracket, t.EndTag));
                    }
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.TagRemover))
                {
                    ((BaseHtmlCleaner.TagRemover)chain).Skipped = config.TagWithTextRemoverConfig.Skipped;
                    ((BaseHtmlCleaner.TagRemover)chain).Tags.Clear();
                    foreach (var t in config.TagWithTextRemoverConfig.Tags)
                    {
                        ((BaseHtmlCleaner.TagRemover)chain).Tags.Add(new BaseHtmlCleaner.Tag(t.StartTagWithoutBracket, t.EndTag));
                    }
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.UrlFormatter))
                {
                    ((BaseHtmlCleaner.UrlFormatter)chain).Skipped = config.URLFormatterConfig.Skipped;
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.TextFormatter))
                {
                    ((BaseHtmlCleaner.TextFormatter)chain).Skipped = config.TextFormatterConfig.Skipped;
                    var b = new byte[config.TextFormatterConfig.Delimiters.Length];
                    for (var i = 0; i < config.TextFormatterConfig.Delimiters.Length; i++)
                    {
                        b[i] = config.TextFormatterConfig.Delimiters[i].SymbolCode;
                    }

                    ((BaseHtmlCleaner.TextFormatter)chain).Delimiters = Encoding.ASCII.GetChars(b);
                }

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
                {
                    config.ParagraphExtractorConfig = new ParagraphExtractorType
                    {
                        Skipped = ((BaseHtmlCleaner.ParagraphExtractor)chain).Skipped
                    };
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.SpecialHTMLRemover))
                {
                    config.SpecialHTMLRemoverConfig = new SpecialHTMLRemoverType
                    {
                        Skipped = ((BaseHtmlCleaner.SpecialHTMLRemover)chain).Skipped,
                        SpecialHTML = new SpecialHTMLSymbolType[((BaseHtmlCleaner.SpecialHTMLRemover) chain).SpecialHTML.Count]
                    };

                    for (var i = 0; i < ((BaseHtmlCleaner.SpecialHTMLRemover) chain).SpecialHTML.Count; i++)
                    {
                        config.SpecialHTMLRemoverConfig.SpecialHTML[i] = new SpecialHTMLSymbolType
                        {
                            SpecialHTML = ((BaseHtmlCleaner.SpecialHTMLRemover) chain).SpecialHTML[i].SpecialHTML, 
                            Replacement = ((BaseHtmlCleaner.SpecialHTMLRemover) chain).SpecialHTML[i].Replacement
                        };
                    }
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.InnerTextProcessor))
                {
                    config.InnerTagRemoverConfig = new InnerTagRemoverType()
                    {
                        Skipped = ((BaseHtmlCleaner.InnerTextProcessor)chain).Skipped,
                        Tags = new TagToRemoveType[((BaseHtmlCleaner.InnerTextProcessor) chain).Tags.Count]
                    };

                    for (var i = 0; i < ((BaseHtmlCleaner.InnerTextProcessor) chain).Tags.Count; i++)
                    {
                        config.InnerTagRemoverConfig.Tags[i] = new TagToRemoveType
                        {
                            StartTagWithoutBracket = ((BaseHtmlCleaner.InnerTextProcessor) chain).Tags[i].StartTag, 
                            EndTag = ((BaseHtmlCleaner.InnerTextProcessor) chain).Tags[i].EndTag
                        };
                    }
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.TagRemover))
                {
                    config.TagWithTextRemoverConfig = new TagWithTextRemoverType()
                    {
                        Skipped = ((BaseHtmlCleaner.TagRemover)chain).Skipped,
                        Tags = new TagToRemoveType[((BaseHtmlCleaner.TagRemover) chain).Tags.Count]
                    };

                    for (var i = 0; i < ((BaseHtmlCleaner.TagRemover) chain).Tags.Count; i++)
                    {
                        config.TagWithTextRemoverConfig.Tags[i] = new TagToRemoveType()
                        {
                            StartTagWithoutBracket = ((BaseHtmlCleaner.TagRemover) chain).Tags[i].StartTag,
                            EndTag = ((BaseHtmlCleaner.TagRemover) chain).Tags[i].EndTag
                        };
                    }
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.UrlFormatter))
                {
                    config.URLFormatterConfig = new URLFormatterType
                    {
                        Skipped = ((BaseHtmlCleaner.UrlFormatter)chain).Skipped
                    };
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.TextFormatter))
                {
                    config.TextFormatterConfig = new TextFormatterType()
                    {
                        Skipped = ((BaseHtmlCleaner.TextFormatter)chain).Skipped,
                        Delimiters = new DelimiterSymbolType[((BaseHtmlCleaner.TextFormatter) chain).Delimiters.Length]
                    };

                    for (var i = 0; i < ((BaseHtmlCleaner.TextFormatter) chain).Delimiters.Length; i++)
                    {
                        var b = Encoding.ASCII.GetBytes(((BaseHtmlCleaner.TextFormatter) chain).Delimiters);
                        config.TextFormatterConfig.Delimiters[i] = new DelimiterSymbolType()
                        {
                            SymbolCode = b[i]
                        };
                    }
                }

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