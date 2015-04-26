using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.PublicFinder.Notices
{
    public class SuspiciousUrl401Notice : ModuleNotice
    {
        public override NoticePriority Priority
        {
            get { return NoticePriority.Low; }
        }

        public override string Text
        {
            get
            {
                string format = "Подозрительный URL: {0}, сервер вернул ошибку 401\n" +
                                "Заголовок `WWW-Authenticate`: {1}";
                return string.Format(format, this.LinkedUrl, this.HeaderString);
            }
        }

        private string HeaderString { get; set; }

        public SuspiciousUrl401Notice(Session session, string linkedUrl, string headerString)
            : base(session, linkedUrl)
        {
            this.HeaderString = headerString;
        }
    }
}