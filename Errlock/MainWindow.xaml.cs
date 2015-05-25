using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules;
using Errlock.Lib.Sessions;
using Errlock.Locators;
using Errlock.Views;

namespace Errlock
{
    public partial class MainWindow
    {
        private readonly ViewModelLocator _locator = new ViewModelLocator();

        public IModule CurrentModule { get; set; }
        public ModuleConfig ModuleConfig { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _locator.MainWindowViewModel.Sessions = App.SessionRepository.EnumerateAll();

            //Session.SessionChanged +=
            //    (sender, e) => {
            //        _locator.MainWindowViewModel.Sessions = Session.EnumerateSessions();
            //    };

            App.Logger.NewMessage += (sender, e) => {
                Action callback = () => LogData.AppendText(e.FormattedMessage + "\n");
                Dispatcher.Invoke(callback);
            };
        }

        private void SessionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var session = SessionList.SelectedItem as Session;
            _locator.MainWindowViewModel.SelectedSession = session;
        }

        private void SessionRemoveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SessionList.SelectedIndex == -1) {
                return;
            }
            var session = SessionList.SelectedItem as Session;
            if (session != null) {
                App.SessionRepository.Delete(session);
            }
        }

        private void LogData_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogData.ScrollToEnd();
        }

        private void NewSessionBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new NewSession();
            win.ShowDialog();
        }

        private void LogListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SessionList.SelectedIndex == -1) {
                return;
            }
            //var logFile = LogFilesList.SelectedItem as SessionLogFile;
            //new LogViewerView(logFile).ShowDialog();
        }

        private async void StartStopModule_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentModule.SetLogger(App.Logger);
            var session = this._locator.MainWindowViewModel.SelectedSession;

            // При поступлении нового предупреждения от модуля
            this.CurrentModule.NewNotice +=
                (pSender, pe) => { App.Logger.Log(pe.Notice.Text, LoggerMessageType.Warn); };

            this.ModuleProgress.IsIndeterminate = !this.CurrentModule.IsSupportProgressReporting;
            this.TaskbarItemInfo.ProgressState = this.CurrentModule.IsSupportProgressReporting
                ? TaskbarItemProgressState.Normal
                : TaskbarItemProgressState.Indeterminate;

            this.CurrentModule.Progress.ProgressChanged += (pSender, i) => {
                this.ModuleProgress.Value = i;
                this.TaskbarItemInfo.ProgressValue = (double)i / 100;
            };

            ModuleProgress.Visibility = Visibility.Visible;
            var scanResult = await Task.Factory.StartNew(() => {
                try {
                    return this.CurrentModule.Start(session);
                } catch (WebException ex) {
                    App.Logger.Log(ex.Message, LoggerMessageType.Error);
                    return null;
                }
            });
            ModuleProgress.Value = 0;
            ModuleProgress.Visibility = Visibility.Hidden;
            if (scanResult == null) {
                return;
            }
            if (scanResult.Status == ModuleScanStatus.Error) {
                this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            } else {
                this.TaskbarItemInfo.ProgressValue = 0;
                this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            }
            if (scanResult.Status != ModuleScanStatus.SiteUnavailable) {
                new ScanResultWindowView(scanResult).ShowDialog();
            }
        }
    }
}