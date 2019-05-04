using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;

namespace HTMLCleanup
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
            if(_pos1 != -1)
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
                _startPos -= len1;

                _text = _text.Remove(_pos3, _endTag.Length);
                _startPos -= _endTag.Length;

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
            } while (true);

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
        private List<SpecialHTMLSymbol> _specialHTML = new List<SpecialHTMLSymbol>();

        public List<SpecialHTMLSymbol> SpecialHTML
        {
            get
            {
                return _specialHTML;
            }
        }

        public SpecialHTMLRemover(TextProcessor next) : base(next) { }

        public override string Process(string text)
        {
            foreach (var sp in _specialHTML)
            {
                text = text.Replace(sp.SpecialHTML, sp.Replacement);
                text = text.Replace("&", "");
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
        /// </summary>
        private List<TagToRemove> _tags = new List<TagToRemove>(new TagToRemove[] {
            new TagToRemove( "<strong", "</strong>" ),
            new TagToRemove( "<span", "</span>" )
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
            foreach (var t in _tags)
            {
                var el = new HTMLElement(t.StartTag, t.EndTag, text);
                do
                {
                    var b = el.FindNext();
                    if (!b) break;
                    el.RemoveTags();
                } while (true);
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
        /// Filled by default values.
        /// </summary>
        private List<TagToRemove> _tags = new List<TagToRemove>(new TagToRemove[] {
            /*
            new TagToRemove( "<script", "</script>" ),
            new TagToRemove( "<style", "</style>" ),
            new TagToRemove( "<link", "/>" ),
            new TagToRemove( "<path", "</path>" ),
            new TagToRemove( "<meta", "/>" ),
            new TagToRemove( "<iframe", "</iframe>" ),
            new TagToRemove( "<svg", "</svg>" ),
            */
            new TagToRemove( "<input", "/>" ),
            //new TagToRemove( "<label", "</label>" )
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
                } while (true);
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
            int startPos = 0;
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
                    var pos1 = substring.LastIndexOfAny(new char[] {'\x0a', '\x0d'});
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
            } while (true);
            return base.Process(result);
        }
    }

    class Program
    {
        private static void ReadConfiguration(TextProcessor chain)
        {
            var pathToConfig = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "Config.xml";
            var serializer = new ConfigSerializer();
            serializer.Deserialize(pathToConfig, chain);
        }

        private static void WriteConfiguration(TextProcessor chain)
        {
            var pathToConfig = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "Config.xml";
            var serializer = new ConfigSerializer();
            serializer.Serialize(pathToConfig, chain);
        }

        /// <summary>
        /// Создает структуру каталогов для сохранения текста страницы.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns>Путь к каталогу.</returns>
        private static string CreateDirectories(string url)
        {
            //  Оставляет только часть URL - "путь", параметры и имя файла отбрасывает.
            //  URL может не иметь части, соответствующей имени файла, поэтому удобнее 
            //  отбросить все от последнего разделителя и добавить стандартное имя файла.
            var baseURL = url.LastIndexOf('/') == -1 ? url : url.Substring(0, url.LastIndexOf('/'));
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

            var l = baseURL.Split(new char[] { '/' });
            //  Формируется структура каталогов (пропускаем "http://").
            for (int i = 2; i < l.Length; i++)
            {
                path = Path.Combine(path, l[i]);
            }

            //  Создает каталоги.
            Directory.CreateDirectory(path);

            return path;
        }

        private static string MakeRequest(string url)
        {
            //  Требуется определить кодировку страницы и преобразовать к стандартной.
            var req = (HttpWebRequest)WebRequest.Create(url);
            var res = req.GetResponse();
            //  Определяет кодировку страницы.
            string charset = String.Empty;
            if (res.ContentType.IndexOf("1251", 0, StringComparison.OrdinalIgnoreCase) != -1) charset = "windows-1251";
            else
                if (res.ContentType.IndexOf("utf-8", 0, StringComparison.OrdinalIgnoreCase) != -1) charset = "utf-8";

            StreamReader f = null;
            string text = String.Empty;

            //  If charset wasn't recognized UTF-8 is used by default.
            if (charset == "utf-8" || string.IsNullOrEmpty(charset))
            {
                f = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                text = f.ReadToEnd();
            }

            if (charset == "windows-1251")
            {
                f = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(1251));
                text = f.ReadToEnd();
                //  Convert to UTF-8.
                var bIn = Encoding.GetEncoding(1251).GetBytes(text);
                var bOut = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.UTF8, bIn);
                text = Encoding.UTF8.GetString(bOut);
            }

            return text;
        }

        private static void WriteTextToFile(string fileName, string text)
        {
            using (var sr = new StreamWriter(fileName))
            {
                sr.Write(text);
                sr.Flush();
            }
        }

        static void Main(string[] args)
        {
            //  Создает последовательность обработки (имеет значение).
            var processChain =
                new TagWithTextRemover(
                    //new SpecialHTMLRemover(
                            //new ParagraphExtractor(
                            //new URLFormatter(
                                //new InnerTagRemover(
                                  null //  new TextFormatter(null)
                                //))))
                                );

            if (args.Count() != 0)
            {
                var url = args[0];

                //  Читает конфигурацию.
                ReadConfiguration(processChain);

                //  Выполняет запрос.
                var s = MakeRequest(url);

                //  Выполняет обработку.
                var output = processChain.Process(s);

                //  Формирует структуру каталогов для сохранения результата.
                var path = CreateDirectories(url);

                //  Формирует имя файла.
                var fileName = path + "\\" + "content.txt";

                //  Сохраняет текст в файл.
                WriteTextToFile(fileName, output);
            }
            else
            {
                //  Writes template of configuration file.
                WriteConfiguration(processChain);
            }
        }
    }
}
