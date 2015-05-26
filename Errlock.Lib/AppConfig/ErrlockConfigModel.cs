using System;
using Errlock.Lib.RequestWrapper;

namespace Errlock.Lib.AppConfig
{
    /// <summary>
    /// Модель настроек Errlock
    /// </summary>
    public class ErrlockConfigModel
    {
        /// <summary>
        /// Настройки по умолчанию: используются при создании файла настроек при первом запуске,
        /// либо пересоздания в случае проблем
        /// </summary>
        public static readonly ErrlockConfigModel Defaults = new ErrlockConfigModel {
            ConnectionConfiguration = new ConnectionConfiguration {
                MaxRedirections = 12,
                Timeout = 3500,
                UserAgent = "SuperBot",
                UseProxy = false,
                ProxyAddress = "",
                ProxyPort = 0
            }
        };

        /// <summary>
        /// Настройки подключения
        /// </summary>
        public ConnectionConfiguration ConnectionConfiguration { get; private set; }
    }
}