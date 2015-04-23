using System.Net;

namespace Errlock.Lib.WebParser
{
    public class Parser
    {
        private WebParserOptions Options { get; set; }

        public Parser(WebParserOptions options)
        {
            this.Options = options;
        }

        public WebParserResult Process(string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.Timeout = this.Options.Timeout;
            request.Headers = this.Options.Headers;
            request.Method = this.Options.Method;
            request.UserAgent = this.Options.UserAgent;
            request.MaximumAutomaticRedirections = 10;
            HttpWebResponse response;
            try {
                response = (HttpWebResponse)request.GetResponse();
            } catch (WebException ex) {
                if (ex.Status == WebExceptionStatus.ProtocolError) {
                    response = (HttpWebResponse)ex.Response;
                } else {
                    throw;
                }
            }
            return new WebParserResult(response);
        }
    }
}