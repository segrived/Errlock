using System;

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

        public override NoticePriority Priority
        {
            get
            {
                return (ScriptsCount <= 10) ? NoticePriority.Low : NoticePriority.Medium;
            }
        }
    }
}
