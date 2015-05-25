using System;
using Errlock.Lib.Logger;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules
{
    public interface IModule
    {
        /// <summary>
        /// Указывает, поддерживает ли модуль сообщения о прогрессе работы
        /// </summary>
        bool IsSupportProgressReporting { get; }

        Progress<int> Progress { get; }

        /// <summary>
        /// Указывает логгер, куда будут логгироваться сообщения модуля
        /// </summary>
        /// <param name="logger"></param>
        void SetLogger(ILogger logger);

        /// <summary>
        /// Запускает модуль и сканирует указанную сессию
        /// </summary>
        /// <param name="session">Сессия, которую необходимо просканировать</param>
        /// <returns></returns>
        ModuleScanResult Start(Session session);

        /// <summary>
        /// Останавилвает сканирования, если оно уже было начало
        /// </summary>
        void Stop();

        void SetConfig(ModuleConfig config);

        event EventHandler<ModuleNoticeEventArgs> NewNotice;
        event EventHandler Started;
        event EventHandler<ModuleScanResultEventArgs> Completed;
    }
}