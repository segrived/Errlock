﻿using System;
using System.Collections.Generic;
using System.Threading;
using Errlock.Lib.Helpers;
using Errlock.Lib.Logger;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules
{
    public enum NoticePriority
    {
        Info, // Информационное сообщение
        Low, // Низкая важность
        Medium, // Средняя важность
        High // Высокая важность
    }

    public abstract class Module<T> : IModule where T : ModuleConfig
    {
        protected T Config { get; private set; }
        protected ILogger Logger { get; private set; }
        private List<ModuleNotice> Notices { get; set; }
        private List<string> Messages { get; set; }
        protected CancellationTokenSource Token { get; set; }

        protected Module(T config)
        {
            this.Config = config;

            this.Notices = new List<ModuleNotice>();
            this.Messages = new List<string>();
        }

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
                Logger.Log("В данный момент тестируемый сайт недоступен, повторите попытку позже",
                    LoggerMessageType.Error);
                scanResult = GetScanResult(ModuleScanStatus.Error);
                this.OnCompleted(scanResult);
                return scanResult;
            }
            var status = Process(session);
            scanResult = GetScanResult(status);
            session.SaveLog(new SessionScanLog {
                Module = this.GetType().Name,
                ScanResult = scanResult
            });
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

        protected abstract ModuleScanStatus Process(Session session);

        private ModuleScanResult GetScanResult(ModuleScanStatus status)
        {
            return new ModuleScanResult(this.Notices, status, this.Messages);
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

        protected void AddMessage(string message, LoggerMessageType type)
        {
            this.Messages.Add(message);
            if (this.Logger != null) {
                this.Logger.Log(message, type);
            }
        }

        protected void AddNotice(ModuleNotice notice)
        {
            this.Notices.Add(notice);
            this.OnNewNotice(notice);
        }

        protected virtual void OnNewNotice(ModuleNotice notice)
        {
            var handler = this.NewNotice;
            handler.Raise(this, new ModuleNoticeEventArgs(notice));
        }
    }
}