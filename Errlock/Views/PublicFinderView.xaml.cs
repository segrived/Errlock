using System.Windows;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class PublicFinderView
    {
        private PublicFinderViewModel _viewModel;

        public PublicFinderView()
        {
            InitializeComponent();
            this.DataContext = _viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new PublicFinderViewModel();
            ((MainWindow)Application.Current.MainWindow).CurrentModule = _viewModel.Module;
        }
    }
}