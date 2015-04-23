using System.Collections.Generic;

namespace Errlock.Lib.Modules
{
    public enum ModuleScanStatus
    {
        Completed,
        Canceled,
        Error
    };

    public class ModuleScanResult
    {
        /// <summary>
        /// Все уведомления, полученные при работе модуля
        /// </summary>
        public List<ModuleNotice> Notices { get; private set; }

        /// <summary>
        /// Статус заверешния работы (завершен, отменен, либо завершен с ошибкой)
        /// </summary>
        public ModuleScanStatus Status { get; private set; }

        public List<string> LogMessages { get; private set; }

        internal ModuleScanResult(
            List<ModuleNotice> notices, ModuleScanStatus status, List<string> logMessages)
        {
            this.Notices = notices;
            this.Status = status;
            this.LogMessages = logMessages;
        }
    }
}