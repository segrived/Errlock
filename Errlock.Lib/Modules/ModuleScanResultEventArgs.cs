using System;

namespace Errlock.Lib.Modules
{
    public class ModuleScanResultEventArgs : EventArgs
    {
        public ModuleScanResult ScanResult { get; private set; }

        public ModuleScanResultEventArgs(ModuleScanResult scanResult)
        {
            this.ScanResult = scanResult;
        }
    }
}