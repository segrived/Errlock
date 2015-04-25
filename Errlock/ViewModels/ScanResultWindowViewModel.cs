using Errlock.Lib;
using Errlock.Lib.Modules;

namespace Errlock.ViewModels
{
    public class ScanResultWindowViewModel : Bindable
    {
        public ModuleScanResult ScanResult { get; set; }

        public ScanResultWindowViewModel()
        {
        }
    }
}