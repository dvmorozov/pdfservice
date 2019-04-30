
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
    public class ConfigSerializer
    {
        /// <summary>
        /// Восстанавливает конфигурацию объектов по данным из файла.
        /// </summary>
        /// <param name="fileName">Имя файла для чтения конфигурации.</param>
        /// <param name="chain">Первый объект в цепочке обработчиков.</param>
        public void Deserialize(string fileName, TextProcessor chain)
        {
            var config = new HTMLCleanupConfig();
            using (var reader = new StreamReader(fileName))
            {
                var serializer = new XmlSerializer(typeof(HTMLCleanupConfig));
                config = (HTMLCleanupConfig)serializer.Deserialize(reader);
            }

            while (chain != null)
            {
                if (chain.GetType() == typeof(ParagraphExtractor))
                {

                }

                if (chain.GetType() == typeof(SpecialHTMLRemover))
                {
                    ((SpecialHTMLRemover)chain).SpecialHTML.Clear();
                    foreach (var t in config.SpecialHTMLRemoverConfig.SpecialHTML)
                    {
                        ((SpecialHTMLRemover)chain).SpecialHTML.Add(new SpecialHTMLSymbol(t.SpecialHTML, t.Replacement));
                    }
                }

                if (chain.GetType() == typeof(InnerTagRemover))
                {
                    ((InnerTagRemover)chain).Tags.Clear();
                    foreach (var t in config.InnerTagRemoverConfig.Tags)
                    {
                        ((InnerTagRemover)chain).Tags.Add(new TagToRemove(t.StartTagWithoutBracket, t.EndTag));
                    }
                }

                if (chain.GetType() == typeof(TagWithTextRemover))
                {
                    ((TagWithTextRemover)chain).Tags.Clear();
                    foreach (var t in config.TagWithTextRemoverConfig.Tags)
                    {
                        ((TagWithTextRemover)chain).Tags.Add(new TagToRemove(t.StartTagWithoutBracket, t.EndTag));
                    }
                }

                if (chain.GetType() == typeof(URLFormatter))
                {

                }

                if (chain.GetType() == typeof(TextFormatter))
                {
                    var b = new byte[config.TextFormatterConfig.Delimiters.Length];
                    for (var i = 0; i < config.TextFormatterConfig.Delimiters.Length; i++)
                    {
                        b[i] = config.TextFormatterConfig.Delimiters[i].SymbolCode;
                    }

                    ((TextFormatter)chain).Delimiters = Encoding.ASCII.GetChars(b);
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
        public void Serialize(string fileName, TextProcessor chain)
        {
            var config = new HTMLCleanupConfig();
            while (chain != null)
            {
                if (chain.GetType() == typeof(ParagraphExtractor))
                {

                }

                if (chain.GetType() == typeof(SpecialHTMLRemover))
                {
                    config.SpecialHTMLRemoverConfig = new SpecialHTMLRemoverType
                    {
                        SpecialHTML = new SpecialHTMLSymbolType[((SpecialHTMLRemover) chain).SpecialHTML.Count]
                    };

                    for (var i = 0; i < ((SpecialHTMLRemover) chain).SpecialHTML.Count; i++)
                    {
                        config.SpecialHTMLRemoverConfig.SpecialHTML[i] = new SpecialHTMLSymbolType
                        {
                            SpecialHTML = ((SpecialHTMLRemover) chain).SpecialHTML[i].SpecialHTML, 
                            Replacement = ((SpecialHTMLRemover) chain).SpecialHTML[i].Replacement
                        };
                    }
                }

                if (chain.GetType() == typeof(InnerTagRemover))
                {
                    config.InnerTagRemoverConfig = new InnerTagRemoverType()
                    {
                        Tags = new TagToRemoveType[((InnerTagRemover) chain).Tags.Count]
                    };

                    for (var i = 0; i < ((InnerTagRemover) chain).Tags.Count; i++)
                    {
                        config.InnerTagRemoverConfig.Tags[i] = new TagToRemoveType
                        {
                            StartTagWithoutBracket = ((InnerTagRemover) chain).Tags[i].StartTag, 
                            EndTag = ((InnerTagRemover) chain).Tags[i].EndTag
                        };
                    }
                }

                if (chain.GetType() == typeof(TagWithTextRemover))
                {
                    config.TagWithTextRemoverConfig = new TagWithTextRemoverType()
                    {
                        Tags = new TagToRemoveType[((TagWithTextRemover) chain).Tags.Count]
                    };

                    for (var i = 0; i < ((TagWithTextRemover) chain).Tags.Count; i++)
                    {
                        config.TagWithTextRemoverConfig.Tags[i] = new TagToRemoveType()
                        {
                            StartTagWithoutBracket = ((TagWithTextRemover) chain).Tags[i].StartTag,
                            EndTag = ((TagWithTextRemover) chain).Tags[i].EndTag
                        };
                    }
                }

                if (chain.GetType() == typeof(URLFormatter))
                {

                }

                if (chain.GetType() == typeof(TextFormatter))
                {
                    config.TextFormatterConfig = new TextFormatterType()
                    {
                        Delimiters = new DelimiterSymbolType[((TextFormatter) chain).Delimiters.Length]
                    };

                    for (var i = 0; i < ((TextFormatter) chain).Delimiters.Length; i++)
                    {
                        var b = Encoding.ASCII.GetBytes(((TextFormatter) chain).Delimiters);
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