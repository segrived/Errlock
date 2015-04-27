using System.Threading;
using System.Windows;
using Errlock.Lib.Helpers;
using Errlock.Lib.Modules.PasswordCracker;
using Errlock.Lib.SmartWebRequest;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class PasswordCrackerPageView
    {
        private readonly PasswordCrackerPageViewModel _viewModel
            = new PasswordCrackerPageViewModel();

        private PasswordCracker _module;
        private CancellationTokenSource _token;

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

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            this._token = new CancellationTokenSource();
            var config = _viewModel.Config;
            this._module = new PasswordCracker(config, App.Config.Model.ConnectionConfiguration);
            ((MainWindow)Application.Current.MainWindow).StartModule(this._module);
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this._module != null) {
                this._module.Stop();
            }
        }
    }
}