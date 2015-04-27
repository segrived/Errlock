using System.Net;

namespace Errlock.Lib.SmartWebRequest
{
    public class ConnectionConfiguration
    {
        public int MaxRedirections { get; set; }
        public string UserAgent { get; set; }
        public int Timeout { get; set; }
    }
}