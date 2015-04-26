using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.PasswordCracker.Notices
{
    public class PasswordMatchNotice : ModuleNotice
    {
        private string Login { get; set; }
        private string Password { get; set; }

        public PasswordMatchNotice(Session session, string linkedUrl, string login, string password)
            : base(session, linkedUrl)
        {
            this.Login = login;
            this.Password = password;
        }

        public override string Text
        {
            get
            {
                string format = "Возможно пароль подошел к аккунту, т.к. сработал указанный " +
                                "триггер.\nЛогин: {0}, пароль: {1}";
                return string.Format(format, this.Login, this.Password);
            }
        }

        public override NoticePriority Priority
        {
            get { return NoticePriority.Info; }
        }
    }
}
