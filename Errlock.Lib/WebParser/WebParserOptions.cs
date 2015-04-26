using System.Net;

namespace Errlock.Lib.WebParser
{
    public class WebParserOptions
    {
        public int Timeout { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public string UserAgent { get; set; }
        public int MaxRedirections { get; set; }

        public WebParserOptions()
        {
            this.Timeout = 2000;
            this.Headers = new WebHeaderCollection();
            this.UserAgent = "ErrlockBot 1.0";
        }
    }
}