using System.Windows;
using System.Windows.Controls;
using Errlock.Lib.Modules.ConfigurationTestModule;
using Errlock.Locators;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class ConfigurationTestView : Page
    {
        private ConfigurationTestViewModel _viewModel;

        private readonly ViewModelLocator _locator = new ViewModelLocator();

        public ConfigurationTestView()
        {
            InitializeComponent();
            _viewModel = new ConfigurationTestViewModel();
            this.DataContext = _viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _locator.MainWindowVM.SelectedModule =
                () => new ConfigurationTest(_viewModel.Config, App.Config.Model.ConnectionConfiguration);
        }
    }
}
