using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Errlock.Lib.Helpers;
using Errlock.Lib.Logger;
using Errlock.Lib.RequestWrapper;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules
{
    public enum NoticePriority
    {
        [Description("Информация")]
        Info,

        [Description("Низкая")]
        Low,

        [Description("Средняя")]
        Medium,

        [Description("Высокая")]
        High
    }

    public abstract class Module<T> : IModule where T: ModuleConfig
    {
        public T Config { get; private set; }
        private ILogger Logger { get; set; }
        private List<ModuleNotice> Notices { get; set; }
        protected CancellationTokenSource Token { get; private set; }
        protected ConnectionConfiguration ConnectionConfiguration { get; private set; }

        protected Module(T moduleConfig, ConnectionConfiguration connectionConfig)
        {
            this.Config = moduleConfig;
            this.Notices = new List<ModuleNotice>();
            this.Progress = new Progress<int>();
            this.ConnectionConfiguration = connectionConfig;
        }

        protected virtual void ProcessConfig()
        {
            // Ничего не делаем
        }

        public void SetConfig(ModuleConfig config)
        {
            this.Config = config as T;
        }

        public Progress<int> Progress { get; private set; }
        public abstract bool IsSupportProgressReporting { get; }

        public void SetLogger(ILogger logger)
        {
            this.Logger = logger;
        }

        public event EventHandler<ModuleNoticeEventArgs> NewNotice;
        public event EventHandler Started;
        public event EventHandler<ModuleScanResultEventArgs> Completed;

        public ModuleScanResult Start(Session session)
        {
            this.Token = new CancellationTokenSource();
            ModuleScanResult scanResult;
            this.OnStarted();
            if (! WebHelpers.IsOnline(session.Url)) {
                const string msg = 
                    "В данный момент тестируемый сайт недоступен, повторите попытку позже";
                this.AddMessage(msg, LoggerMessageType.Error);
                scanResult = this.GetScanResult(ModuleScanStatus.SiteUnavailable);
                this.OnCompleted(scanResult);
                return scanResult;
            }
            this.ProcessConfig();
            var status = this.Process(session, this.Progress);
            scanResult = this.GetScanResult(status);

            this.OnCompleted(scanResult);
            return scanResult;
        }

        public void Stop()
        {
            if (this.Token == null) {
                return;
            }
            this.Token.Cancel();
        }

        protected abstract ModuleScanStatus Process(Session session, IProgress<int> progress);

        private ModuleScanResult GetScanResult(ModuleScanStatus status)
        {
            return new ModuleScanResult(this.Notices, status);
        }

        protected virtual void OnStarted()
        {
            var handler = this.Started;
            handler.Raise(this, EventArgs.Empty);
        }

        protected virtual void OnCompleted(ModuleScanResult scanResult)
        {
            var handler = this.Completed;
            handler.Raise(this, new ModuleScanResultEventArgs(scanResult));
        }

        protected virtual void OnNewNotice(ModuleNotice notice)
        {
            var handler = this.NewNotice;
            handler.Raise(this, new ModuleNoticeEventArgs(notice));
        }

        protected void AddMessage(string message, LoggerMessageType type)
        {
            if (this.Logger != null) {
                this.Logger.Log(message, type);
            }
        }

        protected void AddNotice(ModuleNotice notice)
        {
            this.Notices.Add(notice);
            this.OnNewNotice(notice);
        }
    }
}