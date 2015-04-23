using System;
using System.Collections.Generic;
using Errlock.Lib.Helpers;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules.PublicFinder.Notices;
using Errlock.Lib.Sessions;
using Errlock.Lib.WebParser;
using Errlock.Resources.ModulesData;

namespace Errlock.Lib.Modules.PublicFinder
{
    public class PublicFinder : Module<PublicFinderConfig>
    {
        private List<string> Urls { get; set; }

        public PublicFinder(PublicFinderConfig config)
            : base(config)
        {
            this.Urls = new List<string>();
            // В первых двух ресурсах содержатся части URL, которые, при использовании
            // перестановок, генерируют набор URL для тестирования
            var data0 = PublicFinderData.Data0.Lines();
            this.Urls.AddRange(data0);
            var data1 = PublicFinderData.Data1.Lines();
            this.Urls.AddRange(data1);
            // URL-части, в которые нет необходимости использовать перестановки
            var completed = PublicFinderData.Full.Lines();
            // Разделители, которые используется для перестановок
            var separators = PublicFinderData.Separators.Lines();
            this.Urls.AddRange(completed);
            if (this.Config.UsePermutations) {
                foreach (string separator in separators) {
                    var permutations = WebHelpers.Permutations(data0, data1, separator);
                    this.Urls.AddRange(permutations);
                }
            }
        }

        protected override ModuleScanStatus Process(Session session)
        {
            const string format = "Сканирование начато, будет просканировано {0} вариантов";
            this.Logger.Log(string.Format(format, this.Urls.Count), LoggerMessageType.Info);
            var options = new WebParserOptions { Method = "HEAD" };
            // Если пользоваль выбора отправу GET запросов вместо HEAD
            if (this.Config.UseGetRequests) {
                options.Method = "GET";
            }
            var parser = new WebParser.Parser(options);
            for (int i = 0; i < this.Urls.Count; i++) {
                if (this.Token.IsCancellationRequested) {
                    return ModuleScanStatus.Canceled;
                }
                try {
                    string urlPart = this.Urls[i];
                    string url = new Uri(new Uri(session.Url), urlPart).AbsoluteUri;
                    using (var result = parser.Process(url)) {
                        AddMessage(
                            string.Format("Обработка [{2} из {3}] | [{0}] {1} ",
                                result.Status, url, i + 1, this.Urls.Count), LoggerMessageType.Info);
                        if (this.Config.DetectSuspicious && result.Status == 403) {
                            var notice = new SuspiciousUrl403Notice(session, url);
                            this.AddNotice(notice);
                        }
                        if (this.Config.DetectSuspicious && result.Status == 401) {
                            string header = result.Headers["WWW-Authenticate"];
                            var notice = new SuspiciousUrl401Notice(session, url, header);
                            this.AddNotice(notice);
                        }
                        if (result.Status == 200) {
                            var notice = new OpenResourceNotice(session, url);
                            this.AddNotice(notice);
                        }
                    }
                } catch (Exception ex) {
                    string message = string.Format("Исключительная ситуация.\nURL: {0}",
                        this.Urls[i]);
                    AddMessage(message, LoggerMessageType.Error);
                }
            }
            return ModuleScanStatus.Completed;
        }
    }
}