using System;
using System.Collections.Generic;
using System.Net;
using Errlock.Lib.Helpers;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules.PublicFinder.Notices;
using Errlock.Lib.Sessions;
using Errlock.Lib.SmartWebRequest;
using Errlock.Resources.ModulesData;

namespace Errlock.Lib.Modules.PublicFinder
{
    public class PublicFinder : Module<PublicFinderConfig>
    {
        public override bool IsSupportProgressReporting
        {
            get { return true; }
        }

        private List<string> Urls { get; set; }

        public PublicFinder(
            PublicFinderConfig moduleConfig, ConnectionConfiguration connectionConfig)
            : base(moduleConfig, connectionConfig)
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
            if (this.ModuleConfig.UsePermutations) {
                foreach (string separator in separators) {
                    var permutations = WebHelpers.Permutations(data0, data1, separator);
                    this.Urls.AddRange(permutations);
                }
            }
        }

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            const string format = "Сканирование начато, будет просканировано {0} вариантов";
            this.Logger.Log(string.Format(format, this.Urls.Count), LoggerMessageType.Info);
            // Если пользоваль выбора отправу GET запросов вместо HEAD
            for (int i = 0; i < this.Urls.Count; i++) {
                if (this.Token.IsCancellationRequested) {
                    return ModuleScanStatus.Canceled;
                }
                try {
                    string urlPart = this.Urls[i];
                    string url = new Uri(new Uri(session.Url), urlPart).AbsoluteUri;
                    string requestType = this.ModuleConfig.UseGetRequests ? "GET" : "HEAD";
                    using (var result = new SmartWebRequest.SmartWebRequest(this.ConnectionConfiguration, url).Request(requestType)) {
                        AddMessage(
                            string.Format("Обработка [{2} из {3}] | [{0}] {1} ",
                                (int)result.StatusCode, url, i + 1, this.Urls.Count),
                            LoggerMessageType.Info);
                        if (this.ModuleConfig.DetectSuspicious &&
                            result.StatusCode == HttpStatusCode.Forbidden) {
                            var notice = new SuspiciousUrl403Notice(session, url);
                            this.AddNotice(notice);
                        }
                        if (this.ModuleConfig.DetectSuspicious &&
                            result.StatusCode == HttpStatusCode.Unauthorized) {
                            string header = result.Headers["WWW-Authenticate"];
                            var notice = new SuspiciousUrl401Notice(session, url, header);
                            this.AddNotice(notice);
                        }
                        if (result.StatusCode == HttpStatusCode.OK) {
                            var notice = new OpenResourceNotice(session, url);
                            this.AddNotice(notice);
                        }
                    }
                } catch (Exception ex) {
                    string message = string.Format("Ошибка - URL: {0}", this.Urls[i]);
                    AddMessage(message, LoggerMessageType.Error);
                }
                progress.Report((int)((double)i / this.Urls.Count * 100.0));
            }
            return ModuleScanStatus.Completed;
        }
    }
}