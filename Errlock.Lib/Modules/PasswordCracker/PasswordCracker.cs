using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Errlock.Lib.Sessions;
using Errlock.Resources.ModulesData;

namespace Errlock.Lib.Modules.PasswordCracker
{
    public enum InvalidPasswordAction
    {
        [Description("Перенаправление обратно на страницу входа")]
        RedirectBack,
        [Description("Возврат 403 кода ошибки")]
        Render403
    }

    public enum RequestType
    {
        [Description("GET")]
        Get,
        [Description("POST")]
        Post
    }

    public class PasswordCracker : Module<PasswordCrackerConfig>
    {
        private List<string> PasswordList { get; set; }

        public PasswordCracker(PasswordCrackerConfig config) : base(config)
        {
            this.PasswordList = PasswordCrackerData.Passwords.Lines().ToList();
        }

        public override bool IsSupportProgressReporting
        {
            get { return true; }
        }

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            return ModuleScanStatus.Completed;
        }
    }
}
