using System.Windows;
using System.Windows.Controls;
using Errlock.Lib.Helpers;
using Errlock.Lib.Modules.PasswordCracker;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class PasswordCrackerPageView : Page
    {
        private readonly PasswordCrackerPageViewModel _viewModel 
            = new PasswordCrackerPageViewModel();

        public PasswordCrackerPageView()
        {
            InitializeComponent();
            this.DataContext = _viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.RequestTypeComboBox.ItemsSource = EnumHelpers.EnumToDictionary<RequestType>();
            this.InvalidPasswordBehaviorComboBox.ItemsSource = 
                EnumHelpers.EnumToDictionary<InvalidPasswordAction>();
        }
    }
}
