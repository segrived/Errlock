namespace Errlock.Lib.Modules.ConfigurationTestModule.Notices
{
    public class XssProtectionDisabled : ModuleNotice
    {
        public XssProtectionDisabled(string linkedUrl) : base(linkedUrl) { }

        private const string TextFormat = "Отключена проверка на XSS-уязвимости браузеров.";

        protected override NoticePriority Priority
        {
            get { return NoticePriority.High; }
        }
        public override string Text
        {
            get { return TextFormat; }
        }
    }
}
