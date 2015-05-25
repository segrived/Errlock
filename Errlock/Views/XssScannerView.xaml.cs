using System.Windows;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class XssScannerView
    {
        XssScannerViewModel _viewModel;

        public XssScannerView()
        {
            InitializeComponent();
            this.DataContext = _viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new XssScannerViewModel();
            ((MainWindow)Application.Current.MainWindow).CurrentModule = _viewModel.Module;
        }
    }
}