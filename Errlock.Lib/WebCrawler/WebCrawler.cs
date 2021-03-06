﻿using System;
using System.Collections.Generic;
using System.Linq;
using CsQuery;
using Errlock.Lib.RequestWrapper;
using Errlock.Lib.Sessions;

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

        /// <summary>
        /// Собирает ссылки с указанного URL
        /// </summary>
        /// <param name="url">Адрес страницы, с которой необходимо собрать все ссылки</param>
        /// <returns>Коллекция ссылок</returns>
        private HashSet<string> FetchLinks(string url)
        {
            try {
                var parser = new WebRequestWrapper(this.Options, url);
                using (var webParserResult = parser.GetRequest()) {

                    // Парсинг только HTML-страниц, игнорируя все остальное
                    if (!webParserResult.IsHtmlPage()) {
                        return new HashSet<string>();
                    }
                    string rawContent = webParserResult.Download();
                    var dom = new CQ(rawContent);

                    // Все ссылки, найденные на странице
                    var links = dom["a"].Select(l => l.GetAttribute("href"));

                    // Ингорировать якори
                    if (this.Session.Options.IngoreAnchors) {
                        links = links.Select(x => x.RemoveAnchors());
                    }

                    links = links.MakeAbsoluteBatch(this.Session.Url)
                                 .Where(l => new Uri(l).Host == new Uri(this.Session.Url).Host);

                    // Использовать случайные ссылки вместо последовательных
                    if (this.Session.Options.UseRandomLinks) {
                        links = links.Distinct();
                    }
                    return links.Take(this.Session.Options.FetchPerPage)
                                .SkipExceptions()
                                .ToHashSet();
                }
            } catch {
                return new HashSet<string>();
            }
        }

        private IEnumerable<string> EnumerateLinks(string url, int currentDepth)
        {
            if (currentDepth == 0
                || AnalysedUrls.Contains(url)
                || this.FoundedUrls.Count > this.Session.Options.MaxLinks
                ) {
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