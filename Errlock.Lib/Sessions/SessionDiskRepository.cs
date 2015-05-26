using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Errlock.Lib.Helpers;

namespace Errlock.Lib.Sessions
{
    public class SessionDiskRepository : IRepository<Session>
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
        /// Возвращает директорию текущего экземпляра сессии
        /// </summary>
        /// <returns>Директория, хранящая данные сессии</returns>
        private static string GetSessionDirectory(Session session)
        {
            return Path.Combine(SessionsDirectory, session.Id.ToString());
        }

        public void InsertOrUpdate(Session session)
        {
            string sessionDir = GetSessionDirectory(session);
            // Создание необходимых директорий
            Directory.CreateDirectory(sessionDir);
            Directory.CreateDirectory(Path.Combine(sessionDir, LogsDirectory));
            string sessionInfoFile = Path.Combine(sessionDir, InfoFileName);
            SerializationHelpers.Serialize(sessionInfoFile, session);
        }

        /// <summary>
        /// Удаляет текущую сессию
        /// </summary>
        public void Delete(Session session)
        {
            string path = GetSessionDirectory(session);
            if (! Directory.Exists(path)) {
                return;
            }
            Directory.Delete(path, true);
        }

        /// <summary>
        /// Проверяет, существует ли сессия с указанным ID
        /// </summary>
        /// <param name="sessionId">ID сессии</param>
        /// <returns>Возвращает True, если сессия существует; иначе возвращает False</returns>
        public bool Exists(Guid sessionId)
        {
            string sessionIdStr = sessionId.ToString();
            string sessionDir = Path.Combine(SessionsDirectory, sessionIdStr);
            return GuidHelpers.IsValidGuid(sessionIdStr) && Directory.Exists(sessionDir);
        }

        /// <summary>
        /// Перечисляет все существующие сессии
        /// </summary>
        /// <returns>IEnumerable с экземплярами существующих сессий</returns>
        public IEnumerable<Session> EnumerateAll()
        {
            if (!Directory.Exists(SessionsDirectory)) {
                return Enumerable.Empty<Session>();
            }
            var sessions = Directory
                .EnumerateDirectories(SessionsDirectory)
                .Select(Path.GetFileName)
                .Where(GuidHelpers.IsValidGuid)
                .Select(GetItemById);
            return sessions;
        }

        /// <summary>
        /// Открывает сессию с указанным ID
        /// </summary>
        /// <param name="sessionId">ID сессии</param>
        /// <returns>Экземпляр открытой сессии</returns>
        private static Session GetItemById(string sessionId)
        {
            string sessionInfoFile = Path.Combine(SessionsDirectory, sessionId, InfoFileName);
            if (!File.Exists(sessionInfoFile)) {
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
        public Session GetItemById(Guid sessionId)
        {
            return GetItemById(sessionId.ToString());
        }
    }
}
