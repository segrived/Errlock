using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules;
using Errlock.Lib.Repository;
using Errlock.Lib.Sessions;
using Errlock.Locators;
using Errlock.Views;

namespace Errlock
{
    public partial class MainWindow
    {
        private readonly ViewModelLocator _locator = new ViewModelLocator();

        public MainWindow()
        {
            InitializeComponent();

            _locator.MainWindowViewModel.Sessions = App.SessionRepository.EnumerateAll();

            var events = App.SessionRepository as IRepositoryCollectionChanged<Session>;
            if (events != null) {
                var ri = events;
                ri.CollectionChanged += (sender, args) => {
                    string message = String.Empty;
                    switch (args.EventType) {
                        case CollectionEventType.Updated:
                            message = "Была добавлена или обновлена сессия: " + args.CollectionItem;
                            break;
                        case CollectionEventType.Deleted:
                            message = "Сессия `" + args.CollectionItem + "` была удалена";
                            break;
                    }
                    App.Logger.Log(message, LoggerMessageType.Info);
                    _locator.MainWindowViewModel.Sessions = App.SessionRepository.EnumerateAll();
                };
            }

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

        private async void StartStopModule_Click(object sender, RoutedEventArgs e)
        {
            var module = _locator.MainWindowViewModel.SelectedModule.Invoke();
            
            module.SetLogger(App.Logger);
            var session = this._locator.MainWindowViewModel.SelectedSession;

            // При поступлении нового предупреждения от модуля
            module.NewNotice +=
                (pSender, pe) => { App.Logger.Log(pe.Notice.Text, LoggerMessageType.Warn); };

            this.ModuleProgress.IsIndeterminate = !module.IsSupportProgressReporting;
            this.TaskbarItemInfo.ProgressState = module.IsSupportProgressReporting
                ? TaskbarItemProgressState.Normal
                : TaskbarItemProgressState.Indeterminate;

            module.Progress.ProgressChanged += (pSender, i) => {
                this.ModuleProgress.Value = i;
                this.TaskbarItemInfo.ProgressValue = (double)i / 100;
            };

            ModuleProgress.Visibility = Visibility.Visible;
            var scanResult = await Task.Factory.StartNew(() => {
                try {
                    return module.Start(session);
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