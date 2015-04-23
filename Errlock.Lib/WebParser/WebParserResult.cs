using System;
using System.IO;
using System.Net;

namespace Errlock.Lib.WebParser
{
    public sealed class WebParserResult : IDisposable
    {
        private HttpWebResponse Response { get; set; }
        private string RawContent { get; set; }
        public WebHeaderCollection Headers { get; private set; }
        public string Server { get; private set; }
        public int Status { get; private set; }

        internal WebParserResult(HttpWebResponse response)
        {
            this.Response = response;
            this.Headers = response.Headers;
            this.Server = response.Server;
            this.Status = (int)response.StatusCode;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool IsHtmlPage()
        {
            string type = this.Response.ContentType;
            return type.StartsWith("text/html")
                   || type.StartsWith("text/xml")
                   || type.StartsWith("application/xhtml+xml")
                   || type.StartsWith("application/xml");
        }

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

        private void Dispose(bool disposing)
        {
            if (disposing) {
                this.Response.Dispose();
            }
        }
    }
}