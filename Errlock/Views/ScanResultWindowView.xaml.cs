using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Errlock.ViewModels;

namespace Errlock.Views
{
    /// <summary>
    /// Interaction logic for ScanResultWindowView.xaml
    /// </summary>
    public partial class ScanResultWindowView : Window
    {
        public readonly ScanResultWindowViewModel viewModel;

        public ScanResultWindowView()
        {
            InitializeComponent();
            this.viewModel = new ScanResultWindowViewModel();
            this.DataContext = this.viewModel;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }
    }
}