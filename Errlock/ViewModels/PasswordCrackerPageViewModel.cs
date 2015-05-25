using Errlock.Lib.Modules.PasswordCrackerModule;

namespace Errlock.ViewModels
{
    public class PasswordCrackerPageViewModel : BaseModuleViewModel<PasswordCracker, PasswordCrackerConfig>
    {
        public PasswordCrackerPageViewModel()
            : base(new PasswordCracker(App.Config.Model.ConnectionConfiguration))
        { }

    }
}