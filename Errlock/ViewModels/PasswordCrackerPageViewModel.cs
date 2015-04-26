using Errlock.Lib;
using Errlock.Lib.Modules.PasswordCracker;
using Errlock.Lib.Modules.PublicFinder;

namespace Errlock.ViewModels
{
    public class PasswordCrackerPageViewModel : Bindable
    {
        public PasswordCrackerConfig Config { get; set; }

        public PasswordCrackerPageViewModel()
        {
            this.Config = new PasswordCrackerConfig {
                Login = "admin",
                RequestString = @"login.php?login={{login}}&password={{password}}",
                RequestType = RequestType.Post,
                InvalidPasswordAction = InvalidPasswordAction.Render403
            };
        }
    }
}
