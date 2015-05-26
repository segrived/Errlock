using System;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.ConfigurationTestModule.Notices
{
    class NonProductionServerNotice : ModuleNotice
    {
        private string ServerName { get; set; }

        private const string TextFormat = "Используется сервер, не предназначенный " +
                                          "для работе в продакшене: {0}";

        public NonProductionServerNotice(string linkedUrl, string serverName)
            : base(linkedUrl)
        {
            this.ServerName = serverName;
        }

        public override string Text
        {
            get { return String.Format(TextFormat, this.ServerName); }
        }

        protected override NoticePriority Priority
        {
            get { return NoticePriority.Medium; }
        }
    }
}
