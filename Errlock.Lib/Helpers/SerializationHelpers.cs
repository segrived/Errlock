using System.IO;
using YamlDotNet.Serialization;

namespace Errlock.Lib.Helpers
{
    /// <summary>
    /// Впомогательные функции для сериализиации/десериализации данных
    /// </summary>
    public static class SerializationHelpers
    {
        /// <summary>
        /// Выполняет сериализацию объекта в файл формата YAML
        /// </summary>
        /// <typeparam name="T">Тип сериализируемого объекта</typeparam>
        /// <param name="fileName">Имя файла</param>
        /// <param name="obj">Сериализируемый объект</param>
        public static void Serialize<T>(string fileName, T obj)
        {
            var serializer = new Serializer(SerializationOptions.DisableAliases);
            using (var writer = new StreamWriter(fileName)) {
                serializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        /// Выполняет десериализацию объекта из файла формата YAML
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Экземпляр десериализированного объекта типа T</returns>
        public static T Deserialize<T>(string fileName)
        {
            var deserializer = new Deserializer(ignoreUnmatched: true);
            using (var reader = new StreamReader(fileName)) {
                var instance = deserializer.Deserialize<T>(reader);
                return instance;
            }
        }
    }
}