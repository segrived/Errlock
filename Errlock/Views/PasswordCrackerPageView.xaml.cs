using System.Windows;
using Errlock.Lib.Helpers;
using Errlock.Lib.Modules.PasswordCrackerModule;
using Errlock.Lib.RequestWrapper;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class PasswordCrackerPageView
    {
        private PasswordCrackerPageViewModel _viewModel;

        public PasswordCrackerPageView()
        {
            InitializeComponent();
            _viewModel = new PasswordCrackerPageViewModel();
            this.DataContext = _viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.RequestTypeComboBox.ItemsSource = EnumHelpers.EnumToDictionary<RequestMethod>();
            this.InvalidPasswordBehaviorComboBox.ItemsSource =
                EnumHelpers.EnumToDictionary<InvalidPasswordAction>();
            _viewModel = new PasswordCrackerPageViewModel();
            ((MainWindow)Application.Current.MainWindow).CurrentModule = _viewModel.Module;
        }
    }
}