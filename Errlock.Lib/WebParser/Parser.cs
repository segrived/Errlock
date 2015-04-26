﻿using System.Net;
using System.Text;

namespace Errlock.Lib.WebParser
{
    public class Parser
    {
        private WebParserOptions Options { get; set; }
        private readonly HttpWebRequest _request;

        public Parser(WebParserOptions options, string url)
        {
            this.Options = options;
            this._request = WebRequest.CreateHttp(url);
            this._request.Timeout = Options.Timeout;
            this._request.Headers = Options.Headers;
            this._request.UserAgent = Options.UserAgent;
            this._request.MaximumAutomaticRedirections = Options.MaxRedirections;
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

        public HttpWebResponse Request(string requestType = "GET")
        {
            this._request.Method = requestType;
            return this.GetResponse();
        }

        public HttpWebResponse HeadRequest()
        {
            return Request("HEAD");
        }

        public HttpWebResponse GetRequest()
        {
            return Request("GET");
        }

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