using System;
using System.Net;
using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.Helpers
{
    /// <summary>
    /// Функции-хелперы для классов, работающих с HTTP-запросами
    /// </summary>
    public static class WebHelpers
    {
        /// <summary>
        /// Проверяет переданный URL на валидность
        /// </summary>
        /// <param name="url">Проверяемый URL</param>
        /// <returns>True - если URL валиден; иначе False</returns>
        public static bool IsValidUrl(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp
                           || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        /// <summary>
        /// Проверяет доступность указанного ресурса. Для проверки на указанный URL отправляется
        /// HEAD-запрос и в случае успешного ответа за указанное время, функция возвращает True
        /// </summary>
        /// <param name="url">URL ресурса, доступность которого необходимо проверить</param>
        /// <param name="timeout">Таймауд ожидания ответа</param>
        /// <returns>True - если ресурс дотупен; иначе - False</returns>
        public static bool IsOnline(string url, int timeout = 3000)
        {
            var request = WebRequest.CreateHttp(url);
            request.Timeout = timeout;
            // запрет редиректов, для проверки доступности они не нужны
            request.AllowAutoRedirect = false;
            request.Method = "HEAD";
            try {
                using (request.GetResponse()) {
                    return true;
                }
            } catch (WebException) {
                return false;
            }
        }

        /// <summary>
        /// Преобразовывает строку в экземпляр перечисления RequestMethod. Если строка не
        /// совместима ни с одним из значений перечисление - возвращается Get
        /// </summary>
        /// <param name="method">Преобразовываемая строка</param>
        /// <returns>Элемент перечисления RequestMethod</returns>
        public static RequestMethod ToRequestMethod(string method)
        {
            string res = method.Trim().ToUpperFirstChar();
            try {
                return (RequestMethod)Enum.Parse(typeof(RequestMethod), res);
            } catch(ArgumentException) {
                return RequestMethod.Get;
            }
        }
    }
}