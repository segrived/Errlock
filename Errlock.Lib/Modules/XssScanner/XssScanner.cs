using System;
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

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            var crawler = new WebCrawler.WebCrawler(session, this.ConnectionConfiguration);
            foreach (var link in crawler.EnumerateLinks()) {
                if (Token.IsCancellationRequested) {
                    return ModuleScanStatus.Canceled;
                }
                this.Logger.Log(link, LoggerMessageType.Info);
            }
            return ModuleScanStatus.Completed;
        }
    }
}
