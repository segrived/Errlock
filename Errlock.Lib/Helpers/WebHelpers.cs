using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

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
            try {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 3000;
                request.AllowAutoRedirect = false;
                request.Method = "HEAD";
                var response = request.GetResponse();
                response.Dispose();
                return true;
            } catch (WebException) {
                return false;
            }
        }
    }
}