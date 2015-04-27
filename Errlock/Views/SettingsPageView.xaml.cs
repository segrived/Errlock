using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class SettingsPageView
    {
        private SettingsPageViewModel _viewModel;

        public SettingsPageView()
        {
            InitializeComponent();
            this._viewModel = new SettingsPageViewModel();
            this.DataContext = this._viewModel;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
