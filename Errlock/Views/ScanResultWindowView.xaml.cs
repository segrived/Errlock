using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Errlock.Lib.Modules;
using Errlock.ViewModels;

namespace Errlock.Views
{
    /// <summary>
    /// Interaction logic for ScanResultWindowView.xaml
    /// </summary>
    public partial class ScanResultWindowView : Window
    {
        private readonly ScanResultWindowViewModel _viewModel;

        public ScanResultWindowView()
        {
            InitializeComponent();
            this._viewModel = new ScanResultWindowViewModel();
            this.DataContext = this._viewModel;
        }

        public ScanResultWindowView(ModuleScanResult scanResult) : this()
        {
            this._viewModel.ScanResult = scanResult;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }
    }
}