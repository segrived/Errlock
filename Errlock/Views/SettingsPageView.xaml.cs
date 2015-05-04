using System.Windows;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class SettingsPageView
    {
        private readonly SettingsPageViewModel _viewModel;

        public SettingsPageView()
        {
            InitializeComponent();
            this._viewModel = new SettingsPageViewModel();
            this.DataContext = this._viewModel;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            App.Config.Save(_viewModel.ConfigModel);
        }
    }
}
