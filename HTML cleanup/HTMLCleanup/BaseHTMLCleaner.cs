﻿using System;
using System.Collections.Generic;
using System.IO;

namespace HTMLCleanup
{
    //  Must be public to be accessible from unit-tests.
    public class BaseHTMLCleaner : IHTMLCleaner
    {
        public class HTMLElement
        {
            private string _text;
            private int _startPos = 0;
            private string _startTag;
            private string _endTag;
            private bool _found;
            private int _pos1;      //  Положение стартового тэга.
            private int _pos2;      //  Положение закрывающей скобки стартового тэга.
            private int _pos3;      //  Положение закрывающего тэга.

            public string Text
            {
                get
                {
                    return _text;
                }
            }

            public HTMLElement(string startTag /*Без закрывающей >.*/, string endTag, string text)
            {
                _text = text;
                _startTag = startTag;
                _endTag = endTag;
            }

            /// <summary>
            /// Ищет следующий элемент заданного типа.
            /// </summary>
            public bool FindNext()
            {
                _found = false;
                _pos1 = _text.IndexOf(_startTag, _startPos, StringComparison.OrdinalIgnoreCase);
                if (_pos1 != -1)
                {
                    //  Нашли открывающий тэг.
                    //  Пропускаем атрибуты.
                    _pos2 = _text.IndexOf(">", _pos1 + _startTag.Length);

                    //  Допускается пустой закрывающий тэг.
                    if (_endTag != String.Empty)
                    {
                        _pos3 = _text.IndexOf(_endTag, _pos2 + 1, StringComparison.OrdinalIgnoreCase);
                        if (_pos3 != -1)
                        {
                            //  Нашли закрывающий тэг.
                            _found = true;
                            //  Переходим к следующему.
                            _startPos = _pos3 + _endTag.Length;
                        }
                        else
                        {
                            //  Ошибка - требуется вывод сообщения.
                        }
                    }
                    else
                    {
                        _pos3 = _pos2 + 1;
                        _found = true;
                        _startPos = _pos3;
                    }
                }
                return _found;
            }

            /// <summary>
            /// Возвращает текст тэга.
            /// </summary>
            /// <returns>Текст тэга.</returns>
            public string GetText()
            {
                if (_found)
                {
                    return _text.Substring(_pos2 + 1, _pos3 - _pos2 - 1);
                }
                return String.Empty;
            }

            public string GetAttr(string attrName)
            {
                if (_found)
                {
                    //  Ищет закрывающую скобку.
                    var endBracketPos = _text.IndexOf(">", _pos1);
                    var attrPos = _text.IndexOf(attrName, _pos1, StringComparison.OrdinalIgnoreCase);
                    if (attrPos != -1 && attrPos < endBracketPos)
                    {
                        //  Копирует содержимое тэга.
                        var tagCopy = _text.Substring(_pos1, endBracketPos - _pos1 + 1);
                        //  Заменяет кавычки на пробелы.
                        tagCopy = tagCopy.Replace('"', ' ');
                        tagCopy = tagCopy.Replace('\'', ' ');
                        var attrValStartPos = tagCopy.IndexOf("=", attrPos - _pos1);
                        if (attrValStartPos != -1)
                        {
                            //  Пропускает все возможные пробелы.
                            do
                            {
                                attrValStartPos++;
                            }
                            while (tagCopy[attrValStartPos] == ' ');

                            var attrValEndPos = tagCopy.IndexOfAny(new char[] { ' ', '>' }, attrValStartPos + 1);
                            if (attrValEndPos != -1)
                            {
                                //  Копирует значение атрибута.
                                return tagCopy.Substring(attrValStartPos, attrValEndPos - attrValStartPos).Trim();
                            }
                            else
                            {
                                //  Сообщение об ошибке (вставить в текст).
                            }
                        }
                    }
                    else
                    {
                        //  Сообщение об ошибке (вставить в текст).
                    }
                }
                return String.Empty;
            }

            /// <summary>
            /// Удаляет тэги, сохраняя текст.
            /// </summary>
            public void RemoveTags()
            {
                if (_found)
                {
                    var len1 = _pos2 - _pos1 + 1;
                    _text = _text.Remove(_pos1, len1);
                    _pos3 -= len1;

                    _text = _text.Remove(_pos3, _endTag.Length);

                    //  Tags can be nested, proceed from the same position.
                    _startPos = _pos1;
                    //  Защита от повторного применения.
                    _found = false;
                }
            }

            /// <summary>
            /// Удаляет тэги вместе со внутренним текстом.
            /// </summary>
            public void RemoveTagsWithText()
            {
                if (_found)
                {
                    var len1 = _pos3 + _endTag.Length - _pos1;
                    _text = _text.Remove(_pos1, len1);
                    _pos2 = _pos1;
                    _pos3 = _pos1;
                    _startPos = _pos1;

                    //  Защита от повторного применения.
                    _found = false;
                }
            }

            /// <summary>
            /// Заменяет тэги вместе со внутренним текстом на заданный текст.
            /// </summary>
            public void ReplaceTagsWithText(string text)
            {
                if (_found)
                {
                    RemoveTagsWithText();
                    _text = _text.Insert(_startPos, text);
                    //  Вставленный текст не сканируем.
                    _startPos += text.Length;
                }
            }
        }

        /// <summary>
        /// Базовый класс обработчика текста.
        /// </summary>
        public abstract class TextProcessor
        {
            private readonly TextProcessor _next;

            /// <summary>
            /// Возвращает следующий обработчик.
            /// Используется при конфигурировании объектов.
            /// </summary>
            public TextProcessor Next
            {
                get
                {
                    return _next;
                }
            }

            protected TextProcessor(TextProcessor next)
            {
                _next = next;
            }

            /// <summary>
            /// Обрабатывает текст и вызывает следующий метод обработки в цепочке.
            /// </summary>
            /// <param name="text">Исходный текст.</param>
            /// <returns>Обработанный текст.</returns>
            public virtual string Process(string text)
            {
                if (_next != null) return _next.Process(text);
                else return text;
            }
        }

        /// <summary>
        /// Извлекает параграфы текста.
        /// </summary>
        public class ParagraphExtractor : TextProcessor
        {
            public ParagraphExtractor(TextProcessor next) : base(next) { }

            public override string Process(string text)
            {
                string result = String.Empty;
                //  Can extract only paragraphs.
                HTMLElement el = new HTMLElement("<p", "</p>", text);
                do
                {
                    var b = el.FindNext();
                    if (!b) break;

                    //  Разделяет параграфы.
                    result += Environment.NewLine + Environment.NewLine;
                    //  Отступ в начале следующего параграфа.
                    result += "    ";
                    //  Текст тэга.
                    result += el.GetText();
                }
                while (true);

                return base.Process(result);
            }
        }

        public class SpecialHTMLSymbol
        {
            private readonly string _specialHTML;
            private readonly string _replacement;

            public string SpecialHTML { get { return _specialHTML; } }
            //  Включить десятичный код.
            public string Replacement { get { return _replacement; } }

            public SpecialHTMLSymbol(string specialHTML, string replacement)
            {
                _specialHTML = specialHTML;
                _replacement = replacement;
            }
        }

        /// <summary>
        /// Заменяет спецсимволы HTML.
        /// </summary>
        public class SpecialHTMLRemover : TextProcessor
        {
            //  Согласно этому списку специальные символы заменяются, 
            //  либо отбрасываются (заполняется из конфигурационного файла).
            //  Аналогично обрабатываются десятичные коды.
            private List<SpecialHTMLSymbol> _specialHTML = new List<SpecialHTMLSymbol>(new SpecialHTMLSymbol[] {
            new SpecialHTMLSymbol( "&#8211;", "-" ),
            new SpecialHTMLSymbol( "&#8217;", "'" ),
            new SpecialHTMLSymbol( "&lt;", "<" ),
            new SpecialHTMLSymbol( "&gt;", ">" )
        });

            public List<SpecialHTMLSymbol> SpecialHTML
            {
                get
                {
                    return _specialHTML;
                }
            }

            public SpecialHTMLRemover(TextProcessor next) : base(next)
            {
            }

            public override string Process(string text)
            {
                //  This symbol is added bypassing configuration file
                //  because string consisting only from spaces is read
                //  from XML as completely empty despite it is stored
                //  correctly (workaround).
                _specialHTML.Add(new SpecialHTMLSymbol("&nbsp;", " "));

                foreach (var sp in _specialHTML)
                {
                    text = text.Replace(sp.SpecialHTML, sp.Replacement);
                }

                return base.Process(text);
            }
        }

        public class TagToRemove
        {
            private readonly string _startTag;   //  Без закрывающей >, чтобы игнорировать атрибуты.
            private readonly string _endTag;

            public string StartTag
            {
                get
                {
                    return _startTag;
                }
            }

            public string EndTag
            {
                get
                {
                    return _endTag;
                }
            }

            public TagToRemove(string startTag, string endTag)
            {
                _startTag = startTag;
                _endTag = endTag;
            }
        }

        /// <summary>
        /// Удаляет тэги внутри параграфов, сохраняя внутренний текст.
        /// </summary>
        public class InnerTagRemover : TextProcessor
        {
            /// <summary>
            /// Список тэгов для удаления (заполняется из конфигурационного файла).
            /// All tags representig text data which should be presaved. Tags must
            /// be arranged in the reverse lexigraphical order. The first tag must
            /// not include closing bracket.
            /// </summary>
            private List<TagToRemove> _tags = new List<TagToRemove>(new TagToRemove[] {
            new TagToRemove( "<ul", "</ul>" ),
            new TagToRemove( "<title", "</title>" ),
            new TagToRemove( "<strong", "</strong>" ),
            new TagToRemove( "<span", "</span>" ),
            new TagToRemove( "<small", "</small>" ),
            new TagToRemove( "<pre", "</pre>" ),
            new TagToRemove( "<p", "</p>" ),
            new TagToRemove( "<main", "</main>" ),
            new TagToRemove( "<li", "</li>" ),
            new TagToRemove( "<html", "</html>" ),
            new TagToRemove( "<header", "</header>" ),
            new TagToRemove( "<head", "</head>" ),
            new TagToRemove( "<h3", "</h3>" ),
            new TagToRemove( "<h3", "</h3>" ),
            new TagToRemove( "<h2", "</h2>" ),
            new TagToRemove( "<h1", "</h1>" ),
            new TagToRemove( "<footer", "</footer>" ),
            new TagToRemove( "<em", "</em>" ),
            new TagToRemove( "<div", "</div>" ),
            new TagToRemove( "<code", "</code>" ),
            new TagToRemove( "<body", "</body>" ),
            new TagToRemove( "<article", "</article>" )
        });

            public List<TagToRemove> Tags
            {
                get
                {
                    return _tags;
                }
            }

            public InnerTagRemover(TextProcessor next)
                : base(next)
            {
            }

            public override string Process(string text)
            {
                for (var i = 0; i < _tags.Count; i++)
                {
                    var t = _tags[i];
                    var el = new HTMLElement(t.StartTag, t.EndTag, text);
                    do
                    {
                        var b = el.FindNext();
                        if (!b) break;
                        el.RemoveTags();
                    }
                    while (true);
                    text = el.Text;
                }
                return base.Process(text);
            }
        }

        /// <summary>
        /// Удаляет тэги вместе со внутренним текстом.
        /// </summary>
        public class TagWithTextRemover : TextProcessor
        {
            /// <summary>
            /// Список тэгов для удаления (заполняется из конфигурационного файла).
            /// Filled by default values. When tag doesn't have closing counterpart,
            /// corresponding value should be empty string. Tags must be in the 
            /// reverse lexigraphical order.
            /// </summary>
            private List<TagToRemove> _tags = new List<TagToRemove>(new TagToRemove[] {
            new TagToRemove( "<script", "</script>" ),
            new TagToRemove( "<style", "</style>" ),
            new TagToRemove( "<link", "" ),
            new TagToRemove( "<path", "</path>" ),
            new TagToRemove( "<meta", "" ),
            new TagToRemove( "<iframe", "</iframe>" ),
            new TagToRemove( "<svg", "</svg>" ),
            new TagToRemove( "<input", "" ),
            new TagToRemove( "<label", "</label>" ),
            new TagToRemove( "<form", "</form>" ),
            new TagToRemove( "<noscript", "</noscript>" ),
            new TagToRemove( "<nav", "</nav>" ),
            new TagToRemove( "<!DOCTYPE", "" ),
            new TagToRemove( "<button", "</button>" ),
            new TagToRemove( "<aside", "</aside>" ),
            new TagToRemove( "<!--[if", "<![endif]-->" ),
            new TagToRemove( "<!--", "" )
        });

            public List<TagToRemove> Tags
            {
                get
                {
                    return _tags;
                }
                //  Writeable for unit-testing.
                set
                {
                    _tags = value;
                }
            }

            public TagWithTextRemover(TextProcessor next)
                : base(next)
            {
            }

            public override string Process(string text)
            {
                foreach (var t in _tags)
                {
                    HTMLElement el = new HTMLElement(t.StartTag, t.EndTag, text);
                    do
                    {
                        var b = el.FindNext();
                        if (!b) break;
                        el.RemoveTagsWithText();
                    }
                    while (true);
                    text = el.Text;
                }
                return base.Process(text);
            }
        }

        /// <summary>
        /// Помещает URL в квадратные скобки.
        /// </summary>
        public class URLFormatter : TextProcessor
        {
            public URLFormatter(TextProcessor next)
                : base(next)
            {
            }

            public override string Process(string text)
            {
                string result = String.Empty;
                HTMLElement el = new HTMLElement("<a", "</a>", text);

                do
                {
                    var b = el.FindNext();
                    if (!b) break;
                    var href = el.GetAttr("href");
                    if (href != String.Empty)
                    {
                        el.ReplaceTagsWithText("[" + href + "]");
                    }
                }
                while (true);

                text = el.Text;

                return base.Process(text);
            }
        }

        /// <summary>
        /// Разбивает текст на строки шириной не более _max символов.
        /// </summary>
        public class TextFormatter : TextProcessor
        {
            /// <summary>
            /// Список символов-разделителей (заполняется из конфигурационного файла).
            /// Should have default value for creating configuration template.
            /// </summary>
            private char[] _delimiters = { ' ', ',', '.', ':', ';', '?', '.', '!' };
            //  +1 позволяет обработать случай, когда слово заканчивается точно на границе.
            //  Можно заполнять из конфигурационного файла.
            private const int _max = 81;

            public char[] Delimiters
            {
                get
                {
                    return _delimiters;
                }

                set
                {
                    _delimiters = value;
                }
            }

            public TextFormatter(TextProcessor next)
                : base(next)
            {
            }

            public override string Process(string text)
            {
                var pos = 0;
                var result = String.Empty;
                do
                {
                    if (text.Length - pos > _max - 1)
                    {
                        var substring = text.Substring(pos, _max);

                        //  Проверяет на конец строки.
                        var pos1 = substring.LastIndexOfAny(new char[] { '\x0a', '\x0d' });
                        if (pos1 != -1)
                        {
                            //  Попал конец строки - обрезает по нему.
                            result += substring.Substring(0, pos1 + 1);
                            pos += pos1 + 1;
                        }
                        else
                        {
                            //  Ищет другие разделители.
                            pos1 = substring.LastIndexOfAny(_delimiters);

                            if (pos1 == -1)
                            {
                                result += substring + Environment.NewLine;
                                pos += _max;
                            }
                            else
                            {
                                //  Обрезает строку по последнему разделителю.
                                result += substring.Substring(0, pos1 + 1) + Environment.NewLine;
                                pos += pos1 + 1;
                            }
                        }
                    }
                    else
                    {
                        result += text.Substring(pos, text.Length - pos) + Environment.NewLine;
                        break;
                    }
                }
                while (true);
                return base.Process(result);
            }
        }

        private static TextProcessor CreateProcessingChain() {
            return  //  Создает последовательность обработки (имеет значение).
                new TagWithTextRemover(
                    new SpecialHTMLRemover(
                            //  Using of paragraph extractor must be configurable.
                            //new ParagraphExtractor(
                            new URLFormatter(
                                new InnerTagRemover(
                                    new TextFormatter(null)
                                ))));//);

        }

        private static string GetConfigurationFileName()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "Config.xml";
        }

        private static void ReadConfiguration(TextProcessor chain)
        {
            var pathToConfig = GetConfigurationFileName();
            var serializer = new ConfigSerializer();
            serializer.Deserialize(pathToConfig, chain);
        }

        public static void WriteConfiguration()
        {
            TextProcessor processingChain = CreateProcessingChain();

            var pathToConfig = GetConfigurationFileName();
            var serializer = new ConfigSerializer();
            serializer.Serialize(pathToConfig, processingChain);
        }

        public string Process(string html)
        {
            //  Создает последовательность обработки (имеет значение).
            var processingChain = CreateProcessingChain();

            //  Читает конфигурацию.
            ReadConfiguration(processingChain);

            //  Выполняет обработку.
            return processingChain.Process(html);
        }
    }
}