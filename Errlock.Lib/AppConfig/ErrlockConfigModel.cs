using System;
using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.AppConfig
{
    public class ErrlockConfigModel
    {
        public static readonly ErrlockConfigModel Defaults = new ErrlockConfigModel {
            LastStartTime = DateTime.Now,
            ConnectionConfiguration = new ConnectionConfiguration {
                MaxRedirections = 12,
                Timeout = 3500,
                UserAgent = "SuperBot"
            }
        };

        public DateTime LastStartTime { get; set; }
        public ConnectionConfiguration ConnectionConfiguration { get; set; }
    }
}