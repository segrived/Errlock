using System;
using System.Collections.Generic;
using CsQuery;
using Errlock.Lib.Helpers;
using Errlock.Lib.Logger;
using Errlock.Lib.Sessions;
using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.Modules.XssScanner
{
    public class XssScanner : Module<XssScannerConfig>
    {
        public XssScanner(XssScannerConfig moduleConfig, ConnectionConfiguration connectionConfig) : base(moduleConfig, connectionConfig) { }

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
                    var request = new SmartWebRequest.SmartWebRequest(ConnectionConfiguration, link);
                    using (var getReq = request.GetRequest()) {
                        if (! getReq.IsHtmlPage()) {
                            continue;
                        }
                        var domTree = new CQ(getReq.Download());
                        var forms = domTree["form"];
                        foreach (var form in forms) {
                            string action = form.GetAttribute("action").MakeUrlAbsolute(session.Url);
                            string method = form.GetAttribute("method");
                            var requestType = WebHelpers.ToRequestMethod(method);
                            var webForm = new WebForm(action, requestType);

                            if (_webForms.Contains(webForm)) {
                                continue;
                            }
                            //form.ChildElements.Where(e => e.NodeName == "INPUT");
                            //form.ChildElements.Where(e => e.NodeName == "SELECT");

                            _webForms.Add(webForm);
                            var message = String.Format("Найдена новая форма: {0}", webForm);
                            AddMessage(message, LoggerMessageType.Info);
                        }
                    }
                } catch (Exception ex) {
                    AddMessage(String.Format("Ошибка при парсинге URL {0}", link), LoggerMessageType.Error);
                }
                AddMessage(link, LoggerMessageType.Info);
            }
            return ModuleScanStatus.Completed;
        }
    }
}
