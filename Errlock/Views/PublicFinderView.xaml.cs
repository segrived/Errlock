using System.Threading;
using System.Windows;
using Errlock.Lib.Modules;
using Errlock.Lib.Modules.PublicFinder;
using Errlock.Locators;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class PublicFinderView
    {
        private readonly ViewModelLocator _locator = new ViewModelLocator();
        private readonly PublicFinderViewModel _viewModel;
        private IModule _module;
        private CancellationTokenSource _token;

        public PublicFinderView()
        {
            InitializeComponent();
            this._viewModel = new PublicFinderViewModel();
            this.DataContext = _viewModel;
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            this._token = new CancellationTokenSource();
            var config = _viewModel.Config;
            this._module = new PublicFinder(config, App.Config.Model.ConnectionConfiguration);
            ((MainWindow)Application.Current.MainWindow).StartModule(this._module);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) { }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this._module != null) {
                this._module.Stop();
            }
        }
    }
}