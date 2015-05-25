using Errlock.Lib.Modules.PublicFinderModule;

namespace Errlock.ViewModels
{
    public class PublicFinderViewModel : BaseModuleViewModel<PublicFinder, PublicFinderConfig>
    {
        public PublicFinderViewModel() 
            : base(new PublicFinder(App.Config.Model.ConnectionConfiguration))
        { }
    }
}