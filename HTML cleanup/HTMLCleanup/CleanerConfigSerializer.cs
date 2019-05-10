
using System.IO;
using System.Text;
using System.Xml.Serialization;
using HTMLCleanup.Config;

namespace HTMLCleanup
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
        public void Deserialize(string fileName, BaseHTMLCleaner.TextProcessor chain)
        {
            var config = new HTMLCleanupConfig();
            using (var reader = new StreamReader(fileName))
            {
                var serializer = new XmlSerializer(typeof(HTMLCleanupConfig));
                config = (HTMLCleanupConfig)serializer.Deserialize(reader);
            }

            while (chain != null)
            {
                if (chain.GetType() == typeof(BaseHTMLCleaner.ParagraphExtractor))
                {
                    ((BaseHTMLCleaner.ParagraphExtractor)chain).Skipped = config.ParagraphExtractorConfig.Skipped;
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.SpecialHTMLRemover))
                {
                    ((BaseHTMLCleaner.SpecialHTMLRemover)chain).Skipped = config.SpecialHTMLRemoverConfig.Skipped;
                    ((BaseHTMLCleaner.SpecialHTMLRemover)chain).SpecialHTML.Clear();
                    foreach (var t in config.SpecialHTMLRemoverConfig.SpecialHTML)
                    {
                        ((BaseHTMLCleaner.SpecialHTMLRemover)chain).SpecialHTML.Add(new BaseHTMLCleaner.SpecialHTMLSymbol(t.SpecialHTML, t.Replacement));
                    }
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.InnerTagRemover))
                {
                    ((BaseHTMLCleaner.InnerTagRemover)chain).Skipped = config.InnerTagRemoverConfig.Skipped;
                    ((BaseHTMLCleaner.InnerTagRemover)chain).Tags.Clear();
                    foreach (var t in config.InnerTagRemoverConfig.Tags)
                    {
                        ((BaseHTMLCleaner.InnerTagRemover)chain).Tags.Add(new BaseHTMLCleaner.TagToRemove(t.StartTagWithoutBracket, t.EndTag));
                    }
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.TagWithTextRemover))
                {
                    ((BaseHTMLCleaner.TagWithTextRemover)chain).Skipped = config.TagWithTextRemoverConfig.Skipped;
                    ((BaseHTMLCleaner.TagWithTextRemover)chain).Tags.Clear();
                    foreach (var t in config.TagWithTextRemoverConfig.Tags)
                    {
                        ((BaseHTMLCleaner.TagWithTextRemover)chain).Tags.Add(new BaseHTMLCleaner.TagToRemove(t.StartTagWithoutBracket, t.EndTag));
                    }
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.URLFormatter))
                {
                    ((BaseHTMLCleaner.URLFormatter)chain).Skipped = config.URLFormatterConfig.Skipped;
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.TextFormatter))
                {
                    ((BaseHTMLCleaner.TextFormatter)chain).Skipped = config.TextFormatterConfig.Skipped;
                    var b = new byte[config.TextFormatterConfig.Delimiters.Length];
                    for (var i = 0; i < config.TextFormatterConfig.Delimiters.Length; i++)
                    {
                        b[i] = config.TextFormatterConfig.Delimiters[i].SymbolCode;
                    }

                    ((BaseHTMLCleaner.TextFormatter)chain).Delimiters = Encoding.ASCII.GetChars(b);
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
        public void Serialize(string fileName, BaseHTMLCleaner.TextProcessor chain)
        {
            var config = new HTMLCleanupConfig();
            while (chain != null)
            {
                if (chain.GetType() == typeof(BaseHTMLCleaner.ParagraphExtractor))
                {
                    config.ParagraphExtractorConfig = new ParagraphExtractorType
                    {
                        Skipped = ((BaseHTMLCleaner.ParagraphExtractor)chain).Skipped
                    };
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.SpecialHTMLRemover))
                {
                    config.SpecialHTMLRemoverConfig = new SpecialHTMLRemoverType
                    {
                        Skipped = ((BaseHTMLCleaner.SpecialHTMLRemover)chain).Skipped,
                        SpecialHTML = new SpecialHTMLSymbolType[((BaseHTMLCleaner.SpecialHTMLRemover) chain).SpecialHTML.Count]
                    };

                    for (var i = 0; i < ((BaseHTMLCleaner.SpecialHTMLRemover) chain).SpecialHTML.Count; i++)
                    {
                        config.SpecialHTMLRemoverConfig.SpecialHTML[i] = new SpecialHTMLSymbolType
                        {
                            SpecialHTML = ((BaseHTMLCleaner.SpecialHTMLRemover) chain).SpecialHTML[i].SpecialHTML, 
                            Replacement = ((BaseHTMLCleaner.SpecialHTMLRemover) chain).SpecialHTML[i].Replacement
                        };
                    }
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.InnerTagRemover))
                {
                    config.InnerTagRemoverConfig = new InnerTagRemoverType()
                    {
                        Skipped = ((BaseHTMLCleaner.InnerTagRemover)chain).Skipped,
                        Tags = new TagToRemoveType[((BaseHTMLCleaner.InnerTagRemover) chain).Tags.Count]
                    };

                    for (var i = 0; i < ((BaseHTMLCleaner.InnerTagRemover) chain).Tags.Count; i++)
                    {
                        config.InnerTagRemoverConfig.Tags[i] = new TagToRemoveType
                        {
                            StartTagWithoutBracket = ((BaseHTMLCleaner.InnerTagRemover) chain).Tags[i].StartTag, 
                            EndTag = ((BaseHTMLCleaner.InnerTagRemover) chain).Tags[i].EndTag
                        };
                    }
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.TagWithTextRemover))
                {
                    config.TagWithTextRemoverConfig = new TagWithTextRemoverType()
                    {
                        Skipped = ((BaseHTMLCleaner.TagWithTextRemover)chain).Skipped,
                        Tags = new TagToRemoveType[((BaseHTMLCleaner.TagWithTextRemover) chain).Tags.Count]
                    };

                    for (var i = 0; i < ((BaseHTMLCleaner.TagWithTextRemover) chain).Tags.Count; i++)
                    {
                        config.TagWithTextRemoverConfig.Tags[i] = new TagToRemoveType()
                        {
                            StartTagWithoutBracket = ((BaseHTMLCleaner.TagWithTextRemover) chain).Tags[i].StartTag,
                            EndTag = ((BaseHTMLCleaner.TagWithTextRemover) chain).Tags[i].EndTag
                        };
                    }
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.URLFormatter))
                {
                    config.URLFormatterConfig = new URLFormatterType
                    {
                        Skipped = ((BaseHTMLCleaner.URLFormatter)chain).Skipped
                    };
                }

                if (chain.GetType() == typeof(BaseHTMLCleaner.TextFormatter))
                {
                    config.TextFormatterConfig = new TextFormatterType()
                    {
                        Skipped = ((BaseHTMLCleaner.TextFormatter)chain).Skipped,
                        Delimiters = new DelimiterSymbolType[((BaseHTMLCleaner.TextFormatter) chain).Delimiters.Length]
                    };

                    for (var i = 0; i < ((BaseHTMLCleaner.TextFormatter) chain).Delimiters.Length; i++)
                    {
                        var b = Encoding.ASCII.GetBytes(((BaseHTMLCleaner.TextFormatter) chain).Delimiters);
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