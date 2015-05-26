using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Errlock.Lib.Helpers;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules.PublicFinderModule.Notices;
using Errlock.Lib.Sessions;
using Errlock.Lib.SmartWebRequest;
using Errlock.Resources.ModulesData;

namespace Errlock.Lib.Modules.PublicFinderModule
{
    public class PublicFinder : Module<PublicFinderConfig>
    {
        public override bool IsSupportProgressReporting
        {
            get { return true; }
        }

        private List<string> Urls { get; set; }

        private static readonly PublicFinderConfig DefaultConfig = new PublicFinderConfig {
            UsePermutations = true,
            DetectSuspicious = true,
            UseGetRequests = false,
            UserWordsList = ""
        };

        public PublicFinder(
            PublicFinderConfig moduleConfig, ConnectionConfiguration connectionConfig)
            : base(moduleConfig, connectionConfig)
        {
        }

        public PublicFinder(ConnectionConfiguration connectionConfig)
            : base(DefaultConfig, connectionConfig)
        { }

        protected override void ProcessConfig()
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
                    var permutations = EnumerableHelpers
                        .StringCartesianProduct(data0, data1, separator);
                    this.Urls.AddRange(permutations);
                }
            }
            if (! String.IsNullOrEmpty(this.Config.UserWordsList)) {
                var lines = this.Config.UserWordsList
                    .Lines()
                    .Where(line => ! string.IsNullOrEmpty(line));
                this.Urls.AddRange(lines);
            }
        }

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            int currentProgress = 0;
            const string format = "Сканирование начато, будет просканировано {0} вариантов";
            AddMessage(string.Format(format, this.Urls.Count), LoggerMessageType.Info);
            // Если пользоваль выбора отправу GET запросов вместо HEAD
            for (int i = 0; i < this.Urls.Count; i++) {
                if (this.Token.IsCancellationRequested) {
                    return ModuleScanStatus.Canceled;
                }
                try {
                    string urlPart = this.Urls[i];
                    string url = new Uri(new Uri(session.Url), urlPart).AbsoluteUri;
                    string requestType = this.Config.UseGetRequests ? "GET" : "HEAD";
                    var requester = new SmartRequest(this.ConnectionConfiguration, url);
                    using (var result = requester.Request(requestType)) {
                        string message = String.Format("[{0}%] | [{1}] {2}", currentProgress,
                            (int)result.StatusCode, url);
                        AddMessage(message, LoggerMessageType.Info);

                        // Обнаружение страниц с 403 кодом ошибки
                        if (this.Config.DetectSuspicious &&
                            result.StatusCode == HttpStatusCode.Forbidden) {
                            var notice = new SuspiciousUrl403Notice(url);
                            this.AddNotice(notice);
                        }

                        // Обнаружение страинц с 401 кодом ошибки
                        if (this.Config.DetectSuspicious &&
                            result.StatusCode == HttpStatusCode.Unauthorized) {
                            string header = result.Headers["WWW-Authenticate"];
                            var notice = new SuspiciousUrl401Notice(url, header);
                            this.AddNotice(notice);
                        }

                        // Обнаржение страниц, которые были уцспешно загружены
                        if (result.StatusCode == HttpStatusCode.OK) {
                            var notice = new OpenResourceNotice(url);
                            this.AddNotice(notice);
                        }
                    }
                } catch (Exception) {
                    string message = string.Format("Ошибка - URL: {0}", this.Urls[i]);
                    AddMessage(message, LoggerMessageType.Error);
                }

                int percentProgress = (int)((double)i / this.Urls.Count * 100.0);
                if (percentProgress > currentProgress) {
                    progress.Report(percentProgress);
                    currentProgress = percentProgress;
                }
            }
            return ModuleScanStatus.Completed;
        }
    }
}