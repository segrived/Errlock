namespace Errlock.Lib.RequestWrapper
{
    /// <summary>
    /// Настройки подключения к серверу
    /// </summary>
    public class ConnectionConfiguration
    {
        /// <summary>
        /// Максимальное количество перенаправлений
        /// </summary>
        public int MaxRedirections { get; set; }

        /// <summary>
        /// User-Agent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Максимальное время ожидания ответа от сервера
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Использовать прокси
        /// </summary>
        public bool UseProxy { get; set; }

        /// <summary>
        /// Адрес прокси-сервера (используется, если UseProxy = true)
        /// </summary>
        public string ProxyAddress { get; set; }

        /// <summary>
        /// Порт прокси-сервера  (используется, если UseProxy = true)
        /// </summary>
        public int ProxyPort { get; set; }
    }
}