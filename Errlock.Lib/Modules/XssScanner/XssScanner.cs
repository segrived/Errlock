using System;
using System.Collections.Generic;
using CsQuery;
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
                
                var request = new SmartWebRequest.SmartWebRequest(ConnectionConfiguration, link);
                using (var getReq = request.GetRequest()) {
                    if (!getReq.IsHtmlPage()) {
                        continue;
                    }
                    var domTree = new CQ(getReq.Download());
                    var forms = domTree["form"];
                    foreach (var form in forms) {
                        var action = form.GetAttribute("action").MakeUrlAbsolute(session.Url);
                        var method = form.GetAttribute("method");
                        var requestType = default(RequestType);
                        switch (method.Trim().ToUpper()) {
                            case "GET":
                                requestType = RequestType.Get;
                                break;
                            case "POST":
                                requestType = RequestType.Post;
                                break;
                            default:
                                requestType = RequestType.Get;
                                break;
                        }
                        var webForm = new WebForm(action, requestType);
                        if (! _webForms.Contains(webForm)) {
                            _webForms.Add(webForm);
                            var message = String.Format("Найдена новая форма: {0}", webForm);
                            AddMessage(message, LoggerMessageType.Info);
                        }
                    }
                }
                AddMessage(link, LoggerMessageType.Info);
            }
            return ModuleScanStatus.Completed;
        }
    }
}
