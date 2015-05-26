namespace Errlock.Lib.Modules.PasswordCrackerModule.Notices
{
    public class PasswordMatchNotice : ModuleNotice
    {
        private string Login { get; set; }
        private string Password { get; set; }

        private const string TextFormat = "Возможно пароль подошел к аккунту, т.к. " +
                                          "сработал указанный триггер.\n" +
                                          "Логин: {0}, пароль: {1}";

        public override string Text
        {
            get { return string.Format(TextFormat, this.Login, this.Password); }
        }

        public override NoticePriority Priority
        {
            get { return NoticePriority.Info; }
        }

        public PasswordMatchNotice(string linkedUrl, string login, string password)
            : base(linkedUrl)
        {
            this.Login = login;
            this.Password = password;
        }
    }
}