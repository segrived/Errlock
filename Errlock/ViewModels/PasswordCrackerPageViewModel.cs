using Errlock.Lib;
using Errlock.Lib.Modules.PasswordCracker;
using Errlock.Lib.WebParser;

namespace Errlock.ViewModels
{
    public class PasswordCrackerPageViewModel : Bindable
    {
        public PasswordCrackerConfig Config { get; set; }

        public PasswordCrackerPageViewModel()
        {
            this.Config = new PasswordCrackerConfig {
                Login = "admin",
                RequestUrl = "login",
                RequestParameters = @"login={{login}}&password={{password}}",
                RequestType = RequestType.Post,
                InvalidPasswordAction = InvalidPasswordAction.Render403,
                PasswordsCount = 100,
                StopAfterFirstMatch = true
            };
        }
    }
}