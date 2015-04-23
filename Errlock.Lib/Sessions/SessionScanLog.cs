using Errlock.Lib.Modules;

namespace Errlock.Lib.Sessions
{
    public class SessionScanLog
    {
        public string Module { get; set; }
        public ModuleScanResult ScanResult { get; set; }
    }
}