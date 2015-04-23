using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.PublicFinder.Notices
{
    public class SuspiciousUrl403Notice : ModuleNotice
    {
        public override NoticePriority Priority
        {
            get { return NoticePriority.Low; }
        }

        public override string Text
        {
            get
            {
                return string.Format("Подозрительный URL: {0}, сервер вернул ошибку 403",
                    this.LinkedUrl);
            }
        }

        public SuspiciousUrl403Notice(Session session, string linkedUrl)
            : base(session, linkedUrl) { }
    }
}