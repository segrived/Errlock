using System.Windows;
using Errlock.Lib.Modules.PublicFinderModule;
using Errlock.Locators;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class PublicFinderView
    {
        private readonly PublicFinderViewModel _viewModel;

        private readonly ViewModelLocator _locator = new ViewModelLocator();

        public PublicFinderView()
        {
            InitializeComponent();
            _viewModel = new PublicFinderViewModel();
            this.DataContext = _viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _locator.MainWindowVM.SelectedModule =
                () => new PublicFinder(_viewModel.Config, App.Config.Model.ConnectionConfiguration);
        }
    }
}