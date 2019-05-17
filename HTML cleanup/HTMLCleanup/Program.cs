using System;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using HTMLCleanup;

namespace HtmlCleanup
{
    class Program
    {
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
            var req = WebRequest.Create(url);
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
            if (args.Count() != 0)
            {
                var url = args[0];

                var injector = new HTMLCleanerInjector(new BaseInjectorConfig());
                //  Creating cleaner instance based on URL.
                var processChain = injector.CreateHTMLCleaner(url);

                //  Выполняет запрос.
                var s = MakeRequest(url);

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
                //  Default HTML cleaner for writing configutaion.
                var processChain = new WordPressHtmlCleaner();
                //  Writes template of configuration file.
                processChain.WriteConfiguration();
            }
        }
    }
}
