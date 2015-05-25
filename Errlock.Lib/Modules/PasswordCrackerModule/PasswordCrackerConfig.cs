using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.Modules.PasswordCrackerModule
{
    public class PasswordCrackerConfig : ModuleConfig
    {
        public string Login { get; set; }
        public string RequestUrl { get; set; }
        public string RequestParameters { get; set; }
        public InvalidPasswordAction InvalidPasswordAction { get; set; }
        public RequestMethod RequestMethod { get; set; }
        public int PasswordsCount { get; set; }
        public bool StopAfterFirstMatch { get; set; }
        public bool UseHeuristic { get; set; }
    }
}