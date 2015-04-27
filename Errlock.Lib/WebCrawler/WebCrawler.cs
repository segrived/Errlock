using System;
using System.Collections.Generic;
using System.Linq;
using CsQuery;
using Errlock.Lib.Sessions;
using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.WebCrawler
{
    internal class WebCrawler
    {
        private Session Session { get; set; }
        private ConnectionConfiguration Options { get; set; }
        private HashSet<string> AnalysedUrls { get; set; }
        private HashSet<string> FoundedUrls { get; set; }

        public WebCrawler(Session session, ConnectionConfiguration options)
        {
            this.Session = session;
            this.Options = options;

            this.AnalysedUrls = new HashSet<string>();
            this.FoundedUrls = new HashSet<string>();
        }

        public IEnumerable<string> EnumerateLinks()
        {
            string url = this.Session.Url;
            return this.EnumerateLinks(url, this.Session.Options.RecursionDepth);
        }

        private HashSet<string> FetchLinks(string url)
        {
            var parser = new SmartWebRequest.SmartWebRequest(this.Options, url);
            try {
                var webParserResult = parser.GetRequest();
                if (! webParserResult.IsHtmlPage()) {
                    webParserResult.Dispose();
                    return new HashSet<string>();
                }
                string rawContent = webParserResult.Download();
                var dom = new CQ(rawContent);
                webParserResult.Dispose();
                var links = dom["a"]
                    .Select(l => l.GetAttribute("href").RemoveAnchors())
                    .MakeAbsoluteBatch(this.Session.Url)
                    .Where(l => new Uri(l).Host == new Uri(this.Session.Url).Host)
                    .Distinct()
                    .Take(this.Session.Options.FetchPerPage)
                    .SkipExceptions()
                    .ToHashSet();
                return links;
            } catch {
                return new HashSet<string>();
            }
        }

        private IEnumerable<string> EnumerateLinks(string url, int currentDepth)
        {
            if (currentDepth == 0 || AnalysedUrls.Contains(url)) {
                yield break;
            }
            if (this.FoundedUrls.Count > this.Session.Options.MaxLinks) {
                yield break;
            }

            var links = FetchLinks(url);

            AnalysedUrls.Add(url);
            foreach (string link in links) {
                if (this.FoundedUrls.Count > this.Session.Options.MaxLinks) {
                    yield break;
                }
                if (FoundedUrls.Contains(link)) {
                    continue;
                }
                Console.WriteLine(@"{0} {1}", currentDepth, link);
                FoundedUrls.Add(link);
                yield return link;
            }
            foreach (string link in links) {
                var fetched = this.EnumerateLinks(link, currentDepth - 1);
                foreach (string anotherUrl in fetched) {
                    yield return anotherUrl;
                }
            }
        }
    }
}