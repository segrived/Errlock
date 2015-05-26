using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.ConfigurationTestModule.Notices
{
    class TooManyScriptsNotice : ModuleNotice
    {
        private int ScriptsCount { get; set; }

        private const string TextFormat = "На странице подключено слишком много внешних скриптов " +
                                          "({0} штук)";

        public TooManyScriptsNotice(string linkedUrl, int scriptsCount) 
            : base(linkedUrl)
        {
            this.ScriptsCount = scriptsCount;
        }

        public override string Text
        {
            get { return String.Format(TextFormat, ScriptsCount); }
        }

        protected override NoticePriority Priority
        {
            get
            {
                if (ScriptsCount <= 10) {
                    return NoticePriority.Low;
                } else {
                    return NoticePriority.Medium;
                }
            }
        }
    }
}
