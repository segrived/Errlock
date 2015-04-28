namespace Errlock.Lib.SmartWebRequest
{
    public class ConnectionConfiguration
    {
        public int MaxRedirections { get; set; }
        public string UserAgent { get; set; }
        public int Timeout { get; set; }
        public bool UseProxy { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }
    }
}