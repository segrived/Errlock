using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace Errlock.Lib.SmartWebRequest
{
    /// <summary>
    /// Тип метода запроса серверу
    /// </summary>
    public enum RequestMethod
    {
        [Description("HEAD")]
        Head,

        [Description("GET")]
        Get,

        [Description("POST")]
        Post,

        [Description("UPDATE")]
        Update,

        [Description("DELETE")]
        Delete,

        [Description("PUT")]
        Put
    }

    public class SmartRequest
    {
        /// <summary>
        /// Список юзер-агентов по умолчанию
        /// </summary>
        public static readonly List<string> UserAgentList = new List<string> {
            "Errlock/Bot",
            "Firefox",
            "Opera",
            "Chrome"
        };

        private readonly HttpWebRequest _request;
        private ConnectionConfiguration Options { get; set; }

        public SmartRequest(ConnectionConfiguration options, string url)
        {
            this.Options = options;
            this._request = WebRequest.CreateHttp(url);
            this._request.Timeout = Options.Timeout;
            //this._request.Headers = Options.Headers;
            this._request.UserAgent = Options.UserAgent;
            this._request.MaximumAutomaticRedirections = Options.MaxRedirections;
            if (this.Options.UseProxy) {
                this._request.Proxy = new WebProxy(this.Options.ProxyAddress,
                    this.Options.ProxyPort);
            }
        }

        private HttpWebResponse GetResponse()
        {
            HttpWebResponse response;
            try {
                response = (HttpWebResponse)this._request.GetResponse();
            } catch (WebException ex) {
                if (ex.Status == WebExceptionStatus.ProtocolError) {
                    response = (HttpWebResponse)ex.Response;
                } else {
                    throw;
                }
            }
            return response;
        }

        public HttpWebResponse Request(RequestMethod method = RequestMethod.Get)
        {
            return this.Request(method.ToString().ToUpper());
        }

        public HttpWebResponse Request(string requestType = "GET")
        {
            this._request.Method = requestType;
            return this.GetResponse();
        }

        /// <summary>
        /// Выполняет HEAD-запрос
        /// </summary>
        /// <returns>Ответ сервера</returns>
        public HttpWebResponse HeadRequest()
        {
            return Request("HEAD");
        }

        /// <summary>
        /// Выполняет GET-запрос
        /// </summary>
        /// <returns>Ответ сервера</returns>
        public HttpWebResponse GetRequest()
        {
            return Request("GET");
        }

        /// <summary>
        /// Выполняет POST-запрос
        /// </summary>
        /// <param name="parametersString">Параметры запроса в виде строки</param>
        /// <returns>Ответ сервера</returns>
        public HttpWebResponse PostRequest(string parametersString)
        {
            this._request.Method = "POST";
            this._request.Headers.Add("Accept-Encoding", "gzip, deflate");
            this._request.ContentType = "application/x-www-form-urlencoded";
            var bytes = Encoding.ASCII.GetBytes(parametersString);
            this._request.ContentLength = bytes.Length;
            using (var stream = this._request.GetRequestStream()) {
                stream.Write(bytes, 0, bytes.Length);
            }
            return GetResponse();
        }
    }
}