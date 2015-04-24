using System;
using System.Linq;
using Errlock.Lib.Modules;

namespace Errlock.Lib.Sessions
{
    public class SessionScanLog
    {
        public string Module { get; set; }
        public ModuleScanResult ScanResult { get; set; }

        public override string ToString()
        {
            string newLine = Environment.NewLine;
            var notices = ScanResult.Notices.Select(notice => {
                string url = String.Format("URL: {0}", notice.LinkedUrl);
                string message = String.Format("Сообщение: {0}", notice.Text);
                string priority = String.Format("Важность: {0}", notice.Priority.GetDescription());
                string description = String.Format("Описание: {0}", notice.Information);
                return String.Join(newLine, url, message, priority, description);
            });
            string noticesStr = String.Join(Environment.NewLine.Repeat(2), notices);
            string logMessagesStr = String.Join(Environment.NewLine, ScanResult.LogMessages);
            return noticesStr + Environment.NewLine.Repeat(3) + logMessagesStr;
        }
    }
}