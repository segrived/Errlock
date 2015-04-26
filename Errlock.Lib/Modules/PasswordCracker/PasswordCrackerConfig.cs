namespace Errlock.Lib.Modules.PasswordCracker
{
    public class PasswordCrackerConfig : ModuleConfig
    {
        public string Login { get; set; }
        public string RequestString { get; set; }
        public InvalidPasswordAction InvalidPasswordAction { get; set; }
        public RequestType RequestType { get; set; }
    }
}
