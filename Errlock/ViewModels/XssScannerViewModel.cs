using Errlock.Lib.Modules.XssScannerModule;

namespace Errlock.ViewModels
{
    public class XssScannerViewModel : BaseModuleViewModel<XssScanner, XssScannerConfig>
    {
        public XssScannerViewModel()
            : base(new XssScanner(App.Config.Model.ConnectionConfiguration))
        {  }
    }
}
