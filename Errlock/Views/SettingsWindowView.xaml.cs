using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class SettingsWindow
    {
        readonly SettingsWindowViewModel _viewModel = new SettingsWindowViewModel();

        public SettingsWindow()
        {
            this.DataContext = _viewModel;
            InitializeComponent();
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.Config.Model = _viewModel.ConfigModel;
            App.Config.Save();
            this.Close();
        }


    }
}