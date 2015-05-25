using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.PublicFinderModule.Notices
{
    public class SuspiciousUrl403Notice : ModuleNotice
    {
        private const string TextFormat = "Подозрительный URL: {0}, сервер вернул ошибку 403";

        public override NoticePriority Priority
        {
            get { return NoticePriority.Low; }
        }

        public override string Text
        {
            get { return string.Format(TextFormat, this.LinkedUrl); }
        }

        public SuspiciousUrl403Notice(Session session, string linkedUrl)
            : base(session, linkedUrl) { }
    }
}