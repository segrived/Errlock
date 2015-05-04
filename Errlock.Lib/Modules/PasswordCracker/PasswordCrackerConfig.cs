using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.Modules.PasswordCracker
{
    public class PasswordCrackerConfig : ModuleConfig
    {
        public string Login { get; set; }
        public string RequestUrl { get; set; }
        public string RequestParameters { get; set; }
        public InvalidPasswordAction InvalidPasswordAction { get; set; }
        public RequestType RequestType { get; set; }
        public int PasswordsCount { get; set; }
        public bool StopAfterFirstMatch { get; set; }
        public bool UseHeuristic { get; set; }
    }
}