using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Errlock.Views
{
    public partial class HomeTabView : Page
    {
        public HomeTabView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}