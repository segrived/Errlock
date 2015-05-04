using System.Windows;
using Errlock.Lib.Modules;
using Errlock.Lib.Modules.XssScanner;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class XssScannerView
    {
        XssScannerViewModel _viewModel = new XssScannerViewModel();
        private IModule _module;

        public XssScannerView()
        {
            InitializeComponent();
            this.DataContext = _viewModel;
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            var config = _viewModel.Config;
            this._module = new XssScanner(config, App.Config.Model.ConnectionConfiguration);
            ((MainWindow)Application.Current.MainWindow).StartModule(this._module);
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this._module != null) {
                this._module.Stop();
            }
        }
    }
}