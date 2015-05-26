using System;
using System.Collections.Generic;
using System.Linq;
using CsQuery;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules.XssScanner;
using Errlock.Lib.Modules.XssScannerModule.Notices;
using Errlock.Lib.RequestWrapper;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.XssScannerModule
{
    public class XssScanner : Module<XssScannerConfig>
    {
        private static readonly XssScannerConfig DefaultConfig = 
            new XssScannerConfig();

        public XssScanner(ConnectionConfiguration connectionConfig)
            : base(DefaultConfig, connectionConfig) { }

        public XssScanner(XssScannerConfig moduleConfig, ConnectionConfiguration connectionConfig) 
            : base(moduleConfig, connectionConfig) { }

        public override bool IsSupportProgressReporting
        {
            get { return false; }
        }

        private readonly HashSet<WebForm> _webForms = new HashSet<WebForm>(); 

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            var crawler = new WebCrawler.WebCrawler(session, this.ConnectionConfiguration);
            foreach (var link in crawler.EnumerateLinks()) {
                if (Token.IsCancellationRequested) {
                    return ModuleScanStatus.Canceled;
                }
                try {
                    var request = new WebRequestWrapper(ConnectionConfiguration, link);
                    using (var getReq = request.GetRequest()) {
                        if (! getReq.IsHtmlPage()) {
                            continue;
                        }
                        var domTree = new CQ(getReq.Download());
                        var webForms = domTree["form"]
                            .Select(form => new WebForm(form, session))
                            .Where(webForm => ! _webForms.Contains(webForm));
                        foreach (var webForm in webForms) {
                            _webForms.Add(webForm);
                            var query = webForm.GetQuery();
                            var xssReq = new WebRequestWrapper(ConnectionConfiguration, query);
                            using (var xssRes = xssReq.GetRequest()) {
                                var res = xssRes.Download();
                                if (webForm.HasInjection(res)) {
                                    var notice = new XssInjectionNotice(link, query);
                                    this.AddNotice(notice);
                                }
                            }
                            
                            AddMessage(query, LoggerMessageType.Info);
                            var message = String.Format("Найдена новая форма: {0}", webForm);
                            AddMessage(message, LoggerMessageType.Info);
                        }
                    }
                } catch (Exception) {
                    AddMessage(String.Format("Ошибка при парсинге URL {0}", link), LoggerMessageType.Error);
                }
                AddMessage(link, LoggerMessageType.Info);
            }
            return ModuleScanStatus.Completed;
        }
    }
}
