﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Errlock.Lib.Helpers;
using YamlDotNet.Serialization;

namespace Errlock.Lib.Sessions
{
    public enum SessionEventType
    {
        Created,
        Modified,
        Deleted
    }

    [Serializable]
    public sealed class Session
    {
        /// <summary>
        /// Файл с информацией о сессии
        /// </summary>
        private const string InfoFileName = "info.yml";

        private const string LogsDirectory = "logs";

        /// <summary>
        /// Директория, в которой хранятся данные сессий
        /// </summary>
        private static readonly string SessionsDirectory =
            Path.Combine(AppHelpers.DefaultConfigPath, "sessions");

        /// <summary>
        /// Идентификатор сессии
        /// </summary>
        [YamlMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Адрес сайта
        /// </summary>
        [YamlMember]
        public string Url { get; set; }

        /// <summary>
        /// Настройки анализа
        /// </summary>
        [YamlMember]
        public SessionScanOptions Options { get; set; }

        [YamlIgnore]
        public IEnumerable<SessionLogFile> AllLogs
        {
            get { return this.EnumerateLogs().OrderByDescending(x => x.CreatingDate); }
        }

        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public Session()
        {
            this.Id = Guid.NewGuid();
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

        /// <summary>
        /// Действие, вызываемое при создании, изменении или удалении сессии
        /// </summary>
        public static event EventHandler<SessionEventArgs> SessionChanged;

        /// <summary>
        /// Возвращает директорию текущего экземпляра сессии
        /// </summary>
        /// <returns>Директория, хранящая данные сессии</returns>
        private string GetSessionDirectory()
        {
            return Path.Combine(SessionsDirectory, this.Id.ToString());
        }

        /// <summary>
        /// Перечисляет все существующие логи
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SessionLogFile> EnumerateLogs(string module = null)
        {
            string directory = Path.Combine(this.GetSessionDirectory(), LogsDirectory);
            if (module != null) {
                directory = Path.Combine(directory, module);
            }
            return Directory
                .EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                .Select(file => new SessionLogFile(file))
                .SkipExceptions();
        }

        /// <summary>
        /// Сохраняет лог работы модуля в файл
        /// </summary>
        /// <param name="log">Лог работы, который необходимо сохранить</param>
        public void SaveLog(SessionScanLog log)
        {
            string directory = Path.Combine(this.GetSessionDirectory(), LogsDirectory, log.Module);
            if (! Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            string timestamp = DateTime.Now.DateTimeToUnixTimestamp()
                                       .ToString(CultureInfo.InvariantCulture);
            string fileName = Path.Combine(directory, timestamp + ".log");

            File.WriteAllText(fileName, log.ToString());
        }

        /// <summary>
        /// Удаляет все созданные логи текущей сессии
        /// </summary>
        public void DeleteLogs()
        {
            string directory = Path.Combine(this.GetSessionDirectory(), LogsDirectory);
            if (Directory.Exists(directory)) {
                Directory.Delete(directory);
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Сохраняет или обновляет существующую сессию
        /// </summary>
        public void Save()
        {
            string sessionDir = this.GetSessionDirectory();
            // Создание необходимых директорий
            Directory.CreateDirectory(sessionDir);
            Directory.CreateDirectory(Path.Combine(sessionDir, LogsDirectory));
            string sessionInfoFile = Path.Combine(sessionDir, InfoFileName);
            // Выбор типа действия для события (создан или изменен)
            var eventType = File.Exists(sessionInfoFile)
                                ? SessionEventType.Modified
                                : SessionEventType.Created;
            SerializationHelpers.Serialize(sessionInfoFile, this);
            OnSessionChanged(eventType);
        }

        /// <summary>
        /// Удаляет текущую сессию
        /// </summary>
        public void Delete()
        {
            string path = this.GetSessionDirectory();
            if (! Directory.Exists(path)) {
                return;
            }
            Directory.Delete(path, true);
            OnSessionChanged(SessionEventType.Deleted);
        }

        /// <summary>
        /// Открывает сессию с указанным ID
        /// </summary>
        /// <param name="sessionId">ID сессии</param>
        /// <returns>Экземпляр открытой сессии</returns>
        private static Session Open(string sessionId)
        {
            string sessionInfoFile = Path.Combine(SessionsDirectory, sessionId, InfoFileName);
            if (! File.Exists(sessionInfoFile)) {
                throw new FileNotFoundException("Файл с информацией о сессии не найден");
            }
            return SerializationHelpers.Deserialize<Session>(sessionInfoFile);
        }

        /// <summary>
        /// Открывает сессию по переданному значению Guid
        /// </summary>
        /// <param name="sessionId">Экземплятор класса Guid, представляющий идентификатор
        /// сессии</param>
        /// <returns>Экземпляр открытой сессии</returns>
        public static Session Open(Guid sessionId)
        {
            return Open(sessionId.ToString());
        }

        /// <summary>
        /// Перечисляет все существующие сессии
        /// </summary>
        /// <returns>IEnumerable с экземплярами существующих сессий</returns>
        public static IEnumerable<Session> EnumerateSessions()
        {
            if (! Directory.Exists(SessionsDirectory)) {
                return Enumerable.Empty<Session>();
            }
            var sessions = Directory
                .EnumerateDirectories(SessionsDirectory)
                .Select(Path.GetFileName)
                .Where(GuidHelpers.IsValidGuid)
                .Select(Open);
            return sessions;
        }

        /// <summary>
        /// Проверяет, существует ли сессия с указанным ID
        /// </summary>
        /// <param name="sessionId">ID сессии</param>
        /// <returns>Возвращает True, если сессия существует; иначе возвращает False</returns>
        public static bool Exists(string sessionId)
        {
            string sessionDir = Path.Combine(SessionsDirectory, sessionId);
            return GuidHelpers.IsValidGuid(sessionId) && Directory.Exists(sessionDir);
        }

        private void OnSessionChanged(SessionEventType type)
        {
            var handler = SessionChanged;
            handler.Raise(this, new SessionEventArgs(type));
        }
    }
}