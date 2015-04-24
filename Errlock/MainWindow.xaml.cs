using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules;
using Errlock.Lib.Sessions;
using Errlock.Locators;
using Errlock.Views;

namespace Errlock
{
    public partial class MainWindow : Window
    {
        private readonly ViewModelLocator locator = new ViewModelLocator();

        public MainWindow()
        {
            InitializeComponent();

            locator.MainWindowViewModel.Sessions = Session.EnumerateSessions();

            Session.SessionChanged += (sender, e) => {
                locator.MainWindowViewModel.Sessions = Session.EnumerateSessions();
            };

            App.Logger.NewMessage += (sender, e)
                => Dispatcher.Invoke(() => LogData.AppendText(e.FormattedMessage + "\n"));
        }

        public async void StartModule(IModule module)
        {
            module.SetLogger(App.Logger);
            var session = this.locator.MainWindowViewModel.SelectedSession;

            // При поступлении нового предупреждения от модуля
            module.NewNotice += (sender, e) => {
                App.Logger.Log(e.Notice.Text, LoggerMessageType.Warn);
            };
            
            this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            module.Progress.ProgressChanged += (sender, i) => {
                this.ModuleProgress.Value = i;
                this.TaskbarItemInfo.ProgressValue = (double)i / 100;
            };
            var scanResult = await Task.Factory.StartNew(() => {
                try {
                    return module.Start(session);
                } catch (WebException ex) {
                    App.Logger.Log(ex.Message, LoggerMessageType.Error);
                    return null;
                }
            });
            if (scanResult == null) {
                return;
            }
            if (scanResult.Status == ModuleScanStatus.Error) {
                this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            } else {
                this.TaskbarItemInfo.ProgressValue = 0;
                this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            }
            var win = new ScanResultWindowView {
                viewModel = { Notices = scanResult.Notices, Messages = scanResult.LogMessages }
            };
            win.ShowDialog();
        }

        private void SessionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var session = SessionList.SelectedItem as Session;
            locator.MainWindowViewModel.SelectedSession = session;
        }

        private void SessionEditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SessionList.SelectedIndex == -1) {
                return;
            }
            var session = SessionList.SelectedItem;
            var win = new NewSession {
                DataContext = session
            };
            win.Show();
        }

        private void SessionRemoveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SessionList.SelectedIndex == -1) {
                return;
            }
            var session = SessionList.SelectedItem as Session;
            if (session != null) {
                session.Delete();
            }
        }

        private void LogData_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogData.ScrollToEnd();
        }

        private void NewSessionBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new NewSession {
                DataContext = new Session()
            };
            win.Show();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow();
            window.ShowDialog();
        }

        private void SettingsMenuitem_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();
            window.ShowDialog();
        }
    }
}