using System;
using System.IO;
using System.Net;
using System.Text;

namespace HtmlCleanup
{
    public class HtmlCleanerApp
    {
        /// <summary>
        /// Creates folder structure to save content of page.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns>Path to folder.</returns>
        public static string CreateDirectories(string url)
        {
            //  Gets base path.
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

            string[] l = url.Split(new char[] { '/' });
            //  Path is formed ("http://" is skipped).
            for (int i = 2; i < l.Length; i++)
            {
                path = Path.Combine(path, l[i]);
            }

            //  Created folder.
            Directory.CreateDirectory(path);

            return path;
        }

        public static string MakeRequest(string url)
        {
            //  Defines code page and convert it to UTF-8.
            WebRequest req = WebRequest.Create(url);
            using (WebResponse res = req.GetResponse())
            {
                //  Searches for code page name.
                string charset = string.Empty;
                if (res.ContentType.IndexOf("1251", 0, StringComparison.OrdinalIgnoreCase) != -1) charset = "windows-1251";
                else
                    if (res.ContentType.IndexOf("utf-8", 0, StringComparison.OrdinalIgnoreCase) != -1) charset = "utf-8";

                string text = string.Empty;
                StreamReader f;
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
                    byte[] bIn = Encoding.GetEncoding(1251).GetBytes(text);
                    byte[] bOut = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.UTF8, bIn);
                    text = Encoding.UTF8.GetString(bOut);
                }

                return text;
            }
        }

        public static void WriteTextToFile(string fileName, string text)
        {
            using (StreamWriter sr = new StreamWriter(fileName))
            {
                sr.Write(text);
                sr.Flush();
            }
        }
    }
}
