using System.Collections.Generic;
using Errlock.Lib;
using Errlock.Lib.Modules;

namespace Errlock.ViewModels
{
    public class ScanResultWindowViewModel : Bindable
    {
        public List<ModuleNotice> Notices { get; set; }
        public List<string> Messages { get; set; }
    }
}