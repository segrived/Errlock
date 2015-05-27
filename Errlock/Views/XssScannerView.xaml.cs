using System.Windows;
using Errlock.Lib.Modules.XssScannerModule;
using Errlock.Locators;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class XssScannerView
    {
        private readonly ViewModelLocator _locator = new ViewModelLocator();
        private readonly XssScannerViewModel _viewModel;

        public XssScannerView()
        {
            InitializeComponent();
            this._viewModel = new XssScannerViewModel();
            this.DataContext = _viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _locator.MainWindowVM.SelectedModule =
                () => new XssScanner(_viewModel.Config, App.Config.Model.ConnectionConfiguration);
        }
    }
}