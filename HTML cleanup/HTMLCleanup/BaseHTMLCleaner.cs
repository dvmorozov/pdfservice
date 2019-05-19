﻿using System;
using System.Collections.Generic;

namespace HtmlCleanup
{
    //  Must be public to be accessible from unit-tests.
    //  After any changes in nested class configuration
    //  regenerate configuration files.
    public abstract class BaseHtmlCleaner : IHtmlCleaner
    {
        public class HtmlTag
        {
            private string _startTag;
            private string _endTag;

            public HtmlTag(string startTag, string endTag)
            {
                _startTag = startTag;
                _endTag = endTag;
            }

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
        }

        public class HtmlElement
        {
            private string _text;
            private int _startPos = 0;
            private string _startTag;
            private string _endTag;
            private bool _found;
            private int _pos1;      //  Start tag position.
            private int _pos2;      //  Start tag closing bracket position.
            private int _pos3;      //  End tag position.

            public string Text
            {
                get
                {
                    return _text;
                }
            }

            public HtmlElement(string startTag /*Should not include closing >.*/, string endTag, string text)
            {
                _text = text;
                _startTag = startTag;
                _endTag = endTag;
            }

            /// <summary>
            /// Searches for text tag of given type.
            /// </summary>
            public bool FindNext()
            {
                _found = false;
                _pos1 = _text.IndexOf(_startTag, _startPos, StringComparison.OrdinalIgnoreCase);
                if (_pos1 != -1)
                {
                    //  Start tag was found.
                    //  Skips attributes.
                    _pos2 = _text.IndexOf(">", _pos1 + _startTag.Length);

                    //  Empty closing tag is permitted.
                    if (_endTag != String.Empty)
                    {
                        _pos3 = _text.IndexOf(_endTag, _pos2 + 1, StringComparison.OrdinalIgnoreCase);
                        if (_pos3 != -1)
                        {
                            //  End tag was found.
                            _found = true;
                            //  Go to next tag.
                            _startPos = _pos3 + _endTag.Length;
                        }
                        else
                        {
                            //  Error (TODO: add message).
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
            /// Returns tag internal text.
            /// </summary>
            /// <returns>Tag internal text.</returns>
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
                    //  Searches for closing bracket.
                    var endBracketPos = _text.IndexOf(">", _pos1);
                    var attrPos = _text.IndexOf(attrName, _pos1, StringComparison.OrdinalIgnoreCase);
                    if (attrPos != -1 && attrPos < endBracketPos)
                    {
                        //  Copies tag text.
                        var tagCopy = _text.Substring(_pos1, endBracketPos - _pos1 + 1);
                        //  Replaces quotation marks by spaces.
                        tagCopy = tagCopy.Replace('"', ' ');
                        tagCopy = tagCopy.Replace('\'', ' ');
                        var attrValStartPos = tagCopy.IndexOf("=", attrPos - _pos1);
                        if (attrValStartPos != -1)
                        {
                            //  Skips all spaces.
                            do
                            {
                                attrValStartPos++;
                            }
                            while (tagCopy[attrValStartPos] == ' ');

                            var attrValEndPos = tagCopy.IndexOfAny(new char[] { ' ', '>' }, attrValStartPos + 1);
                            if (attrValEndPos != -1)
                            {
                                //  Copies attribute value.
                                return tagCopy.Substring(attrValStartPos, attrValEndPos - attrValStartPos).Trim();
                            }
                            else
                            {
                                //  Error. TODO: add error message to text.
                            }
                        }
                    }
                    else
                    {
                        //  Error. TODO: add error message to text.
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
            /// Controls if object of inherited class is actually used in the processing chain.
            /// </summary>
            private bool _skipped;

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

            public bool Skipped
            {
                get
                {
                    return _skipped;
                }
                set
                {
                    _skipped = value;
                }
            }

            protected TextProcessor(TextProcessor next)
            {
                _next = next;
            }

            /// <summary>
            /// Does actual text processing. Should be implemented in 
            /// descendant classes.
            /// </summary>
            /// <param name="original">HTML text partially processed at previous stages.</param>
            /// <returns>Processed HTML text.</returns>
            protected abstract string ActualProcessing(string original);

            /// <summary>
            /// Обрабатывает текст и вызывает следующий метод обработки в цепочке.
            /// </summary>
            /// <param name="original">Исходный текст.</param>
            /// <returns>Обработанный текст.</returns>
            public string Process(string original)
            {
                //  Does processing, if enabled and then calls
                //  next processing algorithm in the chain.
                var processed = original;
                if (!Skipped)
                    processed = ActualProcessing(original);

                if (_next != null) return _next.Process(processed);
                else return processed;
            }
        }

        /// <summary>
        /// Creates and initializes domain-specific instance of ParagraphExtractor.
        /// </summary>
        /// <param name="next">Next processing object in the chain.</param>
        /// <returns>Instance of ParagraphExtractor specific for the domain supported
        /// by inherited class.</returns>
        protected abstract ParagraphExtractor GetParagraphExtractor(TextProcessor next);

        /// <summary>
        /// Извлекает параграфы текста.
        /// </summary>
        public class ParagraphExtractor : TextProcessor
        {
            /// <summary>
            /// Paragraph tag.
            /// </summary>
            private HtmlTag _tag;

            public HtmlTag Tag
            {
                get
                {
                    return _tag;
                }
                set
                {
                    _tag = value;
                }
            }

            public ParagraphExtractor(TextProcessor next) : base(next)
            {
                //  By default isn't used in processing chain.
                //  Can be enabled by configuration file, using
                //  should be consistent with using other parts.
                Skipped = true;
                //  Default paragraph tag.
                _tag = new HtmlTag("<p", "</p>");
            }

            protected override string ActualProcessing(string text)
            {
                string result = String.Empty;
                //  Can extract only paragraphs.
                HtmlElement el = new HtmlElement(_tag.StartTag, _tag.EndTag, text);
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

                return result;
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
            new SpecialHTMLSymbol( "&#8220;", "\"" ),
            new SpecialHTMLSymbol( "&#8221;", "\"" ),
            new SpecialHTMLSymbol( "&lt;", "<" ),
            new SpecialHTMLSymbol( "&gt;", ">" ),
            new SpecialHTMLSymbol( "&amp;", "&" )
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

            protected override string ActualProcessing(string text)
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

                return text;
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

        protected abstract InnerTagRemover GetInnerTagRemover(TextProcessor next);

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
            private List<TagToRemove> _tags;

            public List<TagToRemove> Tags
            {
                get
                {
                    return _tags;
                }
                //  Writeable for external initialization.
                set
                {
                    _tags = value;
                }
            }

            public InnerTagRemover(TextProcessor next)
                : base(next)
            {
            }

            protected override string ActualProcessing(string text)
            {
                for (var i = 0; i < _tags.Count; i++)
                {
                    var t = _tags[i];
                    var el = new HtmlElement(t.StartTag, t.EndTag, text);
                    do
                    {
                        var b = el.FindNext();
                        if (!b) break;
                        el.RemoveTags();
                    }
                    while (true);
                    text = el.Text;
                }
                return text;
            }
        }

        /// <summary>
        /// Creates and initializes domain-specific instance of TagWithTextRemover.
        /// </summary>
        /// <param name="next">Next processing object in the chain.</param>
        /// <returns>Instance of TagWithTextRemover specific for the domain supported
        /// by inherited class.</returns>
        protected abstract TagWithTextRemover GetTagWithTextRemover(TextProcessor next);

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
            private List<TagToRemove> _tags;

            public List<TagToRemove> Tags
            {
                get
                {
                    return _tags;
                }
                //  Writeable for external initialization.
                set
                {
                    _tags = value;
                }
            }

            public TagWithTextRemover(TextProcessor next)
                : base(next)
            {
            }

            protected override string ActualProcessing(string text)
            {
                foreach (var t in _tags)
                {
                    HtmlElement el = new HtmlElement(t.StartTag, t.EndTag, text);
                    do
                    {
                        var b = el.FindNext();
                        if (!b) break;
                        el.RemoveTagsWithText();
                    }
                    while (true);
                    text = el.Text;
                }
                return text;
            }
        }

        /// <summary>
        /// Помещает URL в квадратные скобки.
        /// </summary>
        public class UrlFormatter : TextProcessor
        {
            public UrlFormatter(TextProcessor next)
                : base(next)
            {
            }

            protected override string ActualProcessing(string text)
            {
                string result = String.Empty;
                HtmlElement el = new HtmlElement("<a", "</a>", text);

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

                return text;
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

            protected override string ActualProcessing(string text)
            {
                var pos = 0;
                var processed = String.Empty;
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
                            processed += substring.Substring(0, pos1) + Environment.NewLine;
                            pos += pos1 + 1;
                        }
                        else
                        {
                            //  Ищет другие разделители.
                            pos1 = substring.LastIndexOfAny(_delimiters);

                            if (pos1 == -1)
                            {
                                processed += substring + Environment.NewLine;
                                pos += _max;
                            }
                            else
                            {
                                //  Обрезает строку по последнему разделителю.
                                processed += substring.Substring(0, pos1 + 1) + Environment.NewLine;
                                pos += pos1 + 1;
                            }
                        }
                    }
                    else
                    {
                        processed += text.Substring(pos, text.Length - pos) + Environment.NewLine;
                        break;
                    }
                }
                while (true);

                //  Removes lines containing only white spaces.
                var strings = new List<string>(processed.Split('\n'));
                var index = 0;
                while (index < strings.Count)
                {
                    var s = strings[index];
                    var m = s.Trim(' ', '\t', '\r');
                    if (m == "")
                        strings.Remove(s);
                    else
                    {
                        //  Replaces original string with
                        //  string without white spaces.
                        //  Leading spaces are left to 
                        //  safe formatting.
                        strings[index] = s.Trim('\t', '\r').TrimEnd();
                        index++;
                    }
                }
                return string.Join("\n", strings);
            }
        }

        protected virtual TextProcessor CreateProcessingChain() {
            return  //  Создает последовательность обработки (имеет значение).
                GetTagWithTextRemover(
                    //  Replacing tags with text is done before replacing
                    //  special characters to avoid interpreting text as HTML tags.
                    GetInnerTagRemover(
                        //  By default ParagraphExtractor is disabled (see constructor).
                        GetParagraphExtractor(
                            new UrlFormatter(
                                new SpecialHTMLRemover(
                                    new TextFormatter(null)
                                )))));
        }

        protected abstract string GetConfigurationFileName();

        protected void ReadConfiguration(TextProcessor chain)
        {
            var pathToConfig = GetConfigurationFileName();
            var serializer = new CleanerConfigSerializer();
            serializer.Deserialize(pathToConfig, chain);
        }

        public void WriteConfiguration()
        {
            TextProcessor processingChain = CreateProcessingChain();

            var pathToConfig = GetConfigurationFileName();
            var serializer = new CleanerConfigSerializer();
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
