using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.PublicFinder.Notices
{
    public class OpenResourceNotice : ModuleNotice
    {
        public override NoticePriority Priority
        {
            get { return NoticePriority.Medium; }
        }

        public override string Text
        {
            get
            {
                string format = "Возможно открытый доступ по URL '{0}', рекомендуется проверить";
                return string.Format(format, this.LinkedUrl);
            }
        }

        public OpenResourceNotice(Session session, string linkedUrl)
            : base(session, linkedUrl) { }
    }
}