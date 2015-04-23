﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Errlock.Lib
{
    /// <summary>
    /// Расширения, используемые в библиотеке
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Удаляет указанную подстоку из конца строки, если таковая имеется
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <param name="value">Подстрока, которую необходимо обрезать</param>
        /// <returns>Новая строка с обрезанной подстрокой в конце</returns>
        public static string TrimEnd(this string source, string value)
        {
            if (! source.EndsWith(value)) {
                return source;
            }
            int lastIndex = source.LastIndexOf(value, StringComparison.Ordinal);
            return source.Remove(lastIndex);
        }

        /// <summary>
        /// Проверяет, явлеется ли путь к URL абсолютным
        /// </summary>
        /// <param name="url">Проверяемый URL адрес</param>
        /// <returns>Возвращает True если пусть к URL абсолютный; иначе возвращает False</returns>
        public static bool IsAbsoluteUrl(this string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        /// <summary>
        /// Генерирует абсолютный путь к URL; если путь уже абсолютный - возвращает
        /// значение как есть
        /// </summary>
        /// <param name="url">Исходный URL</param>
        /// <param name="baseUrl">Адрес ресурса</param>
        /// <returns>Абсолютный путь к URL</returns>
        public static string MakeUrlAbsolute(this string url, string baseUrl)
        {
            if (IsAbsoluteUrl(url)) {
                return url;
            }
            var baseUri = new Uri(baseUrl);
            return new Uri(baseUri, url).AbsoluteUri;
        }

        /// <summary>
        /// Делает URL абсолютнымы для всей указанной коллекции; абсолютные URL при этом пропускаются
        /// </summary>
        /// <param name="inputUrls">Исходный набор URL</param>
        /// <param name="baseUrl">Базовый хост</param>
        /// <returns>Коллекция с абсолютными URL</returns>
        public static IEnumerable<string> MakeAbsoluteBatch(
            this IEnumerable<string> inputUrls,
            string baseUrl)
        {
            var urls = inputUrls
                .Select(l => l.MakeUrlAbsolute(baseUrl))
                .SkipExceptions();
            return urls;
        }

        /// <summary>
        /// Удаляет значение "якоря" из URL (если он присутствует), включая сам якорь
        /// </summary>
        /// <param name="url">Исходный URL</param>
        /// <returns>URL с обрезанным якорем</returns>
        public static string RemoveAnchors(this string url)
        {
            int index = url.LastIndexOf("#", StringComparison.Ordinal);
            return index == -1 ? url : url.Substring(0, index);
        }

        /// <summary>
        /// Пропускает элементы коллекции, вычисление значения которых выбрасывает исключение
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="input">Исходная коллекция</param>
        /// <returns>Коллекция с пропущенными элементами</returns>
        public static IEnumerable<T> SkipExceptions<T>(this IEnumerable<T> input)
        {
            using (var enumerator = input.GetEnumerator()) {
                bool next = true;
                while (next) {
                    try {
                        next = enumerator.MoveNext();
                    } catch {
                        continue;
                    }
                    if (next) {
                        yield return enumerator.Current;
                    }
                }
            }
        }

        /// <summary>
        /// Создает хэш-таблицу на основе IEnumerable
        /// </summary>
        /// <typeparam name="T">Тип данных элементов коллекции</typeparam>
        /// <param name="source">Источник</param>
        /// <returns>Созданная хэш-таблица</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        /// <summary>
        /// Проверяет ответ веб-сервера и определяет, является ли URL, указанный в
        /// запросе, HTML либо XML страницей
        /// </summary>
        /// <param name="response">Ответ веб-сервера</param>
        /// <returns>True - если это HTML либо XML данные; иначе - False</returns>
        public static bool IsHtmlPage(this WebResponse response)
        {
            string type = response.ContentType;
            return type.StartsWith("text/html")
                   || type.StartsWith("text/xml")
                   || type.StartsWith("application/xhtml+xml")
                   || type.StartsWith("application/xml");
        }

        /// <summary>
        /// Загружает данные по указанному URL в строку
        /// </summary>
        /// <param name="response">Ответ веб-сервера</param>
        /// <returns>Загруженные в строку данные</returns>
        public static string Download(this WebResponse response)
        {
            var stream = response.GetResponseStream();
            using (var reader = new StreamReader(stream)) {
                string content = reader.ReadToEnd();
                return content;
            }
        }

        public static string[] Lines(this string source)
        {
            return source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        public static int ToUnixTimestamp(this DateTime value)
        {
            return
                (int)
                Math.Truncate(
                    (value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        public static void Raise<TEventArgs>(
            this EventHandler<TEventArgs> handler, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (handler != null) {
                handler(sender, e);
            }
        }

        public static void Raise(this EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null) {
                handler(sender, e);
            }
        }
    }
}