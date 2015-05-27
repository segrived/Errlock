using System;
using System.Windows;
using Errlock.Lib.Helpers;
using Errlock.Lib.Modules.PasswordCrackerModule;
using Errlock.Lib.RequestWrapper;
using Errlock.Locators;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class PasswordCrackerPageView
    {
        private readonly ViewModelLocator _locator = new ViewModelLocator();

        private readonly PasswordCrackerPageViewModel _viewModel;

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

            _locator.MainWindowVM.SelectedModule =
                () => new PasswordCracker(_viewModel.Config, App.Config.Model.ConnectionConfiguration);
        }
    }
}