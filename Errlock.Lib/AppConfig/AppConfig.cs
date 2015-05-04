using System.IO;
using Errlock.Lib.Helpers;

namespace Errlock.Lib.AppConfig
{
    /// <summary>
    /// Предоставляет интерфейс к загрузке и сохранению настроек приложения
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Имя файла с настройками
        /// </summary>
        private const string ConfigFileName = "config.yml";

        /// <summary>
        /// Полный путь к файлу с настройками
        /// </summary>
        private readonly string _configFilePath =
            Path.Combine(AppHelpers.DefaultConfigPath, ConfigFileName);

        public ErrlockConfigModel Model { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса AppConfig
        /// </summary>
        /// <param name="defaults">
        /// Экземпляр с настройками по умолчанию;
        /// используется, если файл конфигурации ещё не создан
        /// </param>
        public AppConfig(ErrlockConfigModel defaults)
        {
            // Если конфигурационный файл ещё не существует - создаем его
            if (! File.Exists(_configFilePath)) {
                this.Model = this.RecreateConfigFile(defaults);
                return;
            }
            try {
                this.Model =
                    SerializationHelpers.Deserialize<ErrlockConfigModel>(_configFilePath) ??
                    this.RecreateConfigFile(defaults);
            } catch {
                this.Model = this.RecreateConfigFile(defaults);
            }
        }

        private ErrlockConfigModel RecreateConfigFile(ErrlockConfigModel defaults)
        {
            var fileInfo = new FileInfo(_configFilePath);
            Directory.CreateDirectory(fileInfo.DirectoryName);
            File.Create(_configFilePath).Dispose();
            // Сохраняет настройки по умолчанию в новый файл
            this.Model = defaults;
            this.Save();
            return this.Model;
        }

        /// <summary>
        /// Сохраняет настройки обратно в файл
        /// </summary>
        public void Save()
        {
            SerializationHelpers.Serialize(_configFilePath, this.Model);
        }

        public void Save(ErrlockConfigModel model)
        {
            this.Model = model;
            this.Save();
        }
    }
}