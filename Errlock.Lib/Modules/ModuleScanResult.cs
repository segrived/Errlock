using System.Collections.Generic;
using System.ComponentModel;

namespace Errlock.Lib.Modules
{
    public enum ModuleScanStatus
    {
        [Description("Завершен успешно")]
        Completed,

        [Description("Отменен")]
        Canceled,

        [Description("Завершен с ошибкой")]
        Error,

        [Description("Сайт недоступен")]
        SiteUnavailable
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

        internal ModuleScanResult(
            List<ModuleNotice> notices, ModuleScanStatus status)
        {
            this.Notices = notices;
            this.Status = status;
        }
    }
}