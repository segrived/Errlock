using System;
using System.IO;
using System.Net;

namespace Errlock.Lib.WebParser
{
    public sealed class WebParserResult : IDisposable
    {
        /// <summary>
        /// Ответ сервера
        /// </summary>
        private HttpWebResponse Response { get; set; }

        /// <summary>
        /// Содержимое страницы
        /// </summary>
        private string RawContent { get; set; } 

        /// <summary>
        /// Коллекция заголовков, пришедших с сервера
        /// </summary>
        public WebHeaderCollection Headers { get; private set; }

        /// <summary>
        /// Информация о названии и версии веб-вервера
        /// </summary>
        public string Server { get; private set; }

        /// <summary>
        /// Код статуса
        /// </summary>
        public int Status { get; private set; }

        internal WebParserResult(HttpWebResponse response)
        {
            this.Response = response;
            this.Headers = response.Headers;
            this.Server = response.Server;
            this.Status = (int)response.StatusCode;
        }

        /// <summary>
        /// Проверяет ответ веб-сервера и определяет, является ли URL, указанный в
        /// запросе, HTML либо XML страницей
        /// </summary>
        /// <returns>True - если это HTML либо XML данные; иначе - False</returns>
        public bool IsHtmlPage()
        {
            string type = this.Response.ContentType;
            return type.StartsWith("text/html")
                   || type.StartsWith("text/xml")
                   || type.StartsWith("application/xhtml+xml")
                   || type.StartsWith("application/xml");
        }

        /// <summary>
        /// Загружает данные в строку
        /// </summary>
        /// <param name="useCached">
        /// Если true, то при возможности будет использована ранее загруженная страница
        /// </param>
        /// <returns>Загруженные в строку данные</returns>
        public string Download(bool useCached = true)
        {
            if (useCached && this.RawContent != null) {
                return RawContent;
            }
            var stream = this.Response.GetResponseStream();
            using (var reader = new StreamReader(stream)) {
                string content = reader.ReadToEnd();
                this.RawContent = content;
                return content;
            }
        }

        /// <summary>
        /// Освобождает ресурсы
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                this.Response.Dispose();
            }
        }
    }
}