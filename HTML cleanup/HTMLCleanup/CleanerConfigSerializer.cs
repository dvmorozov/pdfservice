
using System.IO;
using System.Text;
using System.Xml.Serialization;
using HtmlCleanup.Config;

namespace HtmlCleanup
{
    /// <summary>
    /// Читает конфигурацию из XML-файла и создает объекты.
    /// Требуется для того, чтобы можно было свободно 
    /// регенерировать класс конфигурации по XSD-файлу.
    /// </summary>
    class CleanerConfigSerializer : ICleanerConfigSerializer
    {
        /// <summary>
        /// Восстанавливает конфигурацию объектов по данным из файла.
        /// </summary>
        /// <param name="fileName">Имя файла для чтения конфигурации.</param>
        /// <param name="chain">Первый объект в цепочке обработчиков.</param>
        public void Deserialize(string fileName, BaseHtmlCleaner.TextProcessor chain)
        {
            var config = new HTMLCleanupConfig();
            using (var reader = new StreamReader(fileName))
            {
                var serializer = new XmlSerializer(typeof(HTMLCleanupConfig));
                config = (HTMLCleanupConfig)serializer.Deserialize(reader);
            }

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

                if (chain.GetType() == typeof(BaseHtmlCleaner.InnerTagRemover))
                {
                    ((BaseHtmlCleaner.InnerTagRemover)chain).Skipped = config.InnerTagRemoverConfig.Skipped;
                    ((BaseHtmlCleaner.InnerTagRemover)chain).Tags.Clear();
                    foreach (var t in config.InnerTagRemoverConfig.Tags)
                    {
                        ((BaseHtmlCleaner.InnerTagRemover)chain).Tags.Add(new BaseHtmlCleaner.TagToRemove(t.StartTagWithoutBracket, t.EndTag));
                    }
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.TagWithTextRemover))
                {
                    ((BaseHtmlCleaner.TagWithTextRemover)chain).Skipped = config.TagWithTextRemoverConfig.Skipped;
                    ((BaseHtmlCleaner.TagWithTextRemover)chain).Tags.Clear();
                    foreach (var t in config.TagWithTextRemoverConfig.Tags)
                    {
                        ((BaseHtmlCleaner.TagWithTextRemover)chain).Tags.Add(new BaseHtmlCleaner.TagToRemove(t.StartTagWithoutBracket, t.EndTag));
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
        /// Сканирует цепочку обработчиков и копирует их данные.
        /// Затем сериализует их в XML-файл.
        /// </summary>
        /// <param name="fileName">Имя файла для сохранения конфигурации.</param>
        /// <param name="chain">Первый объект в цепочке обработчиков.</param>
        public void Serialize(string fileName, BaseHtmlCleaner.TextProcessor chain)
        {
            var config = new HTMLCleanupConfig();
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

                if (chain.GetType() == typeof(BaseHtmlCleaner.InnerTagRemover))
                {
                    config.InnerTagRemoverConfig = new InnerTagRemoverType()
                    {
                        Skipped = ((BaseHtmlCleaner.InnerTagRemover)chain).Skipped,
                        Tags = new TagToRemoveType[((BaseHtmlCleaner.InnerTagRemover) chain).Tags.Count]
                    };

                    for (var i = 0; i < ((BaseHtmlCleaner.InnerTagRemover) chain).Tags.Count; i++)
                    {
                        config.InnerTagRemoverConfig.Tags[i] = new TagToRemoveType
                        {
                            StartTagWithoutBracket = ((BaseHtmlCleaner.InnerTagRemover) chain).Tags[i].StartTag, 
                            EndTag = ((BaseHtmlCleaner.InnerTagRemover) chain).Tags[i].EndTag
                        };
                    }
                }

                if (chain.GetType() == typeof(BaseHtmlCleaner.TagWithTextRemover))
                {
                    config.TagWithTextRemoverConfig = new TagWithTextRemoverType()
                    {
                        Skipped = ((BaseHtmlCleaner.TagWithTextRemover)chain).Skipped,
                        Tags = new TagToRemoveType[((BaseHtmlCleaner.TagWithTextRemover) chain).Tags.Count]
                    };

                    for (var i = 0; i < ((BaseHtmlCleaner.TagWithTextRemover) chain).Tags.Count; i++)
                    {
                        config.TagWithTextRemoverConfig.Tags[i] = new TagToRemoveType()
                        {
                            StartTagWithoutBracket = ((BaseHtmlCleaner.TagWithTextRemover) chain).Tags[i].StartTag,
                            EndTag = ((BaseHtmlCleaner.TagWithTextRemover) chain).Tags[i].EndTag
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

            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof(HTMLCleanupConfig));
                serializer.Serialize(writer, config);
            }
        }
    }
}