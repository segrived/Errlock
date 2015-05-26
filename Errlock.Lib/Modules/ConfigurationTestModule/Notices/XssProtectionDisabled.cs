using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.ConfigurationTestModule.Notices
{
    public class XssProtectionDisabled : ModuleNotice
    {
        public XssProtectionDisabled(Session session, string linkedUrl) : base(session, linkedUrl) { }

        private const string TextFormat = "Отключена проверка на XSS-уязвимости браузеров.";

        public override NoticePriority Priority
        {
            get { return NoticePriority.High; }
        }
        public override string Text
        {
            get { return TextFormat; }
        }
    }
}
