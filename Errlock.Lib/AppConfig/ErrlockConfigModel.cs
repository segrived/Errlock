using System;

namespace Errlock.Lib.AppConfig
{
    public class ErrlockConfigModel
    {
        public static readonly ErrlockConfigModel Defaults = new ErrlockConfigModel {
            LastStartTime = DateTime.Now
        };

        public DateTime LastStartTime { get; set; }
    }
}