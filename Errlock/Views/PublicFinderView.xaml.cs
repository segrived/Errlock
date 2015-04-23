using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Errlock.Lib.Logger;
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
        private CancellationTokenSource _token;
        private IModule _module;

        public PublicFinderView()
        {
            InitializeComponent();
            this._viewModel = new PublicFinderViewModel();
            this.DataContext = _viewModel;
        }

        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            this._token = new CancellationTokenSource();
            var config = _viewModel.Config;
            this._module = new PublicFinder(config);
            ((MainWindow)Application.Current.MainWindow).StartModule(this._module);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this._token == null) {
                return;
            }
            this._token.Cancel();
            this._token.Dispose();
            this._token = null;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this._module != null) {
                this._module.Stop();
            }
        }
    }
}
