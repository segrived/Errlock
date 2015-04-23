using System;
using System.IO;
using System.Reflection;

namespace Errlock.Lib.Helpers
{
    /// <summary>
    /// Общие вспомогательные функции, константы и свойства
    /// </summary>
    public static class AppHelpers
    {
        /// <summary>
        /// Название проекта
        /// </summary>
        public const string ProjectName = "Errlock";

        /// <summary>
        /// Путь к специальной директории Application Data
        /// </summary>
        private static readonly string AppData =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Путь к директории с настройками по умолчанию
        /// </summary>
        public static readonly string DefaultConfigPath = Path.Combine(AppData, ProjectName);

        /// <summary>
        /// Путь к директории исполняемого файла
        /// </summary>
        public static readonly string ExecAssemblyDir =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}