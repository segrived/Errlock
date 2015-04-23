using System.Windows;
using Errlock.Lib.Sessions;

namespace Errlock.Views
{
    public partial class NewSession
    {

        public NewSession()
        {
            InitializeComponent();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var session = this.DataContext as Session;
            if (session != null) {
                session.Save();
            }
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
