using System;

namespace Errlock.Lib.Sessions
{
    public class Session : IModel
    {
        /// <summary>
        /// Идентификатор сессии
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Адрес сайта
        /// </summary>
        public string Url {get; set; }

        /// <summary>
        /// Настройки анализа
        /// </summary>
        public SessionScanOptions Options { get; set; }

        /// <summary>
        /// Действие, вызываемое при создании, изменении или удалении сессии
        /// </summary>
        public Session()
        {
            //this.Id = Guid.NewGuid();
            this.Options = new SessionScanOptions();
        }

        /// <summary>
        /// Создает новый экземпляр сессии. Для сохранения данных на диск необходимо вызвать
        /// метод Save() у необходимого экземпляра
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="options">Настройки сканирования сессии</param>
        public Session(string url, SessionScanOptions options) : this()
        {
            this.Url = url;
            this.Options = options;
        }

        public override string ToString()
        {
            return this.Url;
        }
    }
}
