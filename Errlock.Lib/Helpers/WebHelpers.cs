using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.Helpers
{
    public static class WebHelpers
    {
        public static bool IsValidUrl(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp
                              || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        public static IEnumerable<string> Permutations(
            IEnumerable<string> input1, IEnumerable<string> input2, string sep)
        {
            var enumerable = input2.ToList();
            foreach (string a in input1) {
                foreach (string b in enumerable) {
                    yield return string.Join(sep, a, b);
                    yield return string.Join(sep, b, a);
                }
            }
        }

        public static bool IsOnline(string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.Timeout = 3000;
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

        public static RequestMethod ToRequestMethod(string method)
        {
            string res = method.Trim().ToUpperFirstChar();
            try {
                return (RequestMethod)Enum.Parse(typeof(RequestMethod), res);
            } catch {
                return RequestMethod.Get;
            }
        }
    }
}