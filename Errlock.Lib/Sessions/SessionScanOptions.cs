namespace Errlock.Lib.Sessions
{
    /// <summary>
    /// Настройки сканирования сессии
    /// </summary>
    public class SessionScanOptions
    {
        /// <summary>
        /// Максимальная глубина рекусии при обходе сайта
        /// </summary>
        public int RecursionDepth { get; set; }

        /// <summary>
        /// Максимальное количество ссылок со страницы
        /// </summary>
        public int FetchPerPage { get; set; }

        /// <summary>
        /// Глобальное ограничение на количество ссылок
        /// </summary>
        public int MaxLinks { get; set; }

        /// <summary>
        /// Использовать случайные ссылки вместо последовательных
        /// </summary>
        public bool UseRandomLinks { get; set; }

        /// <summary>
        /// Игнорировать якоря в URL
        /// </summary>
        public bool IngoreAnchors { get; set; }
    }
}